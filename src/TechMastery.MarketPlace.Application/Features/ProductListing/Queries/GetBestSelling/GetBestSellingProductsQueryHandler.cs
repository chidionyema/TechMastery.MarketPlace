using MediatR;
using TechMastery.MarketPlace.Application.Contracts.Persistence;
using TechMastery.MarketPlace.Application.Models;
using TechMastery.MarketPlace.Domain.Entities; // Make sure to import your entity namespace

namespace TechMastery.MarketPlace.Application.Features.Product.Queries.GetProductsList
{
    public class GetBestSellingProductsQuery : IRequest<List<ProductListVm>>
    {
    }

    public class GetBestSellingProductsQueryHandler : IRequestHandler<GetBestSellingProductsQuery, List<ProductListVm>>
    {
        private readonly IProductRepository _productListingRepository;

        public GetBestSellingProductsQueryHandler(IProductRepository productListingRepository)
        {
            _productListingRepository = productListingRepository;
        }

        public async Task<List<ProductListVm>> Handle(GetBestSellingProductsQuery request, CancellationToken cancellationToken)
        {
            var queryOptions = new QueryOptions<Domain.Entities.Product>
            {
                OrderBy = (products) => products.OrderByDescending(p => p.Orders.Count)
            };


            var bestSellingProducts = await _productListingRepository.GetProductsAsync(queryOptions);

            var productList = bestSellingProducts.Select(product => new ProductListVm
            {
                CategoryId = product.CategoryId,
                Name = product.Name,
            }).ToList();

            return productList;
        }
    }
}
