using Microsoft.Extensions.Configuration;
using TechMastery.MarketPlace.Application.Contracts.Infrastructure;
using TechMastery.MarketPlace.Infrastructure.Blob;

namespace TechMastery.MarketPlace.Infrastructure.IntegrationTests
{
    [Collection("BlobEmulatorCollection")]
    public class BlobStorageServiceTests
    {
        private readonly BlobEmulatorFixture _fixture;
        private readonly AzureBlobStorageProvider _blobStorageService;

        public BlobStorageServiceTests(BlobEmulatorFixture fixture)
        {
            _fixture = fixture;
            var configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.test.json")
                .Build();

            _blobStorageService = new AzureBlobStorageProvider(configuration);
        }

        [Fact]
        public async Task TestBlobUploadAndDownload()
        {
            // Arrange
            var containerName = "test";
            var blobName = "sample.txt"; // Change to your blob name
            var content = "This is a sample text content.";

            // Act
            using (var stream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(content)))
            {
                var name = await _blobStorageService.UploadFileAsync(blobName, stream, CancellationToken.None);

                using (var downloadedStream = await _blobStorageService.DownloadBlobStreamAsync(containerName, name))
                {
                    // Assert
                    Assert.NotNull(downloadedStream);
                    Assert.True(downloadedStream.Length > 0);

                    using (var reader = new StreamReader(downloadedStream))
                    {
                        var downloadedContent = await reader.ReadToEndAsync();
                        Assert.Equal(content, downloadedContent);
                    }
                }
            }
        }

        [Fact]
        public async Task TestUploadZipFileAndDownload()
        {
            var containerName = "test";
            var blobName = "sample.zip"; // Change to your zip file name
            var filePath = "test.zip"; // Change to the actual path of your zip file

            // Read the zip file content as a stream
            using (var stream = File.OpenRead(filePath))
            {
                var uri = await _blobStorageService.UploadFileAsync(blobName, stream, CancellationToken.None);

                using (var downloadedStream = await _blobStorageService.DownloadBlobStreamAsync(containerName, uri.ToString()))
                {
                    // Assert that the downloaded stream has content
                    Assert.True(downloadedStream.Length > 0);
                }
            }
        }
    }

}

