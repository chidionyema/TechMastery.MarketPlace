using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MassTransit;
using TechMastery.Messaging.Consumers.Consumers;
using TechMastery.MarketPlace.Application.Contracts.Messaging;
using Microsoft.Extensions.Options;

namespace TechMastery.Messaging.Consumers
{
    public static class MessagingServiceRegistration
    {
        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
        {
            if (services == null)
                throw new ArgumentNullException(nameof(services));

            if (configuration == null)
                throw new ArgumentNullException(nameof(configuration));

            // Load messaging system options from configuration
            services.Configure<MessagingSystemsOptions>(configuration.GetSection("MessagingSystems"));

            var serviceProvider = services.BuildServiceProvider();
            var messagingSystemsOptions = serviceProvider.GetRequiredService<IOptions<MessagingSystemsOptions>>().Value;

            // Add MassTransit with Azure Service Bus if enabled
            if (messagingSystemsOptions.EnableAzureServiceBus)
            {
                services.AddMassTransitWithAzureServiceBus(configuration.GetSection("MessagingSystems:AzureServiceBus"));
            }

            // Add MassTransit with SQS if enabled
            if (messagingSystemsOptions.EnableSqs)
            {
                services.AddMassTransitWithSqs(configuration.GetSection("MessagingSystems:SQS"));
            }

            // Add MassTransit with RabbitMQ if enabled
            if (messagingSystemsOptions.EnableRabbitMq)
            {
                services.AddMassTransitWithRabbitMq(configuration.GetSection("MessagingSystems:RabbitMQ"));
            }

            // Register IMessagePublisher as a singleton
            services.AddSingleton<IMessagePublisher, MessagePublisher>();

            return services;
        }

        private static void AddMassTransitWithAzureServiceBus(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddMassTransit(configure =>
            {
                var options = new AzureServiceBusOptions();
                configuration.Bind(options);

                configure.UsingAzureServiceBus((context, cfg) =>
                {
                    cfg.Host(options.ConnectionString);

                    cfg.ReceiveEndpoint(options.QueueName, e =>
                    {
                        // Retry policy for transient failures
                        e.UseMessageRetry(r => r.Interval(3, TimeSpan.FromSeconds(5)));

                        // Schedule message redelivery for non-transient failures
                        e.UseScheduledRedelivery(r => r.Intervals(TimeSpan.FromSeconds(30), TimeSpan.FromMinutes(1), TimeSpan.FromMinutes(5)));

                        // Register consumers
                        e.Consumer<ProductAddedConsumer>();
                        e.Consumer<OrderPlacedConsumer>();
                    });
                });
            });

            services.AddMassTransitHostedService();
        }

        private static void AddMassTransitWithSqs(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddMassTransit(configure =>
            {
                var options = new SqsOptions();
                configuration.Bind(options);

                configure.UsingAmazonSqs((context, cfg) =>
                {
                    cfg.Host(options.Region, h =>
                    {
                        h.AccessKey(options.AccessKey);
                        h.SecretKey(options.SecretKey);
                    });

                    cfg.ReceiveEndpoint(options.QueueUrl, e =>
                    {
                        // Retry policy for transient failures
                        e.UseMessageRetry(r => r.Interval(3, TimeSpan.FromSeconds(5)));

                        // Schedule message redelivery for non-transient failures
                        e.UseScheduledRedelivery(r => r.Intervals(TimeSpan.FromSeconds(30), TimeSpan.FromMinutes(1), TimeSpan.FromMinutes(5)));

                        // Register consumers
                        e.Consumer<ProductAddedConsumer>();
                        e.Consumer<OrderPlacedConsumer>();
                    });
                });
            });

            services.AddMassTransitHostedService();
        }

        private static void AddMassTransitWithRabbitMq(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddMassTransit(configure =>
            {
                var options = new RabbitMqOptions();
                configuration.Bind(options);

                configure.UsingRabbitMq((context, cfg) =>
                {
                    cfg.Host(options.Host, h =>
                    {
                        h.Username(options.Username);
                        h.Password(options.Password);
                    });

                    cfg.ReceiveEndpoint(options.QueueName, e =>
                    {
                        // Retry policy for transient failures
                        e.UseMessageRetry(r => r.Interval(3, TimeSpan.FromSeconds(5)));

                        // Schedule message redelivery for non-transient failures
                        e.UseScheduledRedelivery(r => r.Intervals(TimeSpan.FromSeconds(30), TimeSpan.FromMinutes(1), TimeSpan.FromMinutes(5)));

                        // Register consumers
                        e.Consumer<ProductAddedConsumer>();
                        e.Consumer<OrderPlacedConsumer>();
                    });
                });
            });

            services.AddMassTransitHostedService();
        }
    }
}
