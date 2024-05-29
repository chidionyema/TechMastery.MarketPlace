using TechMastery.MarketPlace.Domain.Common;
namespace TechMastery.MarketPlace.Domain.Entities
{	
	public class ProductArtifactDownloadHistory : AuditableEntity
	{
		public Guid Id { get; protected set; }
		public Guid ProductListingId { get; private set; }
		public Product? ProductListing { get; private set; }
		public Guid ProductArtifactId { get; private set; }
		public ProductArtifact? ProductArtifact { get; private set; }
		public DateTime DownloadDate { get; private set; }
		public string ?DownloadedBy { get; private set; }
	}

}