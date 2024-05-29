using MediatR;
using System;
using System.Collections.Generic;
using TechMastery.MarketPlace.Application.DataTransferObjects;
using TechMastery.MarketPlace.Application.Exceptions;
using TechMastery.MarketPlace.Application.Features.ProductListing.Dto;

namespace TechMastery.MarketPlace.Application.Features.ProductListing.Handlers
{
    public record AddListingCommand : IRequest<Guid>
    {
        public Guid ProductListingId { get; init; }
        public string Name { get; init; }
        public string Description { get; init; }
        public decimal ListingPrice { get; init; }
        public decimal PromotionPrice { get; init; }
        public CategoryDto Category { get; init; }
        public string DemoUrl { get; init; }
        public List<Guid> LanguageIds { get; set; }
        public List<Guid> FrameworkIds { get; set; }
        public List<Guid> PlatformIds { get; set; }
        public IReadOnlyList<ProductAssetDto> UploadAssets { get; init; }
        public IReadOnlyList<string> Tags { get; init; }

        // Constructor for default values
        public AddListingCommand() : this(
            name: string.Empty,
            description: string.Empty,
            listingPrice: 0,
            promotionPrice: 0,
            category: new CategoryDto(),
            demoUrl: string.Empty,
            listingId: Guid.Empty,
            uploadAssets: new List<ProductAssetDto>(),
            tags: new List<string>()
        )
        { }

        public AddListingCommand(
            string name,
            string description,
            decimal listingPrice,
            decimal promotionPrice,
            CategoryDto category,
            string demoUrl = "",
            Guid listingId = new Guid(),
            IReadOnlyList<ProductAssetDto>? uploadAssets = null,
            IReadOnlyList<string>? tags = null)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Description = description ?? throw new ArgumentNullException(nameof(description));
            ListingPrice = listingPrice;
            PromotionPrice = promotionPrice;
            Category = category ?? throw new ArgumentNullException(nameof(category));
            DemoUrl = demoUrl;
            ProductListingId = listingId;
            UploadAssets = uploadAssets ?? new List<ProductAssetDto>();
            Tags = tags ?? new List<string>();

        }

    }
}
