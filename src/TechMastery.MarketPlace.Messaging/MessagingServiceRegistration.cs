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

            // remove once options caching issue resolved
            messagingSystemsOptions.EnableAzureServiceBus = false;
            // Add MassTransit with Azure Service Bus if enabled
            if (messagingSystemsOptions.EnableAzureServiceBus)
            {
                services.AddHealthChecks().AddCheck<AzureServiceBusHealthCheck>("AzureServiceBusHealthCheck");
                services.AddMassTransitWithAzureServiceBus(configuration.GetSection("MessagingSystems:AzureServiceBus"));
            }

            if (messagingSystemsOptions.EnableSqs)
            {
                services.AddHealthChecks().AddCheck<AmazonSqsHealthCheck>("AmazonSqsHealthCheck");
                services.AddMassTransitWithSqs(configuration.GetSection("MessagingSystems:SQS"));
            }

            if (messagingSystemsOptions.EnableRabbitMq)
            {
                services.AddHealthChecks().AddCheck<RabbitMqHealthCheck>("RabbitMqHealthCheck");
                services.AddMassTransitWithRabbitMq(configuration.GetSection("MessagingSystems:RabbitMQ"));
            }

            // Register BusControlConfigurator as a singleton
            services.AddSingleton<BusControlConfigurator>(provider =>
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
                        ConfigureReceiveEndpoint(e);

                        // Register consumers
                        e.Consumer<ProductAddedConsumer>();
                        e.Consumer<OrderPlacedConsumer>();
                    });
                });
            });
            
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
                        ConfigureReceiveEndpoint(e);

                        // Register consumers
                        e.Consumer<ProductAddedConsumer>();
                        e.Consumer<OrderPlacedConsumer>();
                    });
                });
            });

            
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
                        ConfigureReceiveEndpoint(e);

                        // Register consumers
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
