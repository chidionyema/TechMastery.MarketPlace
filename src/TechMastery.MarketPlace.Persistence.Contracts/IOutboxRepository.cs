using TechMastery.MarketPlace.Domain.Entities;
using TechMastery.MarketPlace.Application.Contracts;
namespace TechMastery.MarketPlace.Application.Persistence.Contracts
{
    public interface IOutboxRepository : IAsyncRepository<OutboxMessage>
    {
        Task ArchiveAsync(OutboxMessage message);
        Task<IEnumerable<OutboxMessage>> GetPendingMessagesAsync();
        Task MarkAsProcessedAsync(Guid messageId);
        Task<bool> TryLockMessageAsync(Guid id);
    }
}

