using TechMastery.MarketPlace.Application.Contracts;

namespace TechMastery.MarketPlace.Application.Messaging
{
    public class ProductAdded : IMessage
    {
        public Guid MessageId { get; set; }
        public DateTime Timestamp { get; set; }
        public Guid ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public decimal Price { get; set; }
    }
}

