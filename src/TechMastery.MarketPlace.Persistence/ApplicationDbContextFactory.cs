using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Npgsql.EntityFrameworkCore.PostgreSQL;
using Microsoft.Extensions.Hosting;
using System;

namespace TechMastery.MarketPlace.Persistence
{
    public class ApplicationDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
    {
        public ApplicationDbContext CreateDbContext(string[] args)
        {
            var hostBuilder = Host.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration((hostContext, config) =>
                {
                    var currentDirectory = Directory.GetCurrentDirectory();
                    var appSettingsPath = Path.Combine(currentDirectory, "..", "TechMastery.MarketPlace.Api", "appsettings.json");
                    config.AddJsonFile(appSettingsPath, optional: false, reloadOnChange: true);
                    config.AddJsonFile(Path.Combine(currentDirectory, "..", "TechMastery.MarketPlace.Api", $"appsettings.{hostContext.HostingEnvironment.EnvironmentName}.json"), optional: true, reloadOnChange: true);
                })
                .Build();

            var configuration = hostBuilder.Services.GetRequiredService<IConfiguration>();

            var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
            optionsBuilder.UseNpgsql(configuration.GetConnectionString("GloboTicketTicketManagementConnectionString"));

            return new ApplicationDbContext(optionsBuilder.Options);
        }
    }
}
