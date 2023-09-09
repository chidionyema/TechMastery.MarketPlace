using TechMastery.MarketPlace.Domain.Entities;

namespace TechMastery.MarketPlace.Application.Tests.Integration
{
    internal class CartItemBuilder
    {
        private Guid _cartItemId = Guid.NewGuid();
        private decimal _price = 10.99m;
        private int _quantity = 2;
        private Guid _shoppingCartId = Guid.NewGuid();
        private string _productName = "Sample Product";

        internal static CartItemBuilder Create() => new CartItemBuilder();

        internal CartItemBuilder WithCartItemId(Guid cartItemId)
        {
            _cartItemId = cartItemId;
            return this;
        }

        internal CartItemBuilder WithPrice(decimal price)
        {
            _price = price;
            return this;
        }

        internal CartItemBuilder WithQuantity(int quantity)
        {
            _quantity = quantity;
            return this;
        }

        internal CartItemBuilder WithShoppingCartId(Guid shoppingCartId)
        {
            _shoppingCartId = shoppingCartId;
            return this;
        }

        internal CartItemBuilder WithProductName(string productName)
        {
            _productName = productName;
            return this;
        }

        internal static List<CartItem> CreateListOfSize(int size)
        {
            var cartItems = new List<CartItem>();

            for (var i = 0; i < size; i++)
            {
                cartItems.Add(CartItemBuilder.Create().Build());
            }

            return cartItems;
        }


        internal CartItem Build()
        {
            return CartItem.Create(_cartItemId, _price, _quantity, _shoppingCartId);
        }

    }
}
