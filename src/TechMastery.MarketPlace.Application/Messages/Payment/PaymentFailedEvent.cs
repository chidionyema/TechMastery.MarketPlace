namespace TechMastery.MarketPlace.Application.Messages.Payment
{
	public class PaymentFailedEvent
	{
        public Guid OrderId { get; set; }
        public string ErrorMessage { get; set; }
    }
}

