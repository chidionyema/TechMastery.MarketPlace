using MassTransit;
using Microsoft.Extensions.Logging;

namespace TechMastery.MarketPlace.Application.Contracts.Messaging
{
    public class MessagePublisher : IMessagePublisher, IDisposable
    {
        private readonly BusControlConfigurator _busControlConfigurator;
        private readonly IBusControl _busControl;
        private readonly ILogger<MessagePublisher> _logger;

        public MessagePublisher(BusControlConfigurator busControlConfigurator, ILogger<MessagePublisher> logger)
        {
            _busControlConfigurator = busControlConfigurator;
            _busControl = _busControlConfigurator.ChooseBusConfiguration();
            _logger = logger;
        }

        public async Task PublishAsync<TMessage>(TMessage message, string queueName) where TMessage : class, IMessage
        {
            if (message == null)
                throw new ArgumentNullException(nameof(message));

            if (string.IsNullOrWhiteSpace(queueName))
                throw new ArgumentException("Queue name must not be empty or null.", nameof(queueName));

            try
            {
                var endpoint = await _busControl.GetSendEndpoint(new Uri($"queue:{queueName}"));
                await endpoint.Send(message);
            }

            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while publishing a message to {QueueName}", queueName);
                // Implement proper error handling and retries here
            }
        }

        public void Dispose()
        {
            _busControl.Stop();
        }
    }
}
