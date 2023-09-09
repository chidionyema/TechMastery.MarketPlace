using System;
using TechMastery.MarketPlace.Domain.Common;

namespace TechMastery.MarketPlace.Domain.Entities
{
    public enum PaymentStatusEnum
    {
        Pending,
        Completed,
        Failed,
        Refunded,
        Disputed,
        Successful
    }

    public class Payment : AuditableEntity
    {
        private Guid orderId;
        private decimal orderTotal;

        public Guid PaymentId { get; protected set; }
        public Guid SaleTransactionId { get; private set; }
        public decimal Amount { get; private set; }
        public string PaymentProviderId { get; private set; } // e.g., transaction ID from Stripe/PayPal
        public PaymentStatusEnum Status { get; private set; }

        public Payment(Guid saleTransactionId, decimal amount, string paymentProviderId)
        {
            if (saleTransactionId == Guid.Empty)
                throw new ArgumentException("SaleTransactionId cannot be empty.", nameof(saleTransactionId));

            if (string.IsNullOrWhiteSpace(paymentProviderId))
                throw new ArgumentException("Payment Provider ID cannot be empty or null.", nameof(paymentProviderId));

            SaleTransactionId = saleTransactionId;
            Amount = amount;
            PaymentProviderId = paymentProviderId;
            Status = PaymentStatusEnum.Pending; // Default status
        }

        public Payment(Guid orderId, decimal orderTotal)
        {
            this.orderId = orderId;
            this.orderTotal = orderTotal;
        }

        public Payment(Guid paymentId, Guid orderId)
        {
            PaymentId = paymentId;
            this.orderId = orderId;
        }

        public void MarkAsCompleted()
        {
            Status = PaymentStatusEnum.Completed;
        }

        public void MarkAsFailed()
        {
            Status = PaymentStatusEnum.Failed;
        }

        public void MarkSuccess(string paymentId)
        {
            throw new NotImplementedException();
        }

        public void MarkFailed(string errorMessage)
        {
            throw new NotImplementedException();
        }
    }
}
