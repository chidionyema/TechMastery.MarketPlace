namespace TechMastery.Messaging.Consumers
{
    public class MessagingSystemsOptions
    {
        public bool EnableAzureServiceBus { get; set; }
        public bool EnableSqs { get; set; }
        public bool EnableRabbitMq { get; set; }

        public AzureServiceBusOptions? AzureServiceBus { get; set; }
        public SqsOptions? Sqs { get; set; }
        public RabbitMqOptions? RabbitMq { get; set; }
    }
}