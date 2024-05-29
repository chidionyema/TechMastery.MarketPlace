using TechMastery.MarketPlace.Domain.Common;

namespace TechMastery.MarketPlace.Domain.Entities
{
    public class OutboxMessage : AuditableEntity
    {
        public Guid CorrelationId { get; set; }
        public string MessageType { get; set; } = string.Empty;
        public string Payload { get; set; } = string.Empty;
        public DateTime? ProcessedDate { get; set; }
        public DateTime? LockedAt { get; set; }
    }
}

