using System;
using TechMastery.MarketPlace.Domain.Common;

namespace TechMastery.MarketPlace.Domain.Entities
{
	public class PaymentStatus : AuditableEntity
    {
        public string? Name { get;  set; }
        public PaymentStatusEnum StatusEnum { get; private set; }
    }
}

