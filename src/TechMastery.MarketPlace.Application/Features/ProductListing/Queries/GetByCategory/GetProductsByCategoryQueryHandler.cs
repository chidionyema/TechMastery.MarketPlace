using MediatR;

using TechMastery.MarketPlace.Application.Persistence.Contracts;

namespace TechMastery.MarketPlace.Application.Features.Product.Queries.GetProductsList
{
    public class GetProductsByCategoryQuery : IRequest<List<ProductListVm>>
    {
        public Guid CategoryId { get; set; }
        public Guid SubCategoryId { get; set; }
    }

    public class GetProductsByCategoryQueryHandler : IRequestHandler<GetProductsByCategoryQuery, List<ProductListVm>>
    {
        private readonly IProductRepository _productListingRepository;

        public GetProductsByCategoryQueryHandler(IProductRepository productListingRepository)
        {
            _productListingRepository = productListingRepository;
        }

        public async Task<List<ProductListVm>> Handle(GetProductsByCategoryQuery request, CancellationToken cancellationToken)
        {
            var allProducts = await _productListingRepository.ListAllAsync();

            var productsByCategory = allProducts
                .GroupBy(product => product.CategoryId)
                .Select(group => new ProductListVm
                {
                    CategoryId = group.Key
                })
                .ToList();

            return productsByCategory;
        }
    }
}
