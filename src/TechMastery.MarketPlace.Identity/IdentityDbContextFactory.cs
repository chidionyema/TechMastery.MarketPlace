using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Npgsql.EntityFrameworkCore.PostgreSQL;
using Microsoft.Extensions.Hosting;
using System;
using TechMastery.MarketPlace.Identity;

namespace TechMastery.MarketPlace.Persistence
{
    public class ApplicationIdentityDbContextFactory : IDesignTimeDbContextFactory<ApplicationIdentityDbContext>
    {
        public ApplicationIdentityDbContext CreateDbContext(string[] args)
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

            var optionsBuilder = new DbContextOptionsBuilder<ApplicationIdentityDbContext>();
            optionsBuilder.UseNpgsql(configuration.GetConnectionString("TechMasteryMarkePlaceIdentityConnectionStringIdentityConnectionString"));

            return new ApplicationIdentityDbContext(optionsBuilder.Options);
        }
    }
}
