using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Serilog;
using TechMastery.MarketPlace.Application.Contracts.Infrastructure;
using TechMastery.MarketPlace.Infrastructure.Mail;
using Nest;
using Microsoft.Extensions.Logging;
using TechMastery.MarketPlace.Infrastructure.Payment;
using TechMastery.MarketPlace.Infrastructure.Blob;
using TechMastery.MarketPlace.Application.Models.Mail;
using TechMastery.MarketPlace.Infrastructure.Options;

namespace TechMastery.MarketPlace.Infrastructure
{
    public static class InfrastructureServiceRegistration
    {
        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
        {
            // Configure centralized logging
            Log.Logger = new LoggerConfiguration()
                .CreateLogger();

            services.AddLogging(loggingBuilder =>
            {
                loggingBuilder.AddSerilog(dispose: true);
            });

            try
            {
                // Configure and validate email settings
                var emailSettings = new EmailSettings();
                configuration.GetSection("EmailSettings").Bind(emailSettings);
                if (string.IsNullOrEmpty(emailSettings.ApiKey))
                {
                    Log.Logger.Error("EmailSettings: SmtpServer is missing or empty.");
                    throw new ArgumentException("EmailSettings: SmtpServer is missing or empty.", nameof(emailSettings.ApiKey));
                }
                services.AddSingleton(emailSettings);

                // Configure and validate Elasticsearch settings
                var elasticsearchSettings = new ElasticsearchSettings();
                configuration.GetSection("Elasticsearch").Bind(elasticsearchSettings);
                if (string.IsNullOrEmpty(elasticsearchSettings.Uri))
                {
                    Log.Logger.Error("Elasticsearch: Uri is missing or empty.");
                    throw new ArgumentException("Elasticsearch: Uri is missing or empty.", nameof(elasticsearchSettings.Uri));
                }
                if (string.IsNullOrEmpty(elasticsearchSettings.Index))
                {
                    Log.Logger.Error("Elasticsearch: Index is missing or empty.");
                    throw new ArgumentException("Elasticsearch: Index is missing or empty.", nameof(elasticsearchSettings.Index));
                }
                Uri elasticsearchUri = new Uri(elasticsearchSettings.Uri);
                var elasticClient = new ElasticClient(new ConnectionSettings(elasticsearchUri).DefaultIndex(elasticsearchSettings.Index));
                services.AddSingleton(elasticClient);

                // Register services
                services.AddTransient<IEmailService, EmailService>();

                // Validate and configure Stripe secret key
                var stripeSecretKey = configuration["Stripe:SecretKey"];
                if (string.IsNullOrWhiteSpace(stripeSecretKey))
                {
                    throw new InvalidOperationException("Stripe:SecretKey is missing or empty in configuration.");
                }

                // Register StripePaymentService with defensive checks
                services.AddTransient(sp =>
                {
                    var logger = sp.GetRequiredService<ILogger<StripePaymentService>>();
                    return new StripePaymentService(stripeSecretKey, logger);
                });


                // Add health checks
                services.AddHealthChecks()
                    .AddElasticsearch(elasticsearchSettings.Uri);

                return services;
            }
            catch (Exception ex)
            {
                Log.Logger.Error(ex, "Error occurred during service registration.");
                throw;
            }
        }

        public static IServiceCollection AddStorageProvider(this IServiceCollection services, IConfiguration configuration, StorageProviderType providerType)
        {
            switch (providerType)
            {
                case StorageProviderType.AwsS3:
                    services.Configure<S3Options>(configuration.GetSection("BlobStorage:S3"));
                    services.AddSingleton<IStorageProvider, S3StorageProvider>();
                    break;
                case StorageProviderType.AzureBlobStorage:
                    services.Configure<AzureBlobStorageOptions>(configuration.GetSection("BlobStorage:AzureBlob"));
                    services.AddSingleton<IStorageProvider, AzureBlobStorageProvider>();
                    break;
                case StorageProviderType.Both:
                    services.Configure<S3Options>(configuration.GetSection("BlobStorage:S3"));
                    services.AddSingleton<IStorageProvider, S3StorageProvider>();
                    services.Configure<AzureBlobStorageOptions>(configuration.GetSection("BlobStorage:AzureBlob"));
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

