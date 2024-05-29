
namespace TechMastery.MarketPlace.Domain.Entities
{
    public class ArchivedOutboxMessage
    {
        public Guid Id { get; set; }
        public Guid CorrelationId { get; set; }
        public string MessageType { get; set; } = string.Empty;
        public string Payload { get; set; } = string.Empty;
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
        public DateTime? ProcessedDate { get; set; }
    }
}

