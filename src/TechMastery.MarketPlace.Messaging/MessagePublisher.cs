using System.Text;
using MassTransit;
using Microsoft.Extensions.Logging;
using Polly;
using Polly.Retry;

namespace TechMastery.MarketPlace.Application.Contracts.Messaging
{
    public class MessagePublisher : IMessagePublisher, IDisposable
    {
        private readonly BusControlConfigurator _busControlConfigurator;
        private readonly IBusControl _busControl;
        private readonly ILogger<MessagePublisher> _logger;
        AsyncRetryPolicy _policy;

        public MessagePublisher(BusControlConfigurator busControlConfigurator, ILogger<MessagePublisher> logger)
        {
            _busControlConfigurator = busControlConfigurator;
            _busControl = _busControlConfigurator.ChooseBusConfiguration();
            _logger = logger;

            _policy = Policy
           .Handle<Exception>()
           .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));
        }

        public async Task PublishSerializedAsync(string serializedMessage, string queueName)
        {
            if (string.IsNullOrWhiteSpace(serializedMessage))
                throw new ArgumentNullException(nameof(serializedMessage));

            if (string.IsNullOrWhiteSpace(queueName))
                throw new ArgumentException("Queue name must not be empty or null.", nameof(queueName));
            try
            {
                await _policy.ExecuteAsync(async () =>
                {
                    var endpoint = await _busControl.GetSendEndpoint(new Uri($"queue:{queueName}"));
                    await endpoint.Send(serializedMessage);
                });           
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while publishing a message to {QueueName}", queueName);
            }
        }


        public async Task PublishAsync<TMessage>(TMessage message, string queueName) where TMessage : class, IMessage
        {
            if (message == null)
                throw new ArgumentNullException(nameof(message));

            if (string.IsNullOrWhiteSpace(queueName))
                throw new ArgumentException("Queue name must not be empty or null.", nameof(queueName));

            await _policy.ExecuteAsync(async () =>
            {
                var endpoint = await _busControl.GetSendEndpoint(new Uri($"queue:{queueName}"));
                await endpoint.Send(message);
            });
        }

        public void Dispose()
        {
            _busControl.Stop();
        }
    }
}
