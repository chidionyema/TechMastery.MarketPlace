using MassTransit;
using Microsoft.Extensions.DependencyInjection;
namespace TechMastery.Messaging
{
    internal static class RabbitMqConfigurator
    {
        public static void Configure(IServiceCollection services, RabbitMqOptions options, params Type[] consumerTypes)
        {
            services.AddMassTransit(configure =>
            {
                configure.UsingRabbitMq((context, cfg) =>
                {
                    var rabbitMqUri = new Uri($"rabbitmq://{options.Host}/");
                    cfg.Host(rabbitMqUri, h =>
                    {
                        h.Username(options.Username);
                        h.Password(options.Password);
                    });

                    ConsumerConfigurator.Configure(cfg, options.QueueName!, consumerTypes);
                });
            });
        }
    }
}
