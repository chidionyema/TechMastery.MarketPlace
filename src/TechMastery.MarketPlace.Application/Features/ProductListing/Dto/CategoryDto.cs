namespace TechMastery.MarketPlace.Application.Features.ProductListing.Dto
{
    public class CategoryDto
    {
        public Guid CategoryId { get; set; }
        public string Name { get; set; } = string.Empty;
        public Guid? ParentCategoryId { get; set; }
        public CategoryDto? ParentCategory { get; set; }
        public ICollection<CategoryDto> SubCategories { get; set; } = new List<CategoryDto>();
    }
}

