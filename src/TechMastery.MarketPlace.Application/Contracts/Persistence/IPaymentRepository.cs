using System;
using TechMastery.MarketPlace.Domain.Entities;

namespace TechMastery.MarketPlace.Application.Contracts.Persistence
{
    public interface IPaymentRepository :IAsyncRepository<Payment>
    {
        Task <Payment> GetSuccessfulPaymentByOrderId(Guid orderId);
    }
}

