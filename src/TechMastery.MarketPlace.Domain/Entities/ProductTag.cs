using TechMastery.MarketPlace.Domain.Common;

namespace TechMastery.MarketPlace.Domain.Entities
{
    public class ProductTag : AuditableEntity
    {
        public ProductTag (string tagName)
        {
            Name = tagName;
        }
        public Guid TagId { get; protected set; }
        public string? Name { get; private set; }
        public ICollection<Product> ProductListings { get; private set; } = new List<Product>();
    }
}
