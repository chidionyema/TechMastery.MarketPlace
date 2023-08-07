namespace TechMastery.MarketPlace.Application.Contracts.Infrastructure
{
	public interface IBlobStorageService
	{
		Task<string> UploadFileAsync(string fileName, Stream fileStream, CancellationToken cancellationToken);
		Task<Uri> GenerateSasUriAsync(string blobName, DateTimeOffset expiryTime);
		Task<byte[]> DownloadBlobAsync(string containerName, string blobName);
        Task<Stream> DownloadBlobStreamAsync(string containerName, string blobName);
    }
}

