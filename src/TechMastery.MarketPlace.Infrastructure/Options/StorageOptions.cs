using System;
namespace TechMastery.MarketPlace.Infrastructure.Options
{
    public class StorageOptions
    {
        public S3Options S3 { get; set; }
        public AzureBlobStorageOptions AzureBlob { get; set; }
    }
}

