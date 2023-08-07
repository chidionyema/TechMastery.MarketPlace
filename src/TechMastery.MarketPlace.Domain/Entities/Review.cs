using TechMastery.MarketPlace.Domain.Common;

namespace TechMastery.MarketPlace.Domain.Entities
{
    public class ProductReview : AuditableEntity
    {
        public Guid ProductReviewId { get; protected set; }
        public Guid ProductId { get; private set; }
        public Product? Product { get; private set; }
        public string? ReviewerUsername { get; private set; }
        public string? Comment { get; private set; }
        public int Rating { get; private set; }
    }
}