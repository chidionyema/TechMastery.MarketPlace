using System;
namespace TechMastery.MarketPlace.Infrastructure.Payments.Models
{
    public record AddStripePayment(
        string CustomerId,
        string ReceiptEmail,
        string Description,
        string Currency,
        long Amount);
}

