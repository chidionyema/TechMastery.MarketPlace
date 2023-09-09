using System;
using TechMastery.MarketPlace.Application.Contracts.Messaging;

namespace TechMastery.MarketPlace.Application.Messaging
{
    public class OrderPlaced : IMessage
    {
        public Guid MessageId { get; set; }
        public DateTime Timestamp { get; set; }
        public string OrderId { get; set; }
        public string CustomerId { get; set; }
        public decimal TotalAmount { get; set; }
    }
}

