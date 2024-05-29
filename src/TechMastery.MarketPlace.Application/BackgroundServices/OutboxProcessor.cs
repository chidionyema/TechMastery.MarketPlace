using Microsoft.Extensions.Logging;
using Polly;
using TechMastery.MarketPlace.Application.Contracts;
using TechMastery.MarketPlace.Application.Persistence.Contracts;
using TechMastery.MarketPlace.Domain.Entities;

public class OutboxProcessor
{
    private readonly IOutboxRepository _outboxRepo;
    private readonly ILogger<OutboxProcessor> _logger;
    private readonly IMessagePublisher _messagePublisher;
    private readonly AsyncPolicy _combinedPolicy;

    public OutboxProcessor(IOutboxRepository outboxRepo,
                           ILogger<OutboxProcessor> logger,
                           IMessagePublisher messagePublisher)
    {
        _outboxRepo = outboxRepo ?? throw new ArgumentNullException(nameof(outboxRepo), "OutboxRepository cannot be null.");
        _logger = logger ?? throw new ArgumentNullException(nameof(logger), "Logger cannot be null.");
        _messagePublisher = messagePublisher ?? throw new ArgumentNullException(nameof(messagePublisher), "MessagePublisher cannot be null.");

        // Define the policy once during constructor initialization
        var retryPolicy = Policy
            .Handle<Exception>()
            .WaitAndRetryAsync(5, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
            (exception, timeSpan, retryCount, context) =>
            {
                _logger.LogWarning($"Retry {retryCount} encountered an error: {exception.Message}. Waiting {timeSpan} before next try.");
            });

        var timeoutPolicy = Policy.TimeoutAsync(TimeSpan.FromMinutes(1));

        _combinedPolicy = Policy.WrapAsync(retryPolicy, timeoutPolicy);
    }

    public async Task ProcessOutboxAsync()
    {
        _logger.LogInformation("Starting outbox processing...");

        var messagesToProcess = await _outboxRepo.GetPendingMessagesAsync();

        foreach (var message in messagesToProcess)
        {
            if (!await _outboxRepo.TryLockMessageAsync(message.Id))
            {
                _logger.LogInformation($"Message {message.Id} is being processed by another instance. Skipping.");
                continue;
            }

            try
            {
                await _combinedPolicy.ExecuteAsync(async () =>
                {
                    await _messagePublisher.PublishSerializedAsync(message.Payload, string.Empty);
                    await _outboxRepo.MarkAsProcessedAsync(message.Id);                    
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while processing message {message.Id}.");
            }

            await MaybeArchiveOrCleanupAsync(message);
        }

        _logger.LogInformation("Finished outbox processing.");
    }

    private async Task MaybeArchiveOrCleanupAsync(OutboxMessage message)
    {
        if (ShouldArchive(message))
        {
            await _outboxRepo.ArchiveAsync(message);
        }
        else
        {
            await _outboxRepo.DeleteAsync(message);
        }
    }

    private bool ShouldArchive(OutboxMessage message)
    {
        return message.ProcessedDate < DateTime.UtcNow.AddDays(-30);
    }
}
