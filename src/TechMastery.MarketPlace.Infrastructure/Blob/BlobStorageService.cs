using System.IO;
using System;
using System.Threading;
using System.Threading.Tasks;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Specialized;
using Azure.Storage.Sas;
using Microsoft.Extensions.Configuration;
using TechMastery.MarketPlace.Application.Contracts.Infrastructure;
using TechMastery.MarketPlace.Application.Exceptions;

namespace TechMastery.MarketPlace.Infrastructure.Blob
{
    public class BlobStorageService : IBlobStorageService
    {
        private readonly BlobServiceClient _blobServiceClient;
        private readonly string _containerName;
        private readonly string _storageBaseUrl;

        public BlobStorageService(IConfiguration configuration)
        {
            if (configuration == null)
            {
                throw new ArgumentException("configuration is null.", nameof(configuration));
            }

            var storageConnectionString = configuration.GetConnectionString("BlobStorage");
            _containerName = configuration.GetConnectionString("BlobStorageContainerName");
            _storageBaseUrl = configuration.GetConnectionString("BlobStorageBaseUrl");

            if (string.IsNullOrWhiteSpace(storageConnectionString))
            {
                throw new ArgumentException("BlobStorage connection string is missing or empty.", nameof(configuration));
            }

            if (string.IsNullOrWhiteSpace(_containerName))
            {
                throw new ArgumentException("BlobStorageContainerName is missing or empty.", nameof(configuration));
            }

            if (string.IsNullOrWhiteSpace(_storageBaseUrl))
            {
                throw new ArgumentException("BlobStorageBaseUrl is missing or empty.", nameof(configuration));
            }

            _blobServiceClient = new BlobServiceClient(storageConnectionString);
        }


        public async Task<string> UploadFileAsync(string fileName, Stream fileStream, CancellationToken cancellationToken)
        {
            ValidateFileName(fileName);

            var containerClient = await GetBlobContainerClientAsync();
            var blobName = GenerateUniqueBlobName(fileName);

            var blobClient = containerClient.GetBlobClient(blobName);
            await blobClient.UploadAsync(fileStream, cancellationToken);

            return blobClient.Name;
        }

        public async Task<Uri> GenerateSasUriAsync(string blobName, DateTimeOffset expiryTime)
        {
            var containerClient = await GetBlobContainerClientAsync();
            var blobClient = containerClient.GetBlobClient(blobName);
            EnsureBlobClientAuthorization(blobClient);

            var sasBuilder = BuildBlobSasBuilder(containerClient.Name, blobName, expiryTime, BlobSasPermissions.Read);
            var sasUri = blobClient.GenerateSasUri(sasBuilder);

            return sasUri;
        }

        public async Task<byte[]> DownloadBlobAsync(string containerName, string blobName)
        {
            var containerClient = _blobServiceClient.GetBlobContainerClient(containerName);
            var blobClient = containerClient.GetBlobClient(blobName);

            var response = await blobClient.DownloadAsync();

            using (var memoryStream = new MemoryStream())
            {
                await response.Value.Content.CopyToAsync(memoryStream);
                return memoryStream.ToArray();
            }
        }

        public async Task<Stream> DownloadBlobStreamAsync(string containerName, string blobName)
        {
            var containerClient = _blobServiceClient.GetBlobContainerClient(containerName);
            var blobClient = containerClient.GetBlobClient(blobName);

            var response = await blobClient.OpenReadAsync();
            return response;
        }

        private static void ValidateFileName(string fileName)
        {
            if (string.IsNullOrEmpty(fileName))
            {
                throw new ArgumentException("File name cannot be empty.", nameof(fileName));
            }
        }

        private async Task<BlobContainerClient> GetBlobContainerClientAsync()
        {
            var containerClient = _blobServiceClient.GetBlobContainerClient(_containerName);
            await containerClient.CreateIfNotExistsAsync();

            return containerClient;
        }

        private static string GenerateUniqueBlobName(string fileName)
        {
            return Guid.NewGuid().ToString("N") + Path.GetExtension(fileName);
        }

        private static void EnsureBlobClientAuthorization(BlobClient blobClient)
        {
            if (!blobClient.CanGenerateSasUri)
            {
                throw new InvalidOperationException("Blob client is not authorized via Shared Key.");
            }
        }

        private static BlobSasBuilder BuildBlobSasBuilder(string containerName, string blobName, DateTimeOffset expiryTime, BlobSasPermissions permissions)
        {
            var sasBuilder = new BlobSasBuilder
            {
                BlobContainerName = containerName,
                BlobName = blobName,
                Resource = "b",
                ExpiresOn = expiryTime
            };

            sasBuilder.SetPermissions(permissions);
            return sasBuilder;
        }
    }
}
