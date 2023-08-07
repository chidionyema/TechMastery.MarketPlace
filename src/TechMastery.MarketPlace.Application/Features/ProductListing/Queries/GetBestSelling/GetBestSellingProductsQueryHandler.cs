using MediatR;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TechMastery.MarketPlace.Application.Contracts.Persistence;
using TechMastery.MarketPlace.Domain.Entities; // Make sure to import your entity namespace

namespace TechMastery.MarketPlace.Application.Features.Product.Queries.GetProductsList
{
    public class GetBestSellingProductsQueryHandler : IRequestHandler<GetBestSellingProductsQuery, List<ProductListVm>>
    {
        private readonly IProductRepository _productListingRepository;

        public GetBestSellingProductsQueryHandler(IProductRepository productListingRepository)
        {
            _productListingRepository = productListingRepository;
        }

        public async Task<List<ProductListVm>> Handle(GetBestSellingProductsQuery request, CancellationToken cancellationToken)
        {
            var bestSellingProducts = await _productListingRepository.GetBestSellingProductsAsync(); // Assuming this method exists

            var productList = bestSellingProducts.Select(product => new ProductListVm
            {
                CategoryId = product.CategoryId,
                Name = product.Name,
            }).ToList();

            return productList;
        }
    }
}
