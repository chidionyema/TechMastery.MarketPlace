using Azure.Storage.Blobs;
using TechMastery.MarketPlace.Application.Contracts.Infrastructure;

public class AzureBlobStorageProvider : IStorageProvider
{
    private readonly BlobServiceClient _blobServiceClient;
    private readonly string _containerName = "productartifacts"; // Replace with your container name

    public AzureBlobStorageProvider()
    {
        _blobServiceClient = new BlobServiceClient(Environment.GetEnvironmentVariable("AzureWebJobsStorage")); // Use your connection string
    }

    public async Task CreateBlobFolderStructureAsync(string username, string artifactType)
    {
        BlobContainerClient containerClient = _blobServiceClient.GetBlobContainerClient(_containerName);

        string userFolderKey = $"{username}/";
        string artifactTypeFolderKey = $"{username}/{artifactType}/";

        await CreateFolderAsync(containerClient, userFolderKey);
        await CreateFolderAsync(containerClient, artifactTypeFolderKey);
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
