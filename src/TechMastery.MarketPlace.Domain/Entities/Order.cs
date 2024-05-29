using TechMastery.MarketPlace.Domain.Common;

namespace TechMastery.MarketPlace.Domain.Entities
{
    public class Order : AuditableEntity
    {
        public Guid UserId { get; private set; }
        public decimal OrderTotal => CalculateOrderTotal();
        public OrderStatus OrderStatus { get; private set; }
        public DateTime OrderPlaced { get; private set; }
        private readonly List<OrderLineItem> _orderLineItems = new List<OrderLineItem>();
        public IReadOnlyCollection<OrderLineItem> OrderLineItems => _orderLineItems.AsReadOnly();
        public string? BuyerUsername { get; set; }
        public DateTime PurchaseDate { get; private set; }
        public bool OrderPaid { get; private set; }
        public Guid CartId { get; private set; }
        public string? OrderEmail { get; private set; }

        private Order() { } // required for EF core or any other ORM

        public Order(Guid userId)
        {
            if (userId == Guid.Empty)
                throw new ArgumentException("UserId cannot be empty.", nameof(userId));

            UserId = userId;
            OrderPlaced = DateTime.UtcNow;
            OrderStatus = OrderStatus.Pending;
        }

        public void AddOrderLineItem(Guid productId, decimal price, int quantity)
        {
            if (productId == Guid.Empty)
                throw new ArgumentException("ProductId cannot be empty.", nameof(productId));

            if (quantity <= 0)
                throw new ArgumentException("Quantity should be greater than zero.", nameof(quantity));

            if (price <= 0)
                throw new ArgumentException("Price should be positive.", nameof(price));

            var orderLineItem = new OrderLineItem(price, productId, quantity);
            _orderLineItems.Add(orderLineItem);
        }

        public void AddOrderLineItems(IEnumerable<OrderLineItem> orderLineItems)
        {
            if (orderLineItems == null || !orderLineItems.Any())
                throw new ArgumentNullException(nameof(orderLineItems), "Order line items list cannot be null or empty.");

            foreach (var item in orderLineItems)
            {
                AddOrderLineItem(item.ProductId, item.UnitPrice, item.Quantity);
            }
        }

        public void ClearOrderLineItems()
        {
            _orderLineItems.Clear();
        }

        public void SetOrderStatus(OrderStatus status)
        {
            OrderStatus = status;
        }

        public void MarkAsPaid()
        {
            OrderPaid = true;
            PurchaseDate = DateTime.UtcNow;
        }

        private decimal CalculateOrderTotal()
        {
            return _orderLineItems.Sum(item => item.UnitPrice * item.Quantity);
        }
    }
}
