using MassTransit;
using Microsoft.Extensions.DependencyInjection;
namespace TechMastery.Messaging
{
    internal static class SqsConfigurator
    {
        public static void Configure(IServiceCollection services, SqsOptions options, params Type[] consumerTypes)
        {
            services.AddMassTransit(configure =>
            {
                configure.UsingAmazonSqs((context, cfg) =>
                {
                    cfg.Host(options.Region, h =>
                    {
                        h.AccessKey(options.AccessKey);
                        h.SecretKey(options.SecretKey);
                    });

                    ConsumerConfigurator.Configure(cfg, options.ServiceUrl!, consumerTypes);
                });
            });
        }
    }
}
