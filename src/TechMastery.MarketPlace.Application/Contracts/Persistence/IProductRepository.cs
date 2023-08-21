using TechMastery.MarketPlace.Domain.Entities;
namespace TechMastery.MarketPlace.Application.Contracts.Persistence
{
    public interface IProductRepository : IAsyncRepository<Product>
    {
        Task<IReadOnlyCollection<Product>> GetProductsAsync(IQueryOptions<Product> options);
    }
}

	