using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MassTransit;
using Serilog;
using TechMastery.Messaging.Consumers.Consumers;
using TechMastery.MarketPlace.Application.Contracts.Messaging;
using Microsoft.Extensions.Options;
namespace TechMastery.Messaging.Consumers
{
    public static class MessagingServiceRegistration
    {
        public static IServiceCollection AddMessagingServices(this IServiceCollection services, IConfiguration configuration)
        {
            if (services == null)
                throw new ArgumentNullException(nameof(services));

            if (configuration == null)
                throw new ArgumentNullException(nameof(configuration));

            // Configure logging
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug() // Set your desired minimum log level
                .CreateLogger();

            services.AddLogging(loggingBuilder =>
            {
                loggingBuilder.ClearProviders();
                loggingBuilder.AddSerilog(dispose: true);
            });

            // Load messaging system options from configuration
            services.Configure<MessagingSystemsOptions>(configuration.GetSection("MessagingSystems"));

            var serviceProvider = services.BuildServiceProvider();
            var messagingSystemsOptions = serviceProvider.GetRequiredService<IOptions<MessagingSystemsOptions>>().Value;

            // Add MassTransit with Azure Service Bus if enabled
            if (messagingSystemsOptions.EnableAzureServiceBus)
            {
                services.AddHealthChecks().AddCheck<AzureServiceBusHealthCheck>("AzureServiceBusHealthCheck");
                services.AddMassTransitWithAzureServiceBus(messagingSystemsOptions.AzureServiceBus!);
            }

            if (messagingSystemsOptions.EnableSqs)
            {
                services.AddHealthChecks().AddCheck<AmazonSqsHealthCheck>("AmazonSqsHealthCheck");
                services.AddMassTransitWithSqs(messagingSystemsOptions.Sqs!);
            }

            if (messagingSystemsOptions.EnableRabbitMq)
            {
                services.AddHealthChecks().AddCheck<RabbitMqHealthCheck>("RabbitMqHealthCheck");
                services.AddMassTransitWithRabbitMq(messagingSystemsOptions.RabbitMq!);
            }

            // Register BusControlConfigurator as a singleton
            services.AddSingleton(provider =>
            {
                var options = provider.GetRequiredService<MessagingSystemsOptions>();
                return new BusControlConfigurator(options);
            });

            // Register IMessagePublisher as a singleton
            services.AddSingleton<IMessagePublisher>(provider =>
            {
                var busControlConfigurator = provider.GetRequiredService<BusControlConfigurator>();
                var logger = provider.GetRequiredService<ILogger<MessagePublisher>>(); // Inject the logger
                return new MessagePublisher(busControlConfigurator, logger);
            });

            return services;
        }

        private static void AddMassTransitWithAzureServiceBus(this IServiceCollection services, AzureServiceBusOptions serviceBusOptions)
        {
            if (serviceBusOptions == null || string.IsNullOrEmpty(serviceBusOptions.QueueName) || string.IsNullOrEmpty(serviceBusOptions.ConnectionString))

            {
                throw new ArgumentException("SQS configuration is incomplete.");
            }

            services.AddMassTransit(configure =>
            {

                configure.UsingAzureServiceBus((context, cfg) =>
                {
                    cfg.Host(serviceBusOptions.ConnectionString);

                    cfg.ReceiveEndpoint(serviceBusOptions.QueueName, e =>
                    {
                        ConfigureReceiveEndpoint(e);

                        // Register consumers
                        e.Consumer<ProductAddedConsumer>();
                        e.Consumer<OrderPlacedConsumer>();
                    });
                });
            });
            
        }

        private static void AddMassTransitWithSqs(this IServiceCollection services, SqsOptions sqsOptions)
        {
            if (sqsOptions == null || string.IsNullOrEmpty(sqsOptions.AccessKey) || string.IsNullOrEmpty(sqsOptions.SecretKey))
        
            {
                throw new ArgumentException("SQS configuration is incomplete.");
            }

            services.AddMassTransit(configure =>
            {

                configure.UsingAmazonSqs((context, cfg) =>
                {
                    cfg.Host(sqsOptions.Region, h =>
                    {
                        h.AccessKey(sqsOptions.AccessKey);
                        h.SecretKey(sqsOptions.SecretKey);
                    });

                    cfg.ReceiveEndpoint(sqsOptions.QueueUrl, e =>
                    {
                        ConfigureReceiveEndpoint(e);

                        // Register consumers
                        e.Consumer<ProductAddedConsumer>();
                        e.Consumer<OrderPlacedConsumer>();
                    });
                });
            });

            
        }

        public static void AddMassTransitWithRabbitMq(this IServiceCollection services, RabbitMqOptions rabbitMqOptions)
        {
            if (rabbitMqOptions == null || string.IsNullOrEmpty(rabbitMqOptions.Host) ||  string.IsNullOrEmpty(rabbitMqOptions.Username)
                || string.IsNullOrEmpty(rabbitMqOptions.Password)
                || string.IsNullOrEmpty(rabbitMqOptions.QueueName)) {
                throw new ArgumentException("RabbitMQ configuration is incomplete.");
            }

            services.AddMassTransit(configure =>
            {
                configure.UsingRabbitMq((context, cfg) =>
                {
                    cfg.Host(rabbitMqOptions.Host, h =>
                    {
                        h.Username(rabbitMqOptions.Username);
                        h.Password(rabbitMqOptions.Password);
                    });

                    cfg.ReceiveEndpoint(rabbitMqOptions.QueueName, e =>
                    {
                        ConfigureReceiveEndpoint(e);

                        // Assuming ProductAddedConsumer and OrderPlacedConsumer are already registered with the DI container
                        e.Consumer<ProductAddedConsumer>();
                        e.Consumer<OrderPlacedConsumer>();
                    });
                });
            });
        }

        private static void ConfigureReceiveEndpoint(IReceiveEndpointConfigurator receiveEndpoint)
        {
            receiveEndpoint.UseMessageRetry(r => r.Interval(3, TimeSpan.FromSeconds(5)));
            receiveEndpoint.UseScheduledRedelivery(r => r.Intervals(TimeSpan.FromSeconds(30), TimeSpan.FromMinutes(1), TimeSpan.FromMinutes(5)));
        }
    }
}
