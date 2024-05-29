using System.Text.Json;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Serilog;

namespace TechMastery.MarketPlace.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            ConfigureLogging(builder.Configuration);
            var app = InitializeApplication(builder);
            app.Run();
        }

        private static void ConfigureLogging(IConfiguration configuration)
        {
            Log.Logger = new LoggerConfiguration()
                .WriteTo.Console()
                .ReadFrom.Configuration(configuration)
                .CreateBootstrapLogger();

            Log.Information("TechMastery API starting");
        }

        private static WebApplication InitializeApplication(WebApplicationBuilder builder)
        {
            builder.Host.UseSerilog();
            builder.Services.AddHealthChecks()
                .AddCheck<BasicHealthCheck>("my_health_check");
            var app = ConfigureApplicationServices(builder);

            RegisterMiddlewares(app);

            ConfigureHealthChecks(app, builder.Configuration);

            return app;
        }


        private static WebApplication ConfigureApplicationServices(WebApplicationBuilder builder)
        {
            return builder.ConfigureServices()
                          .ConfigurePipeline();
        }

        private static void RegisterMiddlewares(WebApplication app)
        {
            app.UseSerilogRequestLogging();
            app.UseCors("AllowSpecificOrigins");
        }

        private static void ConfigureHealthChecks(WebApplication app, IConfiguration configuration)
        {
            app.MapHealthChecks("/healthz", new HealthCheckOptions
            {
                Predicate = _ => true, // Include all registered health checks.
                ResponseWriter = (context, report) =>
                {
                    try
                    {
                        var response = new
                        {
                            Status = report.Status.ToString(),
                            Checks = report.Entries.ToDictionary(
                                e => e.Key,
                                e => e.Value.Status.ToString()
                            )
                        };
                        context.Response.ContentType = "application/json";
                        return context.Response.WriteAsync(JsonSerializer.Serialize(response));
                    }
                    catch (Exception ex)
                    {
                        // Log the exception for debugging purposes
                      //  _logger.LogError(ex, "Error in health check response generation");
                        context.Response.ContentType = "application/json";
                        context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                        return context.Response.WriteAsync("Internal Server Error");
                    }
                }
            });
        }
    }



    public class BasicHealthCheck : IHealthCheck
    {
        public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            // In this simple example, always return "Healthy" regardless of actual conditions.
            return Task.FromResult(HealthCheckResult.Healthy("Always healthy"));
        }
    }

}
