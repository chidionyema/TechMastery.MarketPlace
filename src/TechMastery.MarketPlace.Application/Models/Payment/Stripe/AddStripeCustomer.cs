using System;
namespace TechMastery.MarketPlace.Infrastructure.Payments.Models
{
    public record AddStripeCustomer(
        string Email,
        string Name,
        AddStripeCard CreditCard);
}

