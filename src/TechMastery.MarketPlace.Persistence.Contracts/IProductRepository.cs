using TechMastery.MarketPlace.Domain.Entities;
using TechMastery.MarketPlace.Application.Contracts;
namespace TechMastery.MarketPlace.Application.Persistence.Contracts
{
    public interface IProductRepository : IAsyncRepository<Product>
    {
        Task<IReadOnlyCollection<Product>> GetProductsAsync(IQueryOptions<Product> options);
    }
}

	