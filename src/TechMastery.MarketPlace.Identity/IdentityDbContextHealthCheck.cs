using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace TechMastery.MarketPlace.Identity
{
    public class IdentityDbContextHealthCheck : IHealthCheck
    {
        private readonly ApplicationIdentityDbContext _dbContext;

        public IdentityDbContextHealthCheck(ApplicationIdentityDbContext dbContext)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        }

        public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            try
            {
                var connectionState = _dbContext.Database.GetDbConnection().State;

                if (connectionState == System.Data.ConnectionState.Open)
                {
                    return HealthCheckResult.Healthy("Identity database connection is open.");
                }
                else
                {
                    return HealthCheckResult.Unhealthy("Identity database connection is not open.");
                }
            }
            catch (Exception ex)
            {
                return HealthCheckResult.Unhealthy("An exception occurred while checking the identity database connection.", ex);
            }
        }
    }
}

