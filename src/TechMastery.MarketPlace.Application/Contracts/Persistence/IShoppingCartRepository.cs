using System;
using TechMastery.MarketPlace.Domain.Entities;

namespace TechMastery.MarketPlace.Application.Contracts.Persistence
{
    public interface IShoppingCartRepository : IAsyncRepository<ShoppingCart>
    {
        Task<ShoppingCart?> GetByUserIdAsync(Guid userId);
    }
}

