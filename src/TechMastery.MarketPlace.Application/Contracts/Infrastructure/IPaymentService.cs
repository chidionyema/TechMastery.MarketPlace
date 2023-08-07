using System;
using TechMastery.MarketPlace.Application.Models.Payment;

namespace TechMastery.MarketPlace.Application.Contracts.Infrastructure
{
    public interface IPaymentService
    {
        Task<PaymentResult> ProcessPaymentAsync(PaymentInfo paymentInfo, CancellationToken ct);
    }

}
