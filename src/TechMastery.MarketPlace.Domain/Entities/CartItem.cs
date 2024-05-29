using TechMastery.MarketPlace.Domain.Common;

namespace TechMastery.MarketPlace.Domain.Entities
{
    public class CartItem : AuditableEntity
    {
        public Guid ShoppingCartId { get; private set; }       
        public Guid ProductId { get; private set; }
        public string? ProductName { get; private set; }
        public decimal Price { get; private set; }
        public int Quantity { get; private set; }

        public CartItem(Guid productId, decimal price, int quantity, Guid shoppingCartId)
        {
            ProductId = productId;
            Quantity = quantity;
            Price = price;
            ShoppingCartId = shoppingCartId;
        }

        public void IncreaseQuantity(int quantity)
        {
            Quantity += quantity;
        }

        public void UpdateQuantity(int quantity)
        {
            Quantity = quantity;
        }

        public static CartItem Create(Guid productId, decimal price, int quantity, Guid shoppingCartId)
        {
            return new CartItem(productId, price, quantity, shoppingCartId);
        }
    }
}