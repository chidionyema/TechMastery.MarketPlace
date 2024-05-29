

using TechMastery.MarketPlace.Application.Contracts;

namespace TechMastery.MarketPlace.Application.Messaging
{
    public class OrderPlaced : IMessage
    {
        public Guid MessageId { get; set; }
        public DateTime Timestamp { get; set; }
        public string OrderId { get; set; } = string.Empty;
        public string CustomerId { get; set; } = string.Empty;
        public decimal TotalAmount { get; set; }
    }
}

