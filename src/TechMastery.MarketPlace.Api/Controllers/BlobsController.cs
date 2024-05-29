using Microsoft.AspNetCore.Authorization;
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
        [Authorize]
        public IActionResult GetSignedUrl(string fileType)
        {
            var userName = User.Identity.Name;
            // Generate the signed URL using Azure Blob Storage SDK or S3 SDK.
            var signedUrl = GenerateSignedUrl(userName, fileType).ToString();

            if (string.IsNullOrEmpty(signedUrl))
                return BadRequest("Failed to generate signed URL.");

            return Ok(new { signedUrl });
        }

        private async Task<Uri> GenerateSignedUrl(string blobKey, string fileType)
        {
            return await _storageProvider.GenerateSasUploadUriAsync(blobKey + "_" +fileType, DateTimeOffset.MaxValue); 
        }

    }
}

