namespace TechMastery.MarketPlace.Application.Models.Search
{
    public class ProductSearchRequest
    {
        public string? Query { get; set; }
        public string? Category { get; set; }
        public string? SortBy { get; set; }
        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
    }
}

