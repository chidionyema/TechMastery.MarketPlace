
using TechMastery.MarketPlace.Application.Models.Search;

namespace TechMastery.MarketPlace.Application.Contracts.Infrastructure
{
    public interface IProductSearchService
    {
        List<ProductSearch> SearchProducts(ProductSearchRequest request);
        Task IndexProducts(List<ProductSearch> products);
    }

}

