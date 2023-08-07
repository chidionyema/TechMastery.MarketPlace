using TechMastery.MarketPlace.Application.Features.Checkout.Dto;
using TechMastery.MarketPlace.Application.Features.Checkout.Handlers;

namespace TechMastery.MarketPlace.Application.Tests.Integration
{
    internal class AddItemToCartCommandBuilder
    {
        private List<CartItemDto> _cartItems;

        internal static AddItemToCartCommandBuilder Create() => new AddItemToCartCommandBuilder();

        internal AddItemToCartCommandBuilder WithCartItem(CartItemDto cartItem)
        {
            _cartItems = new List<CartItemDto> { cartItem };
            return this;
        }

        internal AddItemToCartCommandBuilder WithCartItems(List<CartItemDto> cartItems)
        {
            _cartItems = cartItems;
            return this;
        }

        internal AddItemToCart Build()
        {
            return new AddItemToCart { CartItems = _cartItems };
        }
    }
}
