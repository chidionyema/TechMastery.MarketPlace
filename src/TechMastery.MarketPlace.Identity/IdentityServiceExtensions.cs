using System;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using TechMastery.MarketPlace.Application.Contracts.Identity;
using TechMastery.MarketPlace.Application.Models.Authentication;
using TechMastery.MarketPlace.Identity.Services;

namespace TechMastery.MarketPlace.Identity
{
    public static class IdentityServiceExtensions
    {
        public static void AddIdentityServices(this IServiceCollection services, IConfiguration configuration)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            if (configuration == null)
            {
                throw new ArgumentNullException(nameof(configuration));
            }

          
            // Configure JWT settings
            var jwtSettings = configuration.GetSection("JwtSettings").Get<JwtSettings>();
            if (jwtSettings == null)
            {
                throw new ArgumentNullException("JwtSettings is missing or invalid.");
            }
            services.Configure<JwtSettings>(configuration.GetSection("JwtSettings"));

            // Configure database context with migrations
            var identityDbConnectionString = configuration.GetConnectionString("TechMasteryMarketPlaceIdentityConnectionString");
            if (string.IsNullOrWhiteSpace(identityDbConnectionString))
            {
                throw new ArgumentNullException("Identity DB connection string is missing or invalid.");
            }
            services.AddDbContext<ApplicationIdentityDbContext>(options =>
                options.UseSqlServer(identityDbConnectionString, b =>
                    b.MigrationsAssembly(typeof(ApplicationIdentityDbContext).Assembly.FullName)));

            // Configure Identity
            services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<ApplicationIdentityDbContext>()
                .AddDefaultTokenProviders();

            // Add authentication service
            services.AddTransient<IAuthenticationService, AuthenticationService>();

            // Configure authentication
            // ... existing code ...

            // Configure authentication
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(o =>
            {
                o.RequireHttpsMetadata = false;
                o.SaveToken = false;
                o.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero,
                    ValidIssuer = jwtSettings.Issuer,
                    ValidAudience = jwtSettings.Audience,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Key))
                };

                o.Events = new JwtBearerEvents()
                {
                    OnAuthenticationFailed = c =>
                    {
                        c.NoResult();
                        c.Response.StatusCode = 500;
                        c.Response.ContentType = "text/plain";
                        return c.Response.WriteAsync(c.Exception.ToString());
                    },
                    OnChallenge = context =>
                    {
                        context.HandleResponse();
                        context.Response.StatusCode = 401;
                        context.Response.ContentType = "application/json";
                        var result = JsonSerializer.Serialize("401 Not authorized");
                        return context.Response.WriteAsync(result);
                    },
                    OnForbidden = context =>
                    {
                        context.Response.StatusCode = 403;
                        context.Response.ContentType = "application/json";
                        var result = JsonSerializer.Serialize("403 Not authorized");
                        return context.Response.WriteAsync(result);
                    },
                    OnTokenValidated = context =>
                    {
                        var result = JsonSerializer.Serialize("Validated");
                        return context.Response.WriteAsync(result);
                    },
                    OnMessageReceived = context =>
                    {
                        var result = JsonSerializer.Serialize("Received");
                        return context.Response.WriteAsync(result);
                    }
                };
            });

            // Add health checks
            services.AddHealthChecks()
                .AddCheck<IdentityDbContextHealthCheck>("IdentityDbContextHealthCheck");
        }
    }
}