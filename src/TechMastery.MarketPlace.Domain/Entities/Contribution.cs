using TechMastery.MarketPlace.Domain.Common;

namespace TechMastery.MarketPlace.Domain.Entities
{
    public class Contribution : AuditableEntity
    {
        public Guid ProductId { get; set; }
        public Product? Product { get; set; }
        public Guid ContributorId { get; set; }
        public Contributor? Contributor { get; set; }
        public decimal SharePercentage { get; set; }
    }
}
