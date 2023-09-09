using System;
namespace TechMastery.MarketPlace.Domain.Entities
{
	public class PaymentStatus
    {
        public int Id { get;  set; }
        public string? Name { get;  set; }
        public PaymentStatusEnum StatusEnum { get; private set; }
    }
}

