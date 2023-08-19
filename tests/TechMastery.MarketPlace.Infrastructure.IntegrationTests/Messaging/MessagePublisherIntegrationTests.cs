using Microsoft.Extensions.Logging;
using Moq;
using TechMastery.MarketPlace.Application.Contracts.Messaging;
using TechMastery.MarketPlace.Application.Messaging;
using TechMastery.MarketPlace.Infrastructure.IntegrationTests;
using TechMastery.Messaging.Consumers;
namespace TechMastery.MarketPlace.Application.IntegrationTests
{
    [Collection("RabbitMQCollection")]
    public class MessagePublisherIntegrationTests
    {
        private readonly MessagePublisher _messagePublisher;
        private readonly RabbitMQFixture _rabbitMQFixture;
        private static string QueueName = "product-added-queue";

        public MessagePublisherIntegrationTests(RabbitMQFixture rabbitMQFixture)
        {
            _rabbitMQFixture = rabbitMQFixture;
            _messagePublisher = new MessagePublisher(new BusControlConfigurator (new MessagingSystemsOptions
            {
                EnableRabbitMq = true,
                RabbitMq = new RabbitMqOptions
                {
                    Host = _rabbitMQFixture.GetId(),
                    QueueName = QueueName,     // Replace with your desired queue name
                    Username = "guest",      // Replace with your RabbitMQ username
                    Password = "guest"       // Replace with your RabbitMQ password
                }
            }), new Mock<ILogger<MessagePublisher>>().Object);

        }

        [Fact]
        public async Task PublishMessage_Success()
        {
            // Arrange
            var message = new ProductAdded
            {
                ProductId = Guid.NewGuid(),
                ProductName = "Test Product",
                Price = 99.99m
            };

            // Act
            await _messagePublisher.PublishAsync(message, QueueName);

            // Assert
            var receivedMessage = _rabbitMQFixture.GetMessageFromQueue(QueueName);
            Assert.NotNull(receivedMessage);
            // Additional assertions based on the received message content
        }
    }
}