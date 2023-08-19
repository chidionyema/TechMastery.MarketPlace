using MediatR;
using TechMastery.MarketPlace.Application.DataTransferObjects;
using TechMastery.MarketPlace.Application.Features.ProductListing.DataTransferObjects;
using TechMastery.MarketPlace.Application.Features.ProductListing.Dto;

namespace TechMastery.MarketPlace.Application.Features.ProductListing.Handlers
{
    public record AddOrUpdateListing(string Name, string Description, decimal ListingPrice, decimal PromotionPrice, CategoryDto Category) : IRequest<Guid>
    {
        public AddOrUpdateListing() : this("", "", 0, 0, new CategoryDto())
        {
            UploadAssets = new List<ProductAssetDto>();
            Dependencies = new List<ProductDependencyDto>();
            Tags = new List<string>();
        }

        public string DemoUrl { get; set; }
        public CategoryDto Category { get;  set; }
        public Guid ListingId { get; set; }
        public IReadOnlyList<ProductAssetDto> UploadAssets { get;  set; }
        public IReadOnlyList<ProductDependencyDto> Dependencies { get;  set; }
        public IReadOnlyList<string> Tags { get;  set; }
    }
}
