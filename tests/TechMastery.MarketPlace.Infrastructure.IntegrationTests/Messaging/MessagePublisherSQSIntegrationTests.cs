using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Moq;
using TechMastery.MarketPlace.Application.Contracts.Messaging;
using TechMastery.MarketPlace.Application.Messaging;
using TechMastery.MarketPlace.Tests.Emulators;
using TechMastery.Messaging.Consumers;
namespace TechMastery.MarketPlace.Infrastructure.IntegrationTests.Messaging
{
    [Collection("MockSQSCollection")]
    public class MessagePublisherSQSIntegrationTests
    {
        private readonly MessagePublisher _messagePublisher;
        private readonly MockSQSFixture _mockSQSFixture;
        private static string QueueName = "product-added-queue-sqs"; // Naming it differently to avoid confusion with the RabbitMQ queue

        public MessagePublisherSQSIntegrationTests(MockSQSFixture mockSQSFixture)
        {
            _mockSQSFixture = mockSQSFixture;

            var busControlConfigurator = new BusControlConfigurator(new MessagingSystemsOptions
            {
                EnableSqs = true,  // Ensure it uses AmazonSQS and not RabbitMQ
                SqsOptions = new SqsOptions
                {
                    AccessKey = "fake-access-key",        // Placeholder values for LocalStack
                    SecretKey = "fake-secret-key",
                    ServiceUrl = "http://localhost:4576",  // This is the default LocalStack SQS endpoint
                    Region = "us-east-1"                   // Default region for LocalStack
                }
            });

            _messagePublisher = new MessagePublisher(busControlConfigurator, new Mock<ILogger<MessagePublisher>>().Object);
        }

        [Fact]
        public async Task PublishMessageToSQS_Success()
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
            // You'll need to use the AWS SDK for .NET to fetch the message from LocalStack's SQS to assert.
            // Unlike RabbitMQ, you cannot directly fetch a single message without polling the queue.
            // Poll the queue using the SDK and assert the message.
        }
    }
}
