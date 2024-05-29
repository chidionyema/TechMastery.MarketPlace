using TechMastery.MarketPlace.Domain.Entities;
using TechMastery.MarketPlace.Application.Contracts;
namespace TechMastery.MarketPlace.Application.Persistence.Contracts
{
    public interface IShoppingCartRepository : IAsyncRepository<ShoppingCart>
    {
        Task<ShoppingCart?> GetByUserIdAsync(Guid userId);
    }
}

