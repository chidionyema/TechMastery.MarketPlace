using TechMastery.MarketPlace.Domain.Common;

namespace TechMastery.MarketPlace.Domain.Entities
{
    public class ShoppingCart : AuditableEntity
    {
        public Guid ShoppingCartId { get; private set; }
        public Guid UserId { get; private set; }
        public ShoppingCartStatus Status { get; private set; }
        private List<CartItem> _items = new List<CartItem>();
        public IReadOnlyCollection<CartItem> Items => _items.AsReadOnly();

        public ShoppingCart(Guid userId)
        {
            ShoppingCartId = Guid.NewGuid();
            UserId = userId;
            Status = ShoppingCartStatus.Pending;
        }

        public ShoppingCart(Guid cartId, Guid userId)
        {
            ShoppingCartId = cartId;
            UserId = userId;
            Status = ShoppingCartStatus.Pending;
        }

        public void AddItem(Guid productId, int quantity, decimal price)
        {
            var existingItem = _items.Find(item => item.ProductId == productId);

            if (existingItem != null)
            {
                existingItem.IncreaseQuantity(quantity);
            }
            else
            {
                var newItem = CartItem.Create(productId, price, quantity, ShoppingCartId);
                _items.Add(newItem);
            }
        }

        public void UpdateItemQuantity(Product product, int quantity)
        {
            var existingItem = _items.Find(item => item.ProductId == product.ProductId);

            if (existingItem != null)
            {
                existingItem.UpdateQuantity(quantity);
            }
        }

        public void RemoveItem(Product product)
        {
            var existingItem = _items.Find(item => item.ProductId == product.ProductId);

            if (existingItem != null)
            {
                _items.Remove(existingItem);
            }
        }

        public CartItem? FindCartItem(Guid productId)
        {
            return _items.Find(item => item.ProductId == productId);
        }

        public void SetStatus(ShoppingCartStatus status)
        {
            Status = status;
        }

        public void SetCartItems(List<CartItem> cartItems)
        {
            _items = cartItems;
        }
    }

}

