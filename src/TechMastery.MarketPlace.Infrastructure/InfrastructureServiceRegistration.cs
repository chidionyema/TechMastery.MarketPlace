using TechMastery.MarketPlace.Application.Contracts.Infrastructure;
using TechMastery.MarketPlace.Application.Models.Mail;
using TechMastery.MarketPlace.Infrastructure.Mail;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TechMastery.MarketPlace.Infrastructure.Blob;
using Nest;
using MassTransit;

namespace TechMastery.MarketPlace.Infrastructure
{
    public static class InfrastructureServiceRegistration
    {
        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
        {
            
            services.Configure<EmailSettings>(configuration.GetSection("EmailSettings"));

             Uri elasticsearchUri = new Uri(configuration["Elasticsearch:Uri"]!);
            var settings = new ConnectionSettings(elasticsearchUri)
                .DefaultIndex(configuration["Elasticsearch:Index"]);
            var elasticClient = new ElasticClient(settings);
            services.AddSingleton(elasticClient);
            services.AddTransient<IEmailService, EmailService>();
            services.AddTransient<IPaymentService>(sp =>
            {
                var stripeSecretKey = configuration["Stripe:SecretKey"]; // Replace "Stripe:SecretKey" with your configuration key
                return new StripePaymentService(stripeSecretKey);
            });

            return services;
        }

        public static IServiceCollection AddStorageProvider(this IServiceCollection services, StorageProviderType providerType)
        {
            switch (providerType)
            {
                case StorageProviderType.AwsS3:
                    services.AddSingleton<IStorageProvider, S3StorageProvider>();
                    break;
                case StorageProviderType.AzureBlobStorage:
                    services.AddSingleton<IStorageProvider, AzureBlobStorageProvider>();
                    break;
                case StorageProviderType.Both:
                    services.AddSingleton<IStorageProvider, S3StorageProvider>();
                    services.AddSingleton<IStorageProvider, AzureBlobStorageProvider>();
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(providerType));
            }

            return services;
        }
    }

    public enum StorageProviderType
    {
        AwsS3,
        AzureBlobStorage,
        Both
    }
}

