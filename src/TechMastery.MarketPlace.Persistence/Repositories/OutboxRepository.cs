using System.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TechMastery.MarketPlace.Application.Contracts.Persistence;
using TechMastery.MarketPlace.Domain.Entities;

namespace TechMastery.MarketPlace.Persistence.Repositories
{
    public class OutboxRepository : BaseRepository<OutboxMessage>, IOutboxRepository
    {
        private readonly ILogger<OutboxRepository> _logger;

        public OutboxRepository(ApplicationDbContext dbContext, ILogger<OutboxRepository> logger) : base(dbContext)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<IEnumerable<OutboxMessage>> GetPendingMessagesAsync()
        {
            return await _dbContext.OutboxMessages
                                 .Where(m => m.ProcessedDate == null)
                                 .ToListAsync();
        }

        public async Task ArchiveAsync(OutboxMessage message)
        {
            _dbContext.OutboxMessages.Remove(message);
            await _dbContext.SaveChangesAsync();

            var archivedMessage = new ArchivedOutboxMessage
            {
                Id = message.Id,
                Payload = message.Payload,
                CreatedDate = message.CreatedDate,
                ProcessedDate = DateTime.UtcNow
            };
            await _dbContext.ArchivedOutboxMessages.AddAsync(archivedMessage);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<IEnumerable<OutboxMessage>> GetUnprocessedMessagesAsync()
        {
            return await _dbContext.OutboxMessages
                                 .Where(m => m.ProcessedDate == null)
                                 .ToListAsync();
        }

        public async Task MarkAsProcessedAsync(Guid messageId)
        {
            var message = await _dbContext.OutboxMessages.FindAsync(messageId);
            if (message != null)
            {
                message.ProcessedDate = DateTime.UtcNow;
                _dbContext.OutboxMessages.Update(message);
                await _dbContext.SaveChangesAsync();
            }
        }

        public async Task<bool> TryLockMessageAsync(Guid messageId)
        {
            bool lockAcquired = false;

            using var transaction = await _dbContext.Database.BeginTransactionAsync(IsolationLevel.Serializable);
            try
            {
                var message = await _dbContext.OutboxMessages.FindAsync(messageId);

                if (message == null)
                {
                    _logger.LogWarning($"Message with ID {messageId} not found.");
                    throw new InvalidOperationException($"Message with ID {messageId} not found.");
                }

                if (!message.LockedAt.HasValue)
                {
                    message.LockedAt = DateTime.UtcNow;
                    await _dbContext.SaveChangesAsync();
                    await transaction.CommitAsync();
                    lockAcquired = true;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error while trying to lock message {messageId}. Rolling back transaction.");
                try
                {
                    await transaction.RollbackAsync();
                }
                catch (Exception rollbackEx)
                {
                    _logger.LogError(rollbackEx, "Error rolling back the transaction.");
                }
            }

            return lockAcquired;
        }
    }
}
