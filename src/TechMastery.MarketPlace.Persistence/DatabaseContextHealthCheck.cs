using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace TechMastery.MarketPlace.Persistence
{
    internal class DatabaseContextHealthCheck : IHealthCheck
    {
        private readonly ApplicationDbContext _dbContext;

        public DatabaseContextHealthCheck(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            try
            {
                var connectionState = _dbContext.Database.GetDbConnection().State;

                if (connectionState == System.Data.ConnectionState.Open)
                {
                    return HealthCheckResult.Healthy("Database connection is open.");
                }
                else
                {
                    return HealthCheckResult.Unhealthy("Database connection is not open.");
                }
            }
            catch (Exception ex)
            {
                return HealthCheckResult.Unhealthy("An exception occurred while checking the database connection.", ex);
            }
        }
    }
}