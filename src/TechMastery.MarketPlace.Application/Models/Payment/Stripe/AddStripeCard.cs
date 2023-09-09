namespace TechMastery.MarketPlace.Infrastructure.Payments.Models
{
    public record AddStripeCard(
        string Name,
        string CardNumber,
        string ExpirationYear,
        string ExpirationMonth,
        string Cvc);
}

