using Amazon.S3;
using Amazon.S3.Model;
using TechMastery.MarketPlace.Application.Contracts.Infrastructure;
public class AwsS3StorageProvider : IStorageProvider
{
    private readonly IAmazonS3 _s3Client;
    private readonly string _bucketName = "productartifacts"; // Replace with your bucket name

    public AwsS3StorageProvider()
    {
        _s3Client = new AmazonS3Client(); 
    }

    public async Task CreateBlobFolderStructureAsync(string username, string artifactType)
    {
        string userFolderKey = $"{username}/";
        string artifactTypeFolderKey = $"{username}/{artifactType}/";

        await CreateFolderAsync(userFolderKey);
        await CreateFolderAsync(artifactTypeFolderKey);
    }

    private async Task CreateFolderAsync(string folderKey)
    {
        try
        {
            var putObjectRequest = new PutObjectRequest
            {
                BucketName = _bucketName,
                Key = folderKey,
                InputStream = new MemoryStream(), // Empty stream
            };

            await _s3Client.PutObjectAsync(putObjectRequest);
        }
        catch (Exception ex)
        {
            // Handle exceptions
            throw;
        }
    }
}
