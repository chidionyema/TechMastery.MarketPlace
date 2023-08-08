namespace TechMastery.MarketPlace.Application.Models.Search
{
    public class ProductSearch
    {
        public Guid ProductId { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public string? Category { get; set; }
        public decimal Price { get; set; }
    }
}

