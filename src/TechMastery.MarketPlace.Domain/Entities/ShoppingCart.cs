using TechMastery.MarketPlace.Domain.Common;

namespace TechMastery.MarketPlace.Domain.Entities
{
    /// <summary>
    /// Represents a shopping cart entity.
    /// </summary>
    public class ShoppingCart : AuditableEntity
    {
        public Guid Id { get; private set; }
        public Guid UserId { get; private set; }
        public ShoppingCartStatus Status { get; private set; }

        private readonly List<CartItem> _items = new List<CartItem>();
        public IReadOnlyCollection<CartItem> Items => _items.AsReadOnly();

        public ShoppingCart(Guid userId)
        {
            ValidateGuid(userId, nameof(userId));

            Id = Guid.NewGuid();
            UserId = userId;
            Status = ShoppingCartStatus.Pending;
        }

        public ShoppingCart(Guid cartId, Guid userId) : this(userId)
        {
            ValidateGuid(cartId, nameof(cartId));

            Id = cartId;
        }

        public void AddItem(Guid productId, int quantity, decimal price)
        {
            ValidateGuid(productId, nameof(productId));
            ValidateQuantity(quantity);

            var existingItem = FindCartItemById(productId);

            if (existingItem != null)
            {
                existingItem.IncreaseQuantity(quantity);
            }
            else
            {
                var newItem = CartItem.Create(productId, price, quantity, Id);
                _items.Add(newItem);
            }
        }

        public void UpdateItemQuantity(Guid productId, int quantity)
        {
            ValidateQuantity(quantity);

            var existingItem = FindCartItemById(productId);

            existingItem?.UpdateQuantity(quantity);
        }

        public void RemoveItem(Guid productId)
        {
            var existingItem = FindCartItemById(productId);

            if (existingItem != null)
            {
                _items.Remove(existingItem);
            }
        }

        private CartItem? FindCartItemById(Guid productId)
        {
            return _items.SingleOrDefault(item => item.ProductId == productId);
        }

        public void SetStatus(ShoppingCartStatus status)
        {
            // Additional validations for status can be added here if necessary.
            Status = status;
        }

        public void SetCartItems(List<CartItem> cartItems)
        {
            if (cartItems == null || !cartItems.Any()) return;

            _items.Clear();
            _items.AddRange(cartItems);
        }

        private void ValidateGuid(Guid id, string argumentName)
        {
            if (id == Guid.Empty)
                throw new ArgumentException($"{argumentName} cannot be empty.", argumentName);
        }

        private void ValidateQuantity(int quantity)
        {
            if (quantity <= 0)
                throw new ArgumentException("Quantity should be greater than zero.", nameof(quantity));
        }
    }
}
