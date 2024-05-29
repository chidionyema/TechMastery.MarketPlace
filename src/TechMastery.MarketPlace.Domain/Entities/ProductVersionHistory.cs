using TechMastery.MarketPlace.Domain.Common;

namespace TechMastery.MarketPlace.Domain.Entities
{
    public class ProductVersionHistory : AuditableEntity
    {
        public Guid Id { get; protected set; }
        public Guid ProductId { get; private set; }
        public string Version { get; private set; }
        public string Description { get; private set; }

        public ProductVersionHistory(Guid productId, string version, string description)
        {
            Id = Guid.NewGuid();
            ProductId = productId;
            Version = version;
            Description = description;
        }
    }
}
