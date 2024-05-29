using TechMastery.MarketPlace.Domain.Entities;
using TechMastery.MarketPlace.Application.Contracts;
namespace TechMastery.MarketPlace.Application.Persistence.Contracts
{
    public interface IOrderRepository : IAsyncRepository<Order>
    {
        Task<Order?> GetByCartId(Guid cartId);
        Task<List<Order>> GetPagedOrdersForMonth(DateTime date, int page, int size);
        Task<int> GetTotalCountOfOrdersForMonth(DateTime date);
    }
}
