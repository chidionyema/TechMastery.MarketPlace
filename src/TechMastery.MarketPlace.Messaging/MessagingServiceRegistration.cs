﻿using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using TechMastery.MarketPlace.Application.Contracts.Messaging;

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
}
