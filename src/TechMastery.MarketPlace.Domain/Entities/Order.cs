using TechMastery.MarketPlace.Domain.Common;

namespace TechMastery.MarketPlace.Domain.Entities
{
    public class Order : AuditableEntity
    {
        public Guid OrderId { get; protected set; }
        public Guid UserId { get; set; }
        public decimal OrderTotal => CalculateOrderTotal();
        public OrderStatus OrderStatus { get; set; }
        public DateTime OrderPlaced { get; set; }
        public ICollection<OrderLineItem> OrderLineItems { get; set; } = new List<OrderLineItem>();
        public string? BuyerUsername { get; set; }
        public DateTime PurchaseDate { get; set; }
        public bool OrderPaid { get; set; }
        public Guid CartId { get; set; }
        public string? OrderEmail { get; set; }

        public Order(Guid userId)
        {
            UserId = userId;
            OrderPlaced = DateTime.UtcNow;
            OrderStatus = OrderStatus.Pending;
        }

        public void AddOrderLineItem(Guid productId, decimal price, int quantity)
        {
            var orderLineItem = new OrderLineItem(price, productId, quantity);

            OrderLineItems.Add(orderLineItem);
        }

        public void AddOrderLineItems(IList<OrderLineItem> orderLineItems)
        {
            if (orderLineItems == null)
            {
                throw new ArgumentNullException(nameof(orderLineItems), "Order line items list cannot be null.");
            }

            foreach (var orderLineItem in orderLineItems)
            {
                if (orderLineItem == null)
                {
                    // You can choose to throw an exception, log an error, or skip the null item.
                    throw new ArgumentException("Order line item cannot be null.", nameof(orderLineItems));
                }
                // Add the order line item to the collection.
                OrderLineItems.Add(orderLineItem);
            }
        }


        public void ClearOrderLineItems()
        {
            OrderLineItems.Clear();
        }

        private decimal CalculateOrderTotal()
        {
            return OrderLineItems.Sum(item => item.UnitPrice * item.Quantity);
        }

    }
}
