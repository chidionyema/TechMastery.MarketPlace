using Microsoft.Extensions.Configuration;
using System;

namespace TechMastery.MarketPlace.Infrastructure.Tests
{
    public class StripeTestContext
    {
        public string StripeSecretKey { get; }

        public StripeTestContext()
        {
            var configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json") // Replace with your configuration file
                .Build();

            StripeSecretKey =  configuration.GetConnectionString("Stripe:SecretKey")
                             ?? throw new InvalidOperationException("StripeSecretKey is missing or empty in the configuration.");

            // Use a guard clause to check for invalid configuration
            if (string.IsNullOrWhiteSpace(StripeSecretKey))
            {
                throw new InvalidOperationException("StripeSecretKey is missing or empty in the configuration.");
            }
        }
    }
    
        [CollectionDefinition("StripeTestCollection")]
        public class StripeTestCollection : ICollectionFixture<StripeTestContext>
        {
        }

}
