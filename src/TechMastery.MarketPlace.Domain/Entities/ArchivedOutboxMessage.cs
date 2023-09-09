﻿using System;
namespace TechMastery.MarketPlace.Domain.Entities
{
    public class ArchivedOutboxMessage
    {
        public Guid Id { get; set; }
        public Guid CorrelationId { get; set; }
        public string MessageType { get; set; }
        public string Payload { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
        public DateTime? ProcessedDate { get; set; }
        public object ProcessedAt { get; set; }
    }
}

