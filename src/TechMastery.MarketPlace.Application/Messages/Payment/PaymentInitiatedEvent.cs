using TechMastery.MarketPlace.Application.Models.Payment;

namespace TechMastery.MarketPlace.Application.Messages.Payment
{
    public class PaymentInitiatedEvent
    {
        public Guid OrderId { get; set; }
        public decimal PaymentAmount { get; set; }
        public PaymentInfo PaymentInfo { get; set; }
    }

}

