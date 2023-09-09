using TechMastery.MarketPlace.Domain.Entities;

namespace TechMastery.MarketPlace.Application.Features.ProductListing.DataTransferObjects
{
	public class ProductDependencyDto
	{
        public required string Name { get; set; }
        public required string Version { get; set; }
        public DependencyTypeEnum DependencyType { get; set; }
        public Guid DependencyId { get; internal set; }
    }
}

