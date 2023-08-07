using System;
using TechMastery.MarketPlace.Application.Contracts.Persistence;
using TechMastery.MarketPlace.Domain.Entities;

namespace TechMastery.MarketPlace.Persistence.Repositories
{
    public class ShoppingCartRepository : BaseRepository<ShoppingCart>, IShoppingCartRepository
    {
        public ShoppingCartRepository(ApplicationDbContext dbContext) : base(dbContext) { }

        public Task<ShoppingCart> GetByUserIdAsync(Guid userId)
        {
            throw new NotImplementedException();
        }
    }
}

