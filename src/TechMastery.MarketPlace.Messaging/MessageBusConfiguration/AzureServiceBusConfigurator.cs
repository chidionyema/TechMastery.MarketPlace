using MassTransit;
using Microsoft.Extensions.DependencyInjection;

namespace TechMastery.Messaging
{
    internal static class AzureServiceBusConfigurator
    {
        public static void Configure(IServiceCollection services, AzureServiceBusOptions options, params Type[] consumerTypes)
        {
            services.AddMassTransit(configure =>
            {
                configure.UsingAzureServiceBus((context, cfg) =>
                {
                    cfg.Host(options.ConnectionString);
                    ConsumerConfigurator.Configure(cfg, options.QueueName!, consumerTypes);
                });
            });
        }
    }
}
