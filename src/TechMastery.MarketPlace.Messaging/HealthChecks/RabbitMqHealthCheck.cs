using MassTransit;
using Microsoft.Extensions.Diagnostics.HealthChecks;

public class RabbitMqHealthCheck : IHealthCheck
{
    private readonly IBusControl _busControl;

    public RabbitMqHealthCheck(IBusControl busControl)
    {
        _busControl = busControl;
    }

    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        try
        {
            using var timeoutCancellationTokenSource = new CancellationTokenSource(TimeSpan.FromSeconds(5));
            using var linkedCancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(timeoutCancellationTokenSource.Token, cancellationToken);

            await _busControl.StartAsync(linkedCancellationTokenSource.Token);
            await _busControl.StopAsync(linkedCancellationTokenSource.Token);

            return HealthCheckResult.Healthy();
        }
        catch (Exception ex)
        {
            return HealthCheckResult.Unhealthy("Failed to start and stop RabbitMQ bus control.", ex);
        }
    }
}
