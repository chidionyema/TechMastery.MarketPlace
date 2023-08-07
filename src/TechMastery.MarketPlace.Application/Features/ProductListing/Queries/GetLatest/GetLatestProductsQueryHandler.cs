using MediatR;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TechMastery.MarketPlace.Application.Contracts.Persistence;

namespace TechMastery.MarketPlace.Application.Features.Product.Queries.GetProductsList
{
    public class GetBestSellingProductsListQueryHandler : IRequestHandler<GetLatestProductsQuery, List<ProductListVm>>
    {
        private readonly IProductRepository _productListingRepository;

        public GetBestSellingProductsListQueryHandler(IProductRepository productListingRepository)
        {
            _productListingRepository = productListingRepository;
        }

        public async Task<List<ProductListVm>> Handle(GetLatestProductsQuery request, CancellationToken cancellationToken)
        {
            var latestProducts = await _productListingRepository.GetBestLatestProductsAsync(); // Assuming this method exists

            var productList = latestProducts.Select(product => new ProductListVm
            {
                Name = product.Name,
            }).ToList();

            return productList;
        }
    }
}
