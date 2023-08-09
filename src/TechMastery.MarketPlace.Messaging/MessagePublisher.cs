using System;
using System.Text.Json;
using System.Threading.Tasks;
using Amazon;
using Amazon.Runtime;
using Amazon.SQS;
using Azure.Messaging.ServiceBus;
using RabbitMQ.Client;
using TechMastery.Messaging.Consumers;

namespace TechMastery.MarketPlace.Application.Contracts.Messaging
{
    public class MessagePublisher : IMessagePublisher, IDisposable
    {
        private readonly MessagingSystemsOptions? _messagingSystemsOptions;
        private readonly ServiceBusClient? _serviceBusClient;
        private readonly IAmazonSQS? _sqsClient;
        private readonly IModel? _rabbitMqChannel;

        public MessagePublisher(MessagingSystemsOptions messagingSystemsOptions)
        {
            _messagingSystemsOptions = messagingSystemsOptions;

            if (_messagingSystemsOptions.EnableAzureServiceBus)
            {
                _serviceBusClient = new ServiceBusClient(_messagingSystemsOptions.AzureServiceBus.ConnectionString);
            }

            if (_messagingSystemsOptions.EnableSqs)
            {
                var sqsConfig = new AmazonSQSConfig
                {
                    RegionEndpoint = RegionEndpoint.GetBySystemName(_messagingSystemsOptions.Sqs.Region)
                };

                var credentials = new BasicAWSCredentials(_messagingSystemsOptions.Sqs.AccessKey, _messagingSystemsOptions.Sqs.SecretKey);
                _sqsClient = new AmazonSQSClient(credentials, sqsConfig);
            }

            if (_messagingSystemsOptions.EnableRabbitMq)
            {
                var connectionFactory = new ConnectionFactory
                {
                    HostName = _messagingSystemsOptions.RabbitMq.Host,
                    UserName = _messagingSystemsOptions.RabbitMq.Username,
                    Password = _messagingSystemsOptions.RabbitMq.Password
                };

                var rabbitMqConnection = connectionFactory.CreateConnection();
                _rabbitMqChannel = rabbitMqConnection.CreateModel();
            }
        }

        public async Task PublishAsync<TMessage>(TMessage message, string queueName) where TMessage : class, IMessage
        {
            if (message == null)
                throw new ArgumentNullException(nameof(message));

            if (string.IsNullOrWhiteSpace(queueName))
                throw new ArgumentException("Queue name must not be empty or null.", nameof(queueName));

            if (_messagingSystemsOptions.EnableAzureServiceBus)
            {
                await PublishToAzureServiceBus(message, queueName);
            }

            if (_messagingSystemsOptions.EnableSqs)
            {
                await PublishToSqs(message, queueName);
            }

            if (_messagingSystemsOptions.EnableRabbitMq)
            {
                PublishToRabbitMq(message, queueName);
            }
        }

        private async Task PublishToAzureServiceBus<TMessage>(TMessage message, string queueName) where TMessage : class, IMessage
        {
            await using (ServiceBusSender sender = _serviceBusClient!.CreateSender(queueName))
            {
                string serializedMessage = SerializeMessage(message);
                ServiceBusMessage serviceBusMessage = new ServiceBusMessage(serializedMessage);

                await sender.SendMessageAsync(serviceBusMessage);
            }
        }

        private async Task PublishToSqs<TMessage>(TMessage message, string queueName) where TMessage : class, IMessage
        {
            var request = new Amazon.SQS.Model.SendMessageRequest
            {
                QueueUrl = _messagingSystemsOptions.Sqs.QueueUrl,
                MessageBody = SerializeMessage(message)
            };

            await _sqsClient!.SendMessageAsync(request);
        }

        private void PublishToRabbitMq<TMessage>(TMessage message, string queueName) where TMessage : class, IMessage
        {
            var body = SerializeMessage(message);
            var bodyBytes = System.Text.Encoding.UTF8.GetBytes(body);

            _rabbitMqChannel.BasicPublish(
                exchange: "",
                routingKey: queueName,
                basicProperties: null,
                body: bodyBytes
            );
        }

        private string SerializeMessage<TMessage>(TMessage message)
        {
            // Implement your serialization logic here (e.g., using JSON, XML, etc.)
            // For example, using Newtonsoft.Json:
            return JsonSerializer.Serialize(message);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                // Dispose managed resources
                _serviceBusClient?.DisposeAsync();
                _sqsClient?.Dispose();
                _rabbitMqChannel?.Close();
            }

            // Dispose any unmanaged resources (if any)
        }
    }
}
