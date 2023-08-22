using System;
namespace TechMastery.MarketPlace.Application.Messages.Payment
{
    public class PaymentCompletedEvent
    {
        public Guid OrderId { get; set; }
        public Guid PaymentId { get; set; }
    }

}

