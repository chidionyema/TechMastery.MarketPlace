using System;
namespace TechMastery.MarketPlace.Infrastructure.Payments.Models
{
    public record StripePayment(
        string CustomerId,
        string ReceiptEmail,
        string Description,
        string Currency,
        long Amount,
        string PaymentId);
}

