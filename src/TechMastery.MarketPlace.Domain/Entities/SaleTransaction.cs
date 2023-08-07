using System;
using TechMastery.MarketPlace.Domain.Common;

namespace TechMastery.MarketPlace.Domain.Entities
{
    public class SaleTransaction : AuditableEntity
    {
        public Guid Id { get; protected set; }
        public Guid OrderId { get; private set; }
        public Order? Order { get; private set; }
        public int ProductId { get; private set; }
        public Product? Product { get; private set; }
        public DateTime TransactionDate { get; private set; }
        public decimal TotalAmount { get; private set; }
    }
}

