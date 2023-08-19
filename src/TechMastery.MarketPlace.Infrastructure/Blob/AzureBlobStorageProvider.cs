using Azure.Storage.Blobs;
using Azure.Storage.Sas;
using Microsoft.Extensions.Configuration;
using TechMastery.MarketPlace.Application.Contracts.Infrastructure;

namespace TechMastery.MarketPlace.Infrastructure.Blob
{
    public class AzureBlobStorageProvider : IStorageProvider
    {
        private readonly BlobServiceClient _blobServiceClient;
        private readonly string _containerName;

        public AzureBlobStorageProvider(string storageConnectionString, string containerName)
        {

            if (string.IsNullOrWhiteSpace(storageConnectionString))
            {
                throw new ArgumentException("Storage connection string cannot be empty or null.", nameof(storageConnectionString));
            }

            if (string.IsNullOrWhiteSpace(containerName))
            {
                throw new ArgumentException("Container name cannot be empty or null.", nameof(containerName));
            }

            _blobServiceClient = new BlobServiceClient(storageConnectionString);
            _containerName = containerName;
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

        public async Task<Uri> GenerateSasDownloadUriAsync(string blobName, DateTimeOffset expiryTime)
        {
            ValidateBlobName(blobName);

            var containerClient = await GetBlobContainerClientAsync();
            var blobClient = containerClient.GetBlobClient(blobName);
            EnsureBlobClientAuthorization(blobClient);

            var sasBuilder = BuildBlobSasBuilder(containerClient.Name, blobName, expiryTime, BlobSasPermissions.Read);
            var sasUri = blobClient.GenerateSasUri(sasBuilder);

            return sasUri;
        }

        public async Task<Uri> GenerateSasUploadUriAsync(string blobName, DateTimeOffset expiryTime)
        {
            ValidateBlobName(blobName);

            var containerClient = await GetBlobContainerClientAsync();
            var blobClient = containerClient.GetBlobClient(blobName);
            EnsureBlobClientAuthorization(blobClient);

            var sasBuilder = BuildBlobSasBuilder(containerClient.Name, blobName, expiryTime, BlobSasPermissions.Write);
            var sasUri = blobClient.GenerateSasUri(sasBuilder);

            return sasUri;
        }
        public async Task<byte[]> DownloadBlobAsync(string containerName, string blobName)
        {
            ValidateContainerAndBlobNames(containerName, blobName);

            var containerClient = _blobServiceClient.GetBlobContainerClient(containerName);
            var blobClient = containerClient.GetBlobClient(blobName);

            var response = await blobClient.DownloadAsync();

            using (var memoryStream = new MemoryStream())
            {
                await response.Value.Content.CopyToAsync(memoryStream);
                return memoryStream.ToArray();
            }
        }

        public async Task CreateBlobFolderStructureAsync(string username, string artifactType)
        {
            ValidateFolderName(username);
            ValidateFolderName(artifactType);

            BlobContainerClient containerClient = _blobServiceClient.GetBlobContainerClient(_containerName);

            string userFolderKey = $"{username}/";
            string artifactTypeFolderKey = $"{username}/{artifactType}/";

            await CreateFolderAsync(containerClient, userFolderKey);
            await CreateFolderAsync(containerClient, artifactTypeFolderKey);
        }

        public async Task<Stream> DownloadBlobStreamAsync(string containerName, string blobName)
        {
            ValidateContainerAndBlobNames(containerName, blobName);

            var containerClient = _blobServiceClient.GetBlobContainerClient(containerName);
            var blobClient = containerClient.GetBlobClient(blobName);

            var response = await blobClient.OpenReadAsync();
            return response;
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

        private static void ValidateFileName(string fileName)
        {
            if (string.IsNullOrEmpty(fileName))
            {
                throw new ArgumentException("File name cannot be empty.", nameof(fileName));
            }
        }

        private static void ValidateBlobName(string blobName)
        {
            if (string.IsNullOrEmpty(blobName))
            {
                throw new ArgumentException("Blob name cannot be empty.", nameof(blobName));
            }
        }

        private static void ValidateContainerAndBlobNames(string containerName, string blobName)
        {
            if (string.IsNullOrEmpty(containerName) || string.IsNullOrEmpty(blobName))
            {
                throw new ArgumentException("Container name and blob name cannot be empty.", nameof(containerName));
            }
        }

        private static void ValidateFolderName(string folderName)
        {
            if (string.IsNullOrEmpty(folderName))
            {
                throw new ArgumentException("Folder name cannot be empty.", nameof(folderName));
            }
        }

        private static string GetConfigurationValue(IConfiguration configuration, string key)
        {
            var value = configuration.GetConnectionString(key);
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new ArgumentException($"{key} is missing or empty in the configuration.", nameof(configuration));
            }

            return value;
        }

        private async Task CreateFolderAsync(BlobContainerClient containerClient, string folderKey)
        {
            try
            {
                BlobClient blobClient = containerClient.GetBlobClient(folderKey);
                if (!await blobClient.ExistsAsync())
                {
                    await blobClient.UploadAsync(new MemoryStream(), overwrite: true);
                }
            }
            catch (Exception ex)
            {
                // Handle exceptions
                throw;
            }
        }
    }
}
