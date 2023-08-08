using System;
using TechMastery.MarketPlace.Application.Contracts.Messaging;

namespace TechMastery.Messaging.Consumers.Messages
{
    public class ProductAdded : IMessage
    {
        public Guid MessageId { get; set; }
        public DateTime Timestamp { get; set; }
        public string ProductId { get; set; }
        public string ProductName { get; set; }
        public decimal Price { get; set; }
    }
}

