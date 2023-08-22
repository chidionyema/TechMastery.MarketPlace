using MassTransit;
using Microsoft.Extensions.DependencyInjection;

namespace TechMastery.Messaging.Consumers
{
    internal static class AzureServiceBusConfigurator
    {
        public static void Configure(IServiceCollection services, AzureServiceBusOptions options)
        {
            services.AddMassTransit(configure =>
            {
                configure.UsingAzureServiceBus((context, cfg) =>
                {
                    cfg.Host(options.ConnectionString);
                    ConsumerConfigurator.Configure(cfg, options.QueueName!);
                });
            });
        }
    }
}
