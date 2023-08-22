namespace TechMastery.MarketPlace.Application.Models.Payment
{
    public class PaymentInfo
    {
        public string Token { get; set; } // Stripe token representing payment source
        public decimal Amount { get; set; }
        public string? Currency { get; set; }
        public string? Description { get; set; }
        public decimal PaymentAmount { get; set; }
        public string? SellerStripeAccountId { get; set; }

        public decimal GetPaymentAmount()
        {
            return PaymentAmount - (PaymentAmount * 0.15m);
        }
    }
}

