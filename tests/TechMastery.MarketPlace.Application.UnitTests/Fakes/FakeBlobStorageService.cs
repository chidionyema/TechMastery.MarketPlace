using TechMastery.MarketPlace.Application.Contracts.Infrastructure;

namespace TechMastery.MarketPlace.Application.Tests.Integration
{
    // Fake implementation of IBlobStorageService
    public class FakeBlobStorageService : IBlobStorageService
    {
        public Func<string, Stream, CancellationToken, Task<string>> UploadFileAsyncFunc { get; set; }

        public Task<byte[]> DownloadBlobAsync(string containerName, string blobName)
        {
            throw new NotImplementedException();
        }

        public Task<Stream> DownloadBlobStreamAsync(string containerName, string blobName)
        {
            throw new NotImplementedException();
        }

        public Task<Uri> GenerateSasUriAsync(string blobName, DateTimeOffset expiryTime)
        {
            throw new NotImplementedException();
        }

        public Task<string> UploadFileAsync(string blobName, Stream stream, CancellationToken cancellationToken)
        {
            return UploadFileAsyncFunc?.Invoke(blobName, stream, cancellationToken);
        }
    }
}

