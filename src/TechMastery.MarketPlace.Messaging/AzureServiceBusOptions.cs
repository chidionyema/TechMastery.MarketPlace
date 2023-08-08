using System;
namespace TechMastery.Messaging
{
    public class AzureServiceBusOptions
    {
        public string? ConnectionString { get; set; }
        public string? QueueName { get; set; }
    }
}

