using TechMastery.MarketPlace.Application.Features.Checkout.Dto;

namespace TechMastery.MarketPlace.Application.Tests.Integration
{
    internal class CartItemDtoBuilder
    {
        private Guid _cartItemId = Guid.NewGuid();
        private Guid _shoppingCartId = Guid.NewGuid();
        private Guid _productId = Guid.NewGuid();
        private decimal _price = 10.99m;
        private int _quantity = 2;
        private Guid _productOptionId = Guid.NewGuid();
        private string _productName = "Sample Product";

        internal static CartItemDtoBuilder Create() => new CartItemDtoBuilder();

        internal CartItemDtoBuilder WithCartItem(CartItemDto cartItem)
        {
            _cartItemId = cartItem.CartItemId;
            _shoppingCartId = cartItem.CartId;
            _productId = cartItem.ProductId;
            _price = cartItem.Price;
            _quantity = cartItem.Quantity;
            _productName = cartItem.ProductName;
            return this;
        }

        internal CartItemDtoBuilder WithShoppingCartId(Guid shoppingCartId)
        {
            _shoppingCartId = shoppingCartId;
            return this;
        }

        internal CartItemDtoBuilder WithProductId(Guid productId)
        {
            _productId = productId;
            return this;
        }

        internal CartItemDtoBuilder WithPrice(decimal price)
        {
            _price = price;
            return this;
        }

        internal CartItemDtoBuilder WithQuantity(int quantity)
        {
            _quantity = quantity;
            return this;
        }

        internal CartItemDtoBuilder WithProductOptionId(Guid productOptionId)
        {
            _productOptionId = productOptionId;
            return this;
        }

        internal CartItemDtoBuilder WithProductName(string productName)
        {
            _productName = productName;
            return this;
        }

        internal CartItemDto Build()
        {
            return new CartItemDto(_cartItemId, _shoppingCartId, _productId, _price, _quantity, _productOptionId, _productName);
        }
    }
}
