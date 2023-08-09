using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TechMastery.MarketPlace.Application.Contracts.Messaging;
using RabbitMQ.Client;
using Xunit;
using TechMastery.MarketPlace.Infrastructure.IntegrationTests.Base;

namespace TechMastery.MarketPlace.Infrastructure.IntegrationTests
{
    public class RabbitMQFixture : IAsyncLifetime
    {
        private TestContainerManager _containerManager;
        private string _containerId;

        public async Task InitializeAsync()
        {
            _containerManager = new TestContainerManager();

            var environmentVariables = new List<string>
            {
                "RABBITMQ_DEFAULT_USER=guest",
                "RABBITMQ_DEFAULT_PASS=guest"
            };

            var portBindings = new List<string>
            {
                "5672"
            };
            // Stop and remove the existing container with the same name if it exists
            await _containerManager.StopAndRemoveContainerAsync("rabbitmq:latest");

            _containerId = await _containerManager.StartContainerAsync("rabbitmq:latest", environmentVariables, 5672, 5672);
            await Task.Delay(TimeSpan.FromSeconds(10));

        }

        public async Task DisposeAsync()
        {
            // TODO: Perform teardown logic for the message publisher...

            await _containerManager.StopAndRemoveContainerAsync(_containerId);
        }

        internal string GetMessageFromQueue(string queueName)
        {
            using (var connection = CreateRabbitMQConnection())
            using (var channel = connection.CreateModel())
            {
                var result = channel.BasicGet(queueName, true);
                if (result != null)
                {
                    return Encoding.UTF8.GetString(result.Body.Span);
                }
                return null;
            }
        }

        public string GetId()
        {
            return _containerId;
        }

        private IConnection CreateRabbitMQConnection()
        {
            var factory = new ConnectionFactory
            {
                HostName = _containerManager.GetContainerIPAddress(_containerId), // Update with your RabbitMQ server information
                UserName = "guest",     // Update with your RabbitMQ username
                Password = "guest"      // Update with your RabbitMQ password
            };
            return factory.CreateConnection();
        }
    }

    [CollectionDefinition("RabbitMQCollection")]
    public class RabbitMQCollection : ICollectionFixture<RabbitMQFixture>
    {
    }
}
