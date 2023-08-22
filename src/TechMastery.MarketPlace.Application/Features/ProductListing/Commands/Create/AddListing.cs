using System;
using System.Collections.Generic;
using MediatR;
using TechMastery.MarketPlace.Application.DataTransferObjects;
using TechMastery.MarketPlace.Application.Features.ProductListing.DataTransferObjects;
using TechMastery.MarketPlace.Application.Features.ProductListing.Dto;

namespace TechMastery.MarketPlace.Application.Features.ProductListing.Handlers
{
    public record AddListingCommand : IRequest<Guid>
    {
        public string Name { get; init; }
        public string Description { get; init; }
        public decimal ListingPrice { get; init; }
        public decimal PromotionPrice { get; init; }
        public CategoryDto Category { get; init; }
        public string DemoUrl { get; init; }
        public Guid ListingId { get; init; }
        public IReadOnlyList<ProductAssetDto> UploadAssets { get; init; }
        public IReadOnlyList<ProductDependencyDto> Dependencies { get; init; }
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
            dependencies: new List<ProductDependencyDto>(),
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
            IReadOnlyList<ProductDependencyDto>? dependencies = null,
            IReadOnlyList<string>? tags = null )
        {
            Name = name;
            Description = description;
            ListingPrice = listingPrice;
            PromotionPrice = promotionPrice;
            Category = category ?? throw new ArgumentNullException(nameof(category));
            DemoUrl = demoUrl;
            ListingId = listingId;
            UploadAssets = uploadAssets ?? new List<ProductAssetDto>();
            Dependencies = dependencies ?? new List<ProductDependencyDto>();
            Tags = tags ?? new List<string>();
        }

        // NOTE: For more completeness, consider adding validation methods or attributes here.
    }
}
