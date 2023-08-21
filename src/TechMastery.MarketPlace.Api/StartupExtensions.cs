using Microsoft.OpenApi.Models;
using TechMastery.MarketPlace.Application;
using TechMastery.MarketPlace.Application.Contracts;
using TechMastery.MarketPlace.Application.Services;
using TechMastery.MarketPlace.Identity;
using TechMastery.MarketPlace.Infrastructure;
using TechMastery.MarketPlace.Persistence;
using TechMastery.Messaging.Consumers;
using TechMastery.MarketPlace.Api.Middleware;
using TechMastery.MarketPlace.Api.Utility;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Newtonsoft.Json;

namespace TechMastery.MarketPlace.Api
{
    public static class StartupExtensions
    {
        /// <summary>
        /// Configures services for the application.
        /// </summary>
        public static WebApplication ConfigureServices(this WebApplicationBuilder builder)
        {
            AddCoreServices(builder);
            AddApplicationSpecificServices(builder);
            ConfigureSwaggerServices(builder.Services);

            builder.Services.AddControllers();
            return builder.Build();
        }

        /// <summary>
        /// Configures the application's request/response pipeline.
        /// </summary>
        public static WebApplication ConfigurePipeline(this WebApplication app)
        {
            var env = app.Services.GetRequiredService<IWebHostEnvironment>();

            if (env.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "TechMastery API"));
            }

            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseCors("Open");
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseCustomExceptionHandler();
            app.MapControllers();

            return app;
        }

        /// <summary>
        /// Configures CORS for the application.
        /// </summary>
        public static void ConfigureCors(this WebApplicationBuilder builder)
        {
            var allowedOrigins = builder.Configuration.GetSection("AllowedCorsOrigins").Get<string[]>();
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowOrigin", builder =>
                {
                    builder.WithOrigins(allowedOrigins)
                           .AllowAnyHeader()
                           .AllowAnyMethod();
                });
            });
        }

        /// <summary>
        /// Adds core services that are fundamental to the application's operations.
        /// </summary>
        private static void AddCoreServices(WebApplicationBuilder builder)
        {
            builder.Services.AddHealthChecks();
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("Open", policy => policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());
            });
        }

        /// <summary>
        /// Adds application-specific services.
        /// </summary>
        private static void AddApplicationSpecificServices(WebApplicationBuilder builder)
        {
            builder.Services.AddApplicationServices();
            builder.Services.AddInfrastructureServices(builder.Configuration);
            builder.Services.AddElasticsearchClient(builder.Configuration);
            builder.Services.AddStorageProvider(builder.Configuration, StorageProviderType.AzureBlobStorage);
            builder.Services.AddPersistenceServices(builder.Configuration);
            builder.Services.AddIdentityServices(builder.Configuration);
            builder.Services.AddMessagingServices(builder.Configuration);
            builder.Services.AddStripePaymentService(builder.Configuration);
            builder.Services.AddScoped<ILoggedInUserService, LoggedInUserService>();
            builder.Services.AddHttpContextAccessor();
        }

        /// <summary>
        /// Configures Swagger for API documentation.
        /// </summary>
        private static void ConfigureSwaggerServices(IServiceCollection services)
        {
            services.AddSwaggerGen(c =>
            {
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = "JWT Authorization header using the Bearer scheme. Example: 'Bearer 12345abcdef'",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer"
                });

                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            },
                            Scheme = "oauth2",
                            Name = "Bearer",
                            In = ParameterLocation.Header,
                        },
                        new List<string>()
                    }
                });

                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Version = "v1",
                    Title = "TechMastery MarketPlace API",
                });

                c.OperationFilter<FileResultContentTypeOperationFilter>();
            });
        }
    }
}
