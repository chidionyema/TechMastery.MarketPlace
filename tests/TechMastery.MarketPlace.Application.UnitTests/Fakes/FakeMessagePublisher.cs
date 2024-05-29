using Moq;
using TechMastery.MarketPlace.Application.Contracts;

namespace TechMastery.MarketPlace.Application.IntegrationTests.Fakes
{
    public class FakeMessagePublisher : IMessagePublisher
    {
        private readonly Mock<IMessagePublisher> _mock;

        public FakeMessagePublisher()
        {
            _mock = new Mock<IMessagePublisher>();
        }

        public async Task PublishAsync<TMessage>(TMessage message, string queueName, CancellationToken ca) where TMessage : class, IMessage
        {
            // Log the message being published
            Console.WriteLine($"Fake MessagePublisher: Message published to {queueName}");

            // Simulate a completed Task
            await Task.CompletedTask;
        }

        // Add additional members if required
        public void SetupPublishAsync<TMessage>(Action<TMessage, string> action)
            where TMessage : class, IMessage
        {
            _mock.Setup(publisher => publisher.PublishAsync(It.IsAny<TMessage>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
                 .Callback<TMessage, string>((message, queueName) => action(message, queueName))
                 .Returns(Task.CompletedTask);
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public Task PublishSerializedAsync(string serializedMessage, string queueName)
        {
            throw new NotImplementedException();
        }

        // Expose the Mock for assertions or verifications in tests
        public Mock<IMessagePublisher> Mock => _mock;
    }
}
