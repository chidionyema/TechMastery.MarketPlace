using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Http;
using Serilog;
using Microsoft.Extensions.Configuration;
using System.Linq;

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
            var app = ConfigureApplicationServices(builder);

            RegisterMiddlewares(app);
            ConfigureHealthChecks(app, builder.Configuration);

            return app;
        }

        private static WebApplication ConfigureApplicationServices(WebApplicationBuilder builder)
        {
            // Assuming these methods (ConfigureServices and ConfigurePipeline) are part of some extension methods for the WebApplicationBuilder
            return builder.ConfigureServices()
                          .ConfigurePipeline();
        }

        private static void RegisterMiddlewares(WebApplication app)
        {
            app.UseSerilogRequestLogging();
            app.UseCors("AllowOrigin");
        }

        private static void ConfigureHealthChecks(WebApplication app, IConfiguration configuration)
        {
            var rabbitMqHost = configuration["MessagingSystems:RabbitMQ:Host"];
            Log.Information($"Loaded RabbitMQ Host: {rabbitMqHost}");

            app.MapHealthChecks("/health", new HealthCheckOptions
            {
                ResponseWriter = WriteHealthCheckResponseAsync,
                Predicate = _ => true,
                ResultStatusCodes =
                {
                    [HealthStatus.Healthy] = StatusCodes.Status200OK,
                    [HealthStatus.Degraded] = StatusCodes.Status200OK,
                    [HealthStatus.Unhealthy] = StatusCodes.Status503ServiceUnavailable
                }
            });
        }

        private static Task WriteHealthCheckResponseAsync(HttpContext context, HealthReport report)
        {
            context.Response.ContentType = "application/json";

            var response = new
            {
                Status = report.Status.ToString(),
                Checks = report.Entries.Select(entry => new
                {
                    Name = entry.Key,
                    Status = entry.Value.Status.ToString(),
                    Description = entry.Value.Description
                })
            };

            var jsonResponse = JsonSerializer.Serialize(response, new JsonSerializerOptions { WriteIndented = true });

            return context.Response.WriteAsync(jsonResponse);
        }
    }
}
