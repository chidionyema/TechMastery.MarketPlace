using Amazon;
using Amazon.S3;
using Amazon.S3.Model;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using TechMastery.MarketPlace.Application.Contracts.Infrastructure;
using TechMastery.MarketPlace.Infrastructure.Options;

namespace TechMastery.MarketPlace.Infrastructure.Blob
{
    public class S3StorageProvider : IStorageProvider
    {
        private readonly IAmazonS3 _s3Client;
        private readonly string _bucketName;

        public S3StorageProvider(string accessKey, string secretKey, string region, string bucketName)
        {
            if (string.IsNullOrWhiteSpace(accessKey))
            {
                throw new ArgumentException("AWS S3 access key cannot be empty or null.", nameof(accessKey));
            }

            if (string.IsNullOrWhiteSpace(secretKey))
            {
                throw new ArgumentException("AWS S3 secret key cannot be empty or null.", nameof(secretKey));
            }

            if (string.IsNullOrWhiteSpace(region))
            {
                throw new ArgumentException("AWS S3 region cannot be empty or null.", nameof(region));
            }

            if (string.IsNullOrWhiteSpace(bucketName))
            {
                throw new ArgumentException("AWS S3 bucket name cannot be empty or null.", nameof(bucketName));
            }

            _s3Client = new AmazonS3Client(accessKey, secretKey, RegionEndpoint.GetBySystemName(region));
            _bucketName = bucketName;
        }

        public async Task<string> UploadFileAsync(string fileName, Stream fileStream, CancellationToken cancellationToken)
        {
            ValidateFileName(fileName);

            var objectKey = GenerateUniqueObjectKey(fileName);

            var request = new PutObjectRequest
            {
                BucketName = _bucketName,
                Key = objectKey,
                InputStream = fileStream,
                CannedACL = S3CannedACL.PublicRead // Optional: Set ACL permissions
            };

            try
            {
                await _s3Client.PutObjectAsync(request, cancellationToken);
            }
            catch (AmazonS3Exception ex)
            {
                // Handle S3-specific exceptions (e.g., access denied, bucket not found)
                throw new Exception("Error occurred while uploading file to S3.", ex);
            }
            catch (Exception ex)
            {
                // Handle other unexpected exceptions
                throw new Exception("An unexpected error occurred during file upload.", ex);
            }

            return objectKey;
        }

        private static string GenerateUniqueObjectKey(string fileName)
        {
            if (string.IsNullOrEmpty(fileName))
            {
                throw new ArgumentException("File name cannot be empty.", nameof(fileName));
            }

            // Combine a unique Guid and the file extension to create a unique object key
            var uniqueKey = Guid.NewGuid().ToString("N");
            var fileExtension = Path.GetExtension(fileName);
            var objectKey = $"{uniqueKey}{fileExtension}";

            return objectKey;
        }


        public async Task<Uri> GenerateSasUriAsync(string objectKey, DateTimeOffset expiryTime)
        {
            ValidateObjectKey(objectKey);

            var request = new GetPreSignedUrlRequest
            {
                BucketName = _bucketName,
                Key = objectKey,
                Expires = expiryTime.UtcDateTime
            };

            try
            {
                var preSignedUrl = _s3Client.GetPreSignedURL(request);
                return new Uri(preSignedUrl);
            }
            catch (AmazonS3Exception ex)
            {
                // Handle S3-specific exceptions (e.g., object not found, bucket not found)
                throw new Exception("Error occurred while generating the SAS URI.", ex);
            }
            catch (Exception ex)
            {
                // Handle other unexpected exceptions
                throw new Exception("An unexpected error occurred while generating the SAS URI.", ex);
            }
        }

