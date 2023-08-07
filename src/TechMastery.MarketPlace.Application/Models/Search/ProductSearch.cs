namespace TechMastery.MarketPlace.Application.Models.Search
{
    public class ProductSearch
    {
        public Guid ProductListingId { get; set; }
        public required string Title { get; set; }
        public required string Description { get; set; }
        public required string Category { get; set; }
        public decimal Price { get; set; }
    }
}

