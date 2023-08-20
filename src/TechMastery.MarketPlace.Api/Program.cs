using System.Text.Json;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Serilog;

namespace TechMastery.MarketPlace.Api
{
    public partial class Program
    {
        public static void Main(string[] args)
        {
            // Configure Serilog logger
            Log.Logger = new LoggerConfiguration()
                .WriteTo.Console()
                .CreateBootstrapLogger();

            Log.Information("TechMastery API starting");

            var builder = WebApplication.CreateBuilder(args);

            // Use Serilog for host and request logging
            builder.Host.UseSerilog((context, loggerConfiguration) => loggerConfiguration
                 .WriteTo.Console()
                 .ReadFrom.Configuration(context.Configuration));

            var app = builder
                .ConfigureServices()
                .ConfigurePipeline();

            var rabbitMqHost = builder.Configuration["MessagingSystems:RabbitMQ:Host"];
            Log.Information($"Loaded RabbitMQ Host: {rabbitMqHost}");


            // Health check configuration
            app.MapHealthChecks("/health", new HealthCheckOptions
            {
                ResponseWriter = HealthCheckResponseWriter,
                Predicate = _ => true,
                ResultStatusCodes =
                {
                    [HealthStatus.Healthy] = StatusCodes.Status200OK,
                    [HealthStatus.Degraded] = StatusCodes.Status200OK,
                    [HealthStatus.Unhealthy] = StatusCodes.Status503ServiceUnavailable
                }
            });

            // Use Serilog for request logging
            app.UseSerilogRequestLogging();

            app.Run();
        }

        private static async Task HealthCheckResponseWriter(HttpContext context, HealthReport report)
        {
            context.Response.ContentType = "application/json";
            await JsonSerializer.SerializeAsync(context.Response.Body, report);

        }
    }
}
