using System;
using TechMastery.MarketPlace.Domain.Entities;

namespace TechMastery.MarketPlace.Application.Features.Orders.ViewModel
{
    public class OrderListVm
    {
        public Guid OrderId { get; set; }
        public Guid UserId { get; set; }
        public decimal OrderTotal => CalculateOrderTotal();
        public OrderStatus OrderStatus { get; set; }
        public DateTime OrderPlaced { get; set; }
        public ICollection<OrderLineItemVm> OrderLineItems { get; set; } = new List<OrderLineItemVm>();
        public string? BuyerUsername { get; set; }
        public DateTime PurchaseDate { get; set; }
        public bool OrderPaid { get; set; }

        private decimal CalculateOrderTotal()
        {
            return OrderLineItems.Sum(item => item.UnitPrice * item.Quantity);
        }
    }
}

