using System;
using System.Collections.Generic;
using MassTransit;
using TechMastery.Messaging.Consumers;

namespace TechMastery.MarketPlace.Application.Contracts.Messaging
{
    public class BusControlConfigurator
    {
        private readonly MessagingSystemsOptions _messagingSystemsOptions;
        private readonly Dictionary<Func<MessagingSystemsOptions, bool>, Func<IBusControl>> _configStrategies;

        public BusControlConfigurator(MessagingSystemsOptions messagingSystemsOptions)
        {
            _messagingSystemsOptions = messagingSystemsOptions ?? throw new ArgumentNullException(nameof(messagingSystemsOptions));

            _configStrategies = new Dictionary<Func<MessagingSystemsOptions, bool>, Func<IBusControl>>
            {
                { opt => opt.EnableAzureServiceBus, ConfigureAzureServiceBus },
                { opt => opt.EnableSqs, ConfigureAmazonSqs },
                { opt => opt.EnableRabbitMq, ConfigureRabbitMq }
            };
        }

        public IBusControl ChooseBusConfiguration()
        {
            foreach (var strategy in _configStrategies)
            {
                if (strategy.Key(_messagingSystemsOptions))
                    return strategy.Value();
            }

            throw new InvalidOperationException("No valid messaging system configuration detected.");
        }

        private IBusControl ConfigureAzureServiceBus()
        {
            var connectionString = _messagingSystemsOptions.AzureServiceBus.ConnectionString;
            if (string.IsNullOrWhiteSpace(connectionString))
                throw new InvalidOperationException("Azure Service Bus connection string is missing or invalid.");

            return Bus.Factory.CreateUsingAzureServiceBus(cfg => cfg.Host(new Uri(connectionString)));
        }

        private IBusControl ConfigureAmazonSqs()
        {
            var region = _messagingSystemsOptions.Sqs.Region;
            var accessKey = _messagingSystemsOptions.Sqs.AccessKey;
            var secretKey = _messagingSystemsOptions.Sqs.SecretKey;

            if (string.IsNullOrWhiteSpace(region) || string.IsNullOrWhiteSpace(accessKey) || string.IsNullOrWhiteSpace(secretKey))
                throw new InvalidOperationException("Amazon SQS configuration is missing or invalid.");

            return Bus.Factory.CreateUsingAmazonSqs(cfg => cfg.Host(region, h =>
            {
                h.AccessKey(accessKey);
                h.SecretKey(secretKey);
            }));
        }

        private IBusControl ConfigureRabbitMq()
        {
            var host = _messagingSystemsOptions.RabbitMq.Host;
            var username = _messagingSystemsOptions.RabbitMq.Username;
            var password = _messagingSystemsOptions.RabbitMq.Password;

            if (string.IsNullOrWhiteSpace(host) || string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
                throw new InvalidOperationException("RabbitMQ configuration is missing or invalid.");

            return Bus.Factory.CreateUsingRabbitMq(cfg => cfg.Host(new Uri(host), h =>
            {
                h.Username(username);
                h.Password(password);
            }));
        }
    }
}
