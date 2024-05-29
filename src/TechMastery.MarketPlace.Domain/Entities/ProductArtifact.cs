using TechMastery.MarketPlace.Domain.Common;

namespace TechMastery.MarketPlace.Domain.Entities
{
    public class ProductArtifact : AuditableEntity
    {
        public ProductArtifact(ProductArtifactTypeEnum artifactType, string blobUrl, DateTime downloadDate)
        {
            ArtifactType = artifactType;
            BlobUrl = blobUrl;
            DownloadDate = downloadDate;
        }

        public Guid Id { get; protected set; }
        public Guid ProductId { get; private set; }
        public string BlobUrl { get; private set; }
        public DateTime DownloadDate { get; private set; }
        public bool IsMarkedForDeletion { get; private set; }
        public ProductArtifactTypeEnum ArtifactType { get; private set; }

        public void MarkForDeletion()
        {
            IsMarkedForDeletion = true;
        }
    }
}
