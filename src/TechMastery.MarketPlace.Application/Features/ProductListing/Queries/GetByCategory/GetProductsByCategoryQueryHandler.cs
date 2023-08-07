using MediatR;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TechMastery.MarketPlace.Application.Contracts.Persistence;

namespace TechMastery.MarketPlace.Application.Features.Product.Queries.GetProductsList
{
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
