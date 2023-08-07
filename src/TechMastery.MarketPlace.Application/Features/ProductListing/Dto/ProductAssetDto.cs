using TechMastery.MarketPlace.Domain.Entities;
using Microsoft.AspNetCore.Http;
namespace TechMastery.MarketPlace.Application.DataTransferObjects
{
	public class ProductAssetDto
	{	
		public required IFormFile FormFile { get; set; }
		public ProductArtifactTypeEnum ArtifactType { get; set; }
        public string? BlobUrl { get; internal set; }
		public Guid AssetId { get; set; }
    }
}

