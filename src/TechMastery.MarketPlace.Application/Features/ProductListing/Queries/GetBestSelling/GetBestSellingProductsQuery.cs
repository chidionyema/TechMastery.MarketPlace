using MediatR;

namespace TechMastery.MarketPlace.Application.Features.Product.Queries.GetProductsList
{
    public class GetBestSellingProductsQuery : IRequest<List<ProductListVm>>
    {
    }
}
