using Microsoft.AspNetCore.Mvc;
using TechMastery.MarketPlace.Application.Contracts.Infrastructure;

namespace TechMastery.MarketPlace.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BlobController : ControllerBase
    {
        private readonly IStorageProvider _storageProvider;

        public BlobController(IStorageProvider storageProvider)
        {
            _storageProvider = storageProvider;
        }

        [HttpGet]
        public IActionResult GetSignedUrl(string blobKey, string fileType)
        {
            // Ensure the blobKey and fileType are provided.
            if (string.IsNullOrEmpty(blobKey) || string.IsNullOrEmpty(fileType))
                return BadRequest("Blob key or file type missing.");

            // Generate the signed URL using Azure Blob Storage SDK or S3 SDK.
            var signedUrl = GenerateSignedUrl(blobKey, fileType).ToString();

            if (string.IsNullOrEmpty(signedUrl))
                return BadRequest("Failed to generate signed URL.");

            return Ok(new { signedUrl });
        }

        private async Task<Uri> GenerateSignedUrl(string blobKey, string fileType)
        {
            return await _storageProvider.GenerateSasUploadUriAsync(blobKey, DateTimeOffset.MaxValue); 
        }

    }
}

