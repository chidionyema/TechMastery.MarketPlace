using MassTransit;
using Microsoft.Extensions.Diagnostics.HealthChecks;

public class RabbitMqHealthCheck : IHealthCheck
{
    private readonly IBus _bus;

    public RabbitMqHealthCheck(IBus bus)
    {
        _bus = bus;
    }

    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        try
        {
            var probeResult =  _bus.GetProbeResult();

            // Additional checks on probeResult can be done here if required.

            return HealthCheckResult.Healthy();
        }
        catch (Exception ex)
        {
            return HealthCheckResult.Unhealthy("Unable to communicate with RabbitMQ.", ex);
        }
    }
}
