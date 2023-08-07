using MediatR;
using System.Collections.Generic;
using System.Linq;
using TechMastery.MarketPlace.Application.Contracts.Persistence;
using TechMastery.MarketPlace.Application.Features.Product.Queries.GetProductsList;

namespace TechMastery.MarketPlace.Application.Features.Product.Queries.GetProductsList
{
    public class GetProductsBySubCategoryQueryHandler : IRequestHandler<GetProductsBySubCategoryQuery, List<ProductListVm>>
    {
        private readonly IProductRepository _productListingRepository;

        public GetProductsBySubCategoryQueryHandler(IProductRepository productListingRepository)
        {
            _productListingRepository = productListingRepository;
        }

        public async Task<List<ProductListVm>> Handle(GetProductsBySubCategoryQuery request, CancellationToken cancellationToken)
        {
            var productsBySubCategory = await _productListingRepository.GetProductsBySubCategoryAsync(request.SubCategoryId);

            var productList = productsBySubCategory.Select(product => new ProductListVm
            {
                // Map other properties as needed
            }).ToList();

            return productList;
        }
    }
}
