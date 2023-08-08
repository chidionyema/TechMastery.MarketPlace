namespace TechMastery.MarketPlace.Application.Contracts.Infrastructure
{
    public interface IStorageProvider
    {
        Task<string> UploadFileAsync(string fileName, Stream fileStream, CancellationToken cancellationToken);
        Task<Uri> GenerateSasUriAsync(string objectKey, DateTimeOffset expiryTime);
        Task<byte[]> DownloadBlobAsync(string containerName, string blobName);
        Task<Stream> DownloadBlobStreamAsync(string containerName, string blobName);
        Task CreateBlobFolderStructureAsync(string topLevel, string folderName);
    }
}
