using MediatR;
using TechMastery.MarketPlace.Application.Contracts.Persistence;
using TechMastery.MarketPlace.Application.Models;
namespace TechMastery.MarketPlace.Application.Features.Product.Queries.GetProductsList
{
    public class GetLatestProductsQuery : IRequest<List<ProductListVm>>
    {
    }

    public class GetBestSellingProductsListQueryHandler : IRequestHandler<GetLatestProductsQuery, List<ProductListVm>>
    {
        private readonly IProductRepository _productListingRepository;

        public GetBestSellingProductsListQueryHandler(IProductRepository productListingRepository)
        {
            _productListingRepository = productListingRepository;
        }

        public async Task<List<ProductListVm>> Handle(GetLatestProductsQuery request, CancellationToken cancellationToken)
        {
            var queryOptions = new QueryOptions<Domain.Entities.Product>
            {
                OrderBy = products => products.OrderByDescending(p => p.CreatedDate)
            };
            var latestProducts = await _productListingRepository.GetProductsAsync(queryOptions);


            var productList = latestProducts.Select(product => new ProductListVm
            {
                Name = product.Name,
            }).ToList();

            return productList;
        }
    }
}
