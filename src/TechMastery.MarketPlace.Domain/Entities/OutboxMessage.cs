using System;
namespace TechMastery.MarketPlace.Domain.Entities
{
    public class OutboxMessage
    {
        public Guid Id { get; set; }
        public Guid CorrelationId { get; set; }
        public string MessageType { get; set; }
        public string Payload { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
        public DateTime? ProcessedDate { get; set; }
        public DateTime? LockedAt { get; set; }
    }
}