        public async Task CreateBlobFolderStructureAsync(string topLevel, string folderName)
        {
            ValidateFolderName(topLevel);
            ValidateFolderName(folderName);

            var userFolderKey = $"{topLevel}/";
            var artifactTypeFolderKey = $"{topLevel}/{folderName}/";

            try
            {
                await CreateFolderAsync(userFolderKey);
                await CreateFolderAsync(artifactTypeFolderKey);
            }
            catch (AmazonS3Exception ex)
            {
                // Handle S3-specific exceptions
                throw new Exception("Error occurred while creating folder structure in S3.", ex);
            }
            catch (Exception ex)
            {
                // Handle other unexpected exceptions
                throw new Exception("An unexpected error occurred while creating folder structure.", ex);
            }
        }

        public async Task<byte[]> DownloadBlobAsync(string containerName, string blobName)
        {
            ValidateContainerAndBlobNames(containerName, blobName);

            var request = new GetObjectRequest
            {
                BucketName = _bucketName,
                Key = $"{containerName}/{blobName}"
            };

            try
            {
                using (var response = await _s3Client.GetObjectAsync(request))
                using (var responseStream = response.ResponseStream)
                using (var memoryStream = new MemoryStream())
                {
                    await responseStream.CopyToAsync(memoryStream);
                    return memoryStream.ToArray();
                }
            }
            catch (AmazonS3Exception ex)
            {
                // Handle S3-specific exceptions (e.g., object not found, access denied)
                throw new Exception("Error occurred while downloading the blob from S3.", ex);
            }
            catch (Exception ex)
            {
                // Handle other unexpected exceptions
                throw new Exception("An unexpected error occurred during blob download.", ex);
            }
        }

        public async Task<Stream> DownloadBlobStreamAsync(string containerName, string blobName)
        {
            ValidateContainerAndBlobNames(containerName, blobName);

            var request = new GetObjectRequest
            {
                BucketName = _bucketName,
                Key = $"{containerName}/{blobName}"
            };

            try
            {
                var response = await _s3Client.GetObjectAsync(request);
                return response.ResponseStream;
            }
            catch (AmazonS3Exception ex)
            {
                // Handle S3-specific exceptions (e.g., object not found, access denied)
                throw new Exception("Error occurred while downloading the blob stream from S3.", ex);
            }
            catch (Exception ex)
            {
                // Handle other unexpected exceptions
                throw new Exception("An unexpected error occurred during blob stream download.", ex);
            }
        }

        // Other methods and private helper methods as needed

        private static void ValidateFileName(string fileName)
        {
            if (string.IsNullOrEmpty(fileName))
            {
                throw new ArgumentException("File name cannot be empty.", nameof(fileName));
            }
        }

        private static void ValidateObjectKey(string objectKey)
        {
            if (string.IsNullOrEmpty(objectKey))
            {
                throw new ArgumentException("Object key cannot be empty.", nameof(objectKey));
            }
        }

        private static void ValidateFolderName(string folderName)
        {
            if (string.IsNullOrEmpty(folderName))
            {
                throw new ArgumentException("Folder name cannot be empty.", nameof(folderName));
            }
        }

        private static void ValidateContainerAndBlobNames(string containerName, string blobName)
        {
            if (string.IsNullOrEmpty(containerName) || string.IsNullOrEmpty(blobName))
            {
                throw new ArgumentException("Container name and blob name cannot be empty.", nameof(containerName));
            }
        }

        private async Task CreateFolderAsync(string folderKey)
        {
            var request = new PutObjectRequest
            {
                BucketName = _bucketName,
                Key = folderKey,
                InputStream = new MemoryStream(new byte[0]) // Upload an empty object to create a "folder"
            };

            try
            {
                await _s3Client.PutObjectAsync(request);
            }
            catch (AmazonS3Exception ex)
            {
                // Handle S3-specific exceptions
                throw new Exception("Error occurred while creating a folder in S3.", ex);
            }
            catch (Exception ex)
            {
                // Handle other unexpected exceptions
                throw new Exception("An unexpected error occurred during folder creation.", ex);
            }
        }
    }
}
