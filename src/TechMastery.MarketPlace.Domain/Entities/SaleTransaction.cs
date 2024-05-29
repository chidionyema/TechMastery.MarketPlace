using System;
using TechMastery.MarketPlace.Domain.Common;

namespace TechMastery.MarketPlace.Domain.Entities
{
    public class SaleTransaction : AuditableEntity
    {
        public Guid Id { get; protected set; }
        public Guid OrderId { get; private set; }
        public int ProductId { get; private set; }
        public DateTime TransactionDate { get; private set; }
        public decimal TotalAmount { get; private set; }
        public string PaymentId { get; private set; }

        public SaleTransaction(Guid orderId, DateTime transactionDate, decimal totalAmount, string paymentId)
        {
            if (orderId == Guid.Empty)
                throw new ArgumentException("OrderId cannot be empty.", nameof(orderId));

            if (string.IsNullOrWhiteSpace(paymentId))
                throw new ArgumentException("Payment ID cannot be empty or null.", nameof(paymentId));

            OrderId = orderId;
            TransactionDate = transactionDate;
            TotalAmount = totalAmount;
            PaymentId = paymentId;
        }
    }
}
