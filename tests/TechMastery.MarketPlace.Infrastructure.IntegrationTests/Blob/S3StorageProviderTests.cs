using System;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading;
using Amazon.S3.Model;
using TechMastery.MarketPlace.Infrastructure.Blob;
using TechMastery.MarketPlace.Infrastructure.Options;
using TechMastery.MarketPlace.Tests.Emulators;

namespace TechMastery.MarketPlace.Infrastructure.IntegrationTests
{
    [Collection("MockS3Collection")]
    public class S3StorageProviderTests : IAsyncLifetime
    {
        private readonly MockS3Fixture _fixture;
        private readonly S3StorageProvider _provider;
        private const string _bucketName = "testbucket"; 

        public S3StorageProviderTests(MockS3Fixture fixture)
        {
            _fixture = fixture;
            var options = new S3Options
            {
                AccessKey = "test",
                SecretKey = "test",
                BucketName = "your-chosen-bucket-name",
                Region = "us-east-1" // LocalStack defaults to us-east-1
            };

            _provider = new S3StorageProvider(options.AccessKey, options.SecretKey, options.Region, _bucketName);
        }

        public async Task InitializeAsync()
        {
            //await EnsureBucketExistsAsync();
        }

        public Task DisposeAsync()
        {
            return Task.CompletedTask; // If you have cleanup to do after tests, you'd do it here.
        }


        [Fact]
        public async Task UploadFileAsync_WithValidInput_ShouldReturnObjectKey()
        {
            var stream = new MemoryStream(Encoding.UTF8.GetBytes("test content"));
            var objectKey = await _provider.UploadFileAsync("test.txt", stream, CancellationToken.None);
            Assert.NotNull(objectKey);
        }

        [Fact]
        public async Task DownloadBlobAsync_ShouldReturnByteArray()
        {
            var expectedContent = "test content";
            var stream = new MemoryStream(Encoding.UTF8.GetBytes(expectedContent));
            var objectKey = await _provider.UploadFileAsync("test.txt", stream, CancellationToken.None);
            var result = await _provider.DownloadBlobAsync("testcontainer", objectKey);
            Assert.Equal(expectedContent, Encoding.UTF8.GetString(result));
        }

        [Fact]
        public async Task DownloadBlobStreamAsync_ShouldReturnStream()
        {
            var expectedContent = "test content";
            var stream = new MemoryStream(Encoding.UTF8.GetBytes(expectedContent));
            var objectKey = await _provider.UploadFileAsync("test.txt", stream, CancellationToken.None);
            using var resultStream = await _provider.DownloadBlobStreamAsync("testcontainer", objectKey);
            using var reader = new StreamReader(resultStream);
            var result = reader.ReadToEnd();
            Assert.Equal(expectedContent, result);
        }

        [Fact]
        public async Task GenerateSasDownloadUriAsync_ShouldReturnValidUri()
        {
            var stream = new MemoryStream(Encoding.UTF8.GetBytes("test content"));
            var objectKey = await _provider.UploadFileAsync("test.txt", stream, CancellationToken.None);
            var expiryTime = DateTimeOffset.Now.AddHours(1);
            var sasUri = await _provider.GenerateSasDownloadUriAsync(objectKey, expiryTime);
            Assert.NotNull(sasUri);

            // Validate the SAS URL by performing a GET request
            using (var httpClient = new HttpClient())
            {
                var response = await httpClient.GetAsync(sasUri);
                response.EnsureSuccessStatusCode();
            }
        }

        /*
        [Fact]
        public async Task CreateBlobFolderStructureAsync_ShouldCreateFolders()
        {
            await _provider.CreateBlobFolderStructureAsync("topFolder", "subFolder");

            // Check that the folders actually exist
            var listObjectsResponse = await _fixture.GetS3Client().ListObjectsV2Async(new ListObjectsV2Request { BucketName = _bucketName, Prefix = "topFolder/" });
            Assert.Contains(listObjectsResponse.S3Objects, s3Object => s3Object.Key == "topFolder/");
            Assert.Contains(listObjectsResponse.S3Objects, s3Object => s3Object.Key == "topFolder/subFolder/");
        }*/
    }
}
