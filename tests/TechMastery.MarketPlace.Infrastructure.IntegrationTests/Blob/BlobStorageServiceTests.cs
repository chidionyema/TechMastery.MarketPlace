﻿using TechMastery.MarketPlace.Infrastructure.Blob;
using TechMastery.MarketPlace.Infrastructure.Options;
using Microsoft.Extensions.Options;


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

            // Get the Blob Emulator connection information from the fixture
            var connectionString = _fixture.GetBlobEmulatorConnectionString();

            // Create AzureBlobStorageOptions configuration
            var options = new AzureBlobStorageOptions
            {
                ConnectionString = connectionString,
                ContainerName = "test"
            };

            // Create the storage service using the options
            _blobStorageService = new AzureBlobStorageProvider(options.ConnectionString, options.ContainerName);
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

