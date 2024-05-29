
using TechMastery.MarketPlace.Application.Persistence.Contracts;
using TechMastery.MarketPlace.Application.Models;
using TechMastery.MarketPlace.Domain.Entities;

namespace TechMastery.MarketPlace.Persistence.Repositories
{
    public class ShoppingCartRepository : BaseRepository<ShoppingCart>, IShoppingCartRepository
    {
        public ShoppingCartRepository(ApplicationDbContext dbContext) : base(dbContext) { }

        public async Task<ShoppingCart?> GetByUserIdAsync(Guid userId)
        {
            var options = new QueryOptions<ShoppingCart>
            {
                Filter = c => c.UserId == userId
            };

            return (await GetAsync(options)).SingleOrDefault();
        }
    }
}

