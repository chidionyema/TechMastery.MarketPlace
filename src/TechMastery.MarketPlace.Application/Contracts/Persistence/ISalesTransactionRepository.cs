using TechMastery.MarketPlace.Domain.Entities;

namespace TechMastery.MarketPlace.Application.Contracts.Persistence
{
    public interface ISalesTransactionRepository : IAsyncRepository<SaleTransaction>
    {
    }
}
