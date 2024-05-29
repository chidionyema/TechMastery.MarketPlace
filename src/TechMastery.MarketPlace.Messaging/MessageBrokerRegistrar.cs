using Microsoft.Extensions.DependencyInjection;

namespace TechMastery.Messaging
{
    internal static class MessageBrokerRegistrar
    {
        public static void RegisterMessageBrokers(IServiceCollection services, MessagingSystemsOptions options, params Type[] consumerTypes)
        {
            if (options.EnableAzureServiceBus)
            {
                ValidateAzureServiceBusOptions(options.AzureServiceBus!);
                services.AddHealthChecks().AddCheck<AzureServiceBusHealthCheck>("AzureServiceBusHealthCheck");
                AzureServiceBusConfigurator.Configure(services, options.AzureServiceBus!, consumerTypes);
            }

            else if (options.EnableSqs)
            {
                ValidateSqsOptions(options.SqsOptions!);
                services.AddHealthChecks().AddCheck<AmazonSqsHealthCheck>("AmazonSqsHealthCheck");
                SqsConfigurator.Configure(services, options.SqsOptions!, consumerTypes);
            }

            else if (options.EnableRabbitMq)
            {
                ValidateRabbitMqOptions(options.RabbitMq!);
                services.AddHealthChecks().AddCheck<RabbitMqHealthCheck>("RabbitMqHealthCheck");
                RabbitMqConfigurator.Configure(services, options.RabbitMq!, consumerTypes);
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
}
