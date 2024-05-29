using System;
using TechMastery.MarketPlace.Domain.Common;

namespace TechMastery.MarketPlace.Domain.Entities
{
    public class Contributor : AuditableEntity
    {
        public Guid UserId { get; set; }
        public string? Email { get; set; }
        // Other properties related to the contributor
        public ICollection<Contribution>? Contributions { get; set; }
    }
}

