namespace TechMastery.MarketPlace.Application.Models.Payment
{
    public class PaymentInfo
    {
        public required string Token { get; set; } // Stripe token representing payment source
        public decimal Amount { get; set; } // Payment amount
        public decimal SellerCut { get; set; } // Payment amount
        public required string Currency { get; set; } // Currency code (e.g., "USD")
        public string? Description { get; set; } // Payment description
        public string SellerStripeAccountId { get; set; }
    }
}

