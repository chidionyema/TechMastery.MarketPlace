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
using Microsoft.Extensions.Options;
using Stripe.Tax;
using Stripe;

namespace TechMastery.MarketPlace.Infrastructure
{
    public static class InfrastructureServiceRegistration
    {
        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<EmailOptions>(configuration.GetSection("EmailSettings"));

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

                // Register services
                services.AddTransient<IEmailService, EmailService>();

                // Validate and configure Stripe secret key
                var stripeSecretKey = configuration["Stripe:SecretKey"];
                if (string.IsNullOrWhiteSpace(stripeSecretKey))
                {
                    throw new InvalidOperationException("Stripe:SecretKey is missing or empty in configuration.");
                }

                return services;
            }
            catch (Exception ex)
            {
                Log.Logger.Error(ex, "Error occurred during service registration.");
                throw;
            }
        }

        public static IServiceCollection AddStripePaymentService(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<StripeOptions>(configuration.GetSection("Stripe"));

            services.AddSingleton<IPaymentService>(provider =>
            {
                var stripeOptions = provider.GetRequiredService<IOptions<StripeOptions>>().Value;

                if (string.IsNullOrEmpty(stripeOptions.SecretKey))
                {
                    throw new ArgumentNullException(nameof(stripeOptions.SecretKey), "StripeSecretKey is missing or null.");
                }

                var logger = provider.GetRequiredService<ILogger<StripePaymentService>>();
                StripeConfiguration.ApiKey = stripeOptions.SecretKey;

                return new StripePaymentService(stripeOptions.SecretKey, logger);
            });

            return services;
        }
        public static IServiceCollection AddStorageProvider(this IServiceCollection services, IConfiguration configuration, StorageProviderType providerType)
        {
            services.Configure<StorageOptions>(configuration.GetSection("BlobStorage"));

            services.AddSingleton<IStorageProvider>(provider =>
            {
                var storageOptions = provider.GetRequiredService<IOptions<StorageOptions>>().Value;

                switch (providerType)
                {
                    case StorageProviderType.AwsS3:
                        var s3Options = storageOptions.S3;
                        return new S3StorageProvider(
                            s3Options.AccessKey,
                            s3Options.SecretKey,
                            s3Options.Region,
                            s3Options.BucketName
                        );
                    case StorageProviderType.AzureBlobStorage:
                        var azureBlobOptions = storageOptions.AzureBlob;
                        return new AzureBlobStorageProvider(azureBlobOptions.ConnectionString, azureBlobOptions.ContainerName);
                    case StorageProviderType.Both:
                        var s3OptionsBoth = storageOptions.S3;
                        var azureBlobOptionsBoth = storageOptions.AzureBlob;
                        return new S3StorageProvider(
                            s3OptionsBoth.AccessKey,
                            s3OptionsBoth.SecretKey,
                            s3OptionsBoth.Region,
                            s3OptionsBoth.BucketName
                        );
                    // You can also add AzureBlobStorageProvider here if needed
                    default:
                        throw new ArgumentOutOfRangeException(nameof(providerType));
                }
            });

            return services;
        }


        public static IServiceCollection AddElasticsearchClient(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<ElasticsearchOptions>(configuration.GetSection("Elasticsearch"));

            services.AddSingleton<IElasticClient>(provider =>
            {
                var elasticsearchOptions = provider.GetRequiredService<IOptions<ElasticsearchOptions>>().Value;

                if (string.IsNullOrEmpty(elasticsearchOptions.Uri))
                {
                    Log.Logger.Error("Elasticsearch: Uri is missing or empty.");
                    throw new ArgumentException("Elasticsearch: Uri is missing or empty.", nameof(elasticsearchOptions.Uri));
                }
                if (string.IsNullOrEmpty(elasticsearchOptions.Index))
                {
                    Log.Logger.Error("Elasticsearch: Index is missing or empty.");
                    throw new ArgumentException("Elasticsearch: Index is missing or empty.", nameof(elasticsearchOptions.Index));
                }

                Uri elasticsearchUri = new Uri(elasticsearchOptions.Uri);
                var elasticClient = new ElasticClient(new ConnectionSettings(elasticsearchUri).DefaultIndex(elasticsearchOptions.Index));

                // Add health checks
                services.AddHealthChecks()
                    .AddElasticsearch(elasticsearchUri.ToString());
                return elasticClient;

            });


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

