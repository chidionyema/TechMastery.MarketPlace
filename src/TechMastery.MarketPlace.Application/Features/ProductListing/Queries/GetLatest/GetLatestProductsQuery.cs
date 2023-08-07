using MediatR;

namespace TechMastery.MarketPlace.Application.Features.Product.Queries.GetProductsList
{
    public class GetLatestProductsQuery : IRequest<List<ProductListVm>>
    {
    }
}
