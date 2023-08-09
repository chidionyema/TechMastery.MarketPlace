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

            // Use Serilog for request logging
            app.UseSerilogRequestLogging();

            app.Run();
        }
    }
}
