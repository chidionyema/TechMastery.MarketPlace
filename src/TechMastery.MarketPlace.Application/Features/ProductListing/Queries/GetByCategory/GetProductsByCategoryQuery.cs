using MediatR;

namespace TechMastery.MarketPlace.Application.Features.Product.Queries.GetProductsList
{
    public class GetProductsByCategoryQuery : IRequest<List<ProductListVm>>
    {
        public Guid CategoryId { get; set; }
        public Guid SubCategoryId { get; set; }
    }
}
