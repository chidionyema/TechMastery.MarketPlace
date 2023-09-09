using TechMastery.MarketPlace.Application.Contracts.Persistence;
using TechMastery.MarketPlace.Domain.Entities;

namespace TechMastery.MarketPlace.Persistence.Repositories
{
    public class ProductRepository : BaseRepository<Product>, IProductRepository
    {
        public ProductRepository(ApplicationDbContext dbContext)
            : base(dbContext)
        {
        }

        public async Task<IReadOnlyCollection<Product>> GetProductsAsync(IQueryOptions<Product> options)
        {
            return await GetAsync(options);
        }
    }
}
