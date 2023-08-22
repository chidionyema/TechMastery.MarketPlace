using System;
using TechMastery.MarketPlace.Domain.Entities;

namespace TechMastery.MarketPlace.Application.Contracts.Persistence
{
    public interface IOutboxRepository : IAsyncRepository<OutboxMessage>
    {
        Task ArchiveAsync(OutboxMessage message);
        Task<IEnumerable<OutboxMessage>> GetPendingMessagesAsync();
        Task MarkAsProcessedAsync(Guid messageId);
        Task<bool> TryLockMessageAsync(Guid id);
    }
}

