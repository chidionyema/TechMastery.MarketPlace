using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Serilog;
using System;
using TechMastery.MarketPlace.Application.Contracts.Messaging;
using TechMastery.Messaging.Consumers.Consumers;

namespace TechMastery.Messaging.Consumers
{
    public static class MessagingServiceRegistration
    {
        public static IServiceCollection AddMessagingServices(this IServiceCollection services, IConfiguration configuration)
        {
            if (services == null) throw new ArgumentNullException(nameof(services));
            if (configuration == null) throw new ArgumentNullException(nameof(configuration));

            LoggingConfigurator.ConfigureLogging(services);

            services.Configure<MessagingSystemsOptions>(configuration.GetSection("MessagingSystems"));
            var serviceProvider = services.BuildServiceProvider();
            var messagingSystemsOptions = serviceProvider.GetRequiredService<IOptions<MessagingSystemsOptions>>().Value;

            MessageBrokerRegistrar.RegisterMessageBrokers(services, messagingSystemsOptions);

            services.AddSingleton(provider =>
            {
                var options = provider.GetRequiredService<MessagingSystemsOptions>();
                return new BusControlConfigurator(options);
            });

            services.AddSingleton<IMessagePublisher>(provider =>
            {
                var busControlConfigurator = provider.GetRequiredService<BusControlConfigurator>();
                var logger = provider.GetRequiredService<ILogger<MessagePublisher>>();
                return new MessagePublisher(busControlConfigurator, logger);
            });

            return services;
        }
    }

    static class LoggingConfigurator
    {
        public static void ConfigureLogging(IServiceCollection services)
        {
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .CreateLogger();

            services.AddLogging(loggingBuilder =>
            {
                loggingBuilder.ClearProviders();
                loggingBuilder.AddSerilog(dispose: true);
            });
        }
    }

    static class MessageBrokerRegistrar
    {
        public static void RegisterMessageBrokers(IServiceCollection services, MessagingSystemsOptions options)
        {
            if (options.EnableAzureServiceBus)
            {
                ValidateAzureServiceBusOptions(options.AzureServiceBus!);
                services.AddHealthChecks().AddCheck<AzureServiceBusHealthCheck>("AzureServiceBusHealthCheck");
                AzureServiceBusConfigurator.Configure(services, options.AzureServiceBus!);
            }

            if (options.EnableSqs)
            {
                ValidateSqsOptions(options.Sqs!);
                services.AddHealthChecks().AddCheck<AmazonSqsHealthCheck>("AmazonSqsHealthCheck");
                SqsConfigurator.Configure(services, options.Sqs!);
            }

            if (options.EnableRabbitMq)
            {
                ValidateRabbitMqOptions(options.RabbitMq!);
                services.AddHealthChecks().AddCheck<RabbitMqHealthCheck>("RabbitMqHealthCheck");
                RabbitMqConfigurator.Configure(services, options.RabbitMq!);
            }
        }

        private static void ValidateAzureServiceBusOptions(AzureServiceBusOptions options)
        {
            if (options == null || string.IsNullOrEmpty(options.QueueName) || string.IsNullOrEmpty(options.ConnectionString))
                throw new ArgumentException("Azure Service Bus configuration is incomplete.");
        }

        private static void ValidateSqsOptions(SqsOptions options)
        {
            if (options == null || string.IsNullOrEmpty(options.AccessKey) || string.IsNullOrEmpty(options.SecretKey))
                throw new ArgumentException("Amazon SQS configuration is incomplete.");
        }

        private static void ValidateRabbitMqOptions(RabbitMqOptions options)
        {
            if (options == null || string.IsNullOrEmpty(options.Host) || string.IsNullOrEmpty(options.Username) ||
                string.IsNullOrEmpty(options.Password) || string.IsNullOrEmpty(options.QueueName))
                throw new ArgumentException("RabbitMQ configuration is incomplete.");
        }
    }

    static class AzureServiceBusConfigurator
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

    static class SqsConfigurator
    {
        public static void Configure(IServiceCollection services, SqsOptions options)
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

                    ConsumerConfigurator.Configure(cfg, options.QueueUrl!);
                });
            });
        }
    }


    static class RabbitMqConfigurator
    {
        public static void Configure(IServiceCollection services, RabbitMqOptions options)
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

                    ConsumerConfigurator.Configure(cfg, options.QueueName!);
                });
            });
        }
    }

    static class ConsumerConfigurator
    {
        public static void Configure(IBusFactoryConfigurator cfg, string queueName)
        {
            cfg.ReceiveEndpoint(queueName, e =>
            {
                e.UseMessageRetry(r => r.Interval(3, TimeSpan.FromSeconds(5)));
                e.UseScheduledRedelivery(r => r.Intervals(TimeSpan.FromSeconds(30), TimeSpan.FromMinutes(1), TimeSpan.FromMinutes(5)));

                e.Consumer<ProductAddedConsumer>();
                e.Consumer<OrderPlacedConsumer>();
            });
        }
    }
}
