using System;
namespace TechMastery.MarketPlace.Infrastructure.Payments.Models
{
    public record StripeCustomer(
        string Name,
        string Email,
        string CustomerId);
}

