namespace TechMastery.MarketPlace.Application.Models.Search
{
    public class SearchRequest
    {
        public required string Query { get; set; }
        public required string Category { get; set; }
        public required string SortBy { get; set; }
        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
    }
}

