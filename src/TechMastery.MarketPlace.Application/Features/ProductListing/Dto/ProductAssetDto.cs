using TechMastery.MarketPlace.Domain.Entities;
namespace TechMastery.MarketPlace.Application.DataTransferObjects
{
	public class ProductAssetDto
	{	
		public ProductArtifactTypeEnum ArtifactType { get; set; }
        public string? BlobUrl { get; set; }
		public Guid AssetId { get; set; }
    }
}

