using System.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TechMastery.MarketPlace.Application.Persistence.Contracts;
using TechMastery.MarketPlace.Domain.Entities;

namespace TechMastery.MarketPlace.Persistence.Repositories
{
    public class OutboxRepository : BaseRepository<OutboxMessage>, IOutboxRepository
    {
        private readonly ILogger<OutboxRepository> _logger;

        public OutboxRepository(ApplicationDbContext dbContext, ILogger<OutboxRepository> logger)
            : base(dbContext)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger), "Logger cannot be null.");
        }


        public async Task<IEnumerable<OutboxMessage>> GetPendingMessagesAsync()
        {
            return await _dbContext.OutboxMessages
                                 .Where(m => m.ProcessedDate == null)
                                 .ToListAsync();
        }

        public async Task ArchiveAsync(OutboxMessage message)
        {
            using var transaction = await _dbContext.Database.BeginTransactionAsync();
            try
            {
                _dbContext.OutboxMessages.Remove(message);

                var archivedMessage = new ArchivedOutboxMessage
                {
                    Id = message.Id,
                    Payload = message.Payload,
                    CreatedDate = message.CreatedDate,
                    ProcessedDate = DateTime.UtcNow
                };

                await _dbContext.ArchivedOutboxMessages.AddAsync(archivedMessage);
                await _dbContext.SaveChangesAsync();

                await transaction.CommitAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred during the archival process.");
                await transaction.RollbackAsync();
            }
        }

        public async Task MarkAsProcessedAsync(Guid messageId)
        {
            var message = await _dbContext.OutboxMessages.FindAsync(messageId);
            if (message == null)
            {
                _logger.LogWarning($"Message with ID {messageId} not found for marking as processed.");
                return;
            }

            message.ProcessedDate = DateTime.UtcNow;
            _dbContext.OutboxMessages.Update(message);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<bool> TryLockMessageAsync(Guid messageId)
        {
            using var transaction = await _dbContext.Database.BeginTransactionAsync(IsolationLevel.Serializable);
            var message = await _dbContext.OutboxMessages.FindAsync(messageId);

            if (message == null)
            {
                _logger.LogWarning($"Message with ID {messageId} not found for locking.");
                return false;
            }

            if (!message.LockedAt.HasValue)
            {
                message.LockedAt = DateTime.UtcNow;
                await _dbContext.SaveChangesAsync();
                await transaction.CommitAsync();
                return true;
            }

            return false;
        }
    }
}
