using TechMastery.MarketPlace.Application.Contracts.Infrastructure;

namespace TechMastery.MarketPlace.Application.Tests.Integration
{
    // Fake implementation of IBlobStorageService
    public class FakeBlobStorageService : IStorageProvider
    {
        public Func<string, Stream, CancellationToken, Task<string>> UploadFileAsyncFunc { get; set; }

        public Task CreateBlobFolderStructureAsync(string topLevel, string folderName)
        {
            throw new NotImplementedException();
        }

        public Task<byte[]> DownloadBlobAsync(string containerName, string blobName)
        {
            throw new NotImplementedException();
        }

        public Task<Stream> DownloadBlobStreamAsync(string containerName, string blobName)
        {
            throw new NotImplementedException();
        }

        public Task<Uri> GenerateSasDownloadUriAsync(string objectKey, DateTimeOffset expiryTime)
        {
            throw new NotImplementedException();
        }

        public Task<Uri> GenerateSasUploadUriAsync(string objectKey, DateTimeOffset expiryTime)
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

