using TechMastery.MarketPlace.Domain.Entities;

namespace TechMastery.MarketPlace.Application.Contracts.Persistence
{
    public interface IOrderRepository : IAsyncRepository<Order>
    {
        Task<Order?> GetByCartId(Guid cartId);
        Task<List<Order>> GetPagedOrdersForMonth(DateTime date, int page, int size);
        Task<int> GetTotalCountOfOrdersForMonth(DateTime date);
    }
}
