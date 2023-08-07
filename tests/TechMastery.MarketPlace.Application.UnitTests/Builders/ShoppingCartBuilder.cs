using System;
using System.Collections.Generic;
using TechMastery.MarketPlace.Domain.Entities;

namespace TechMastery.MarketPlace.Application.Tests.Features.Orders.Commands
{
    internal class ShoppingCartBuilder
    {
        private Guid _cartId = Guid.NewGuid();
        private Guid _userId = Guid.NewGuid();
        private List<CartItem> _cartItems = new List<CartItem>();
        private ShoppingCartStatus _status = ShoppingCartStatus.Active;

        internal static ShoppingCartBuilder Create() => new ShoppingCartBuilder();

        internal ShoppingCartBuilder WithCartId(Guid cartId)
        {
            _cartId = cartId;
            return this;
        }

        internal ShoppingCartBuilder WithUserId(Guid userId)
        {
            _userId = userId;
            return this;
        }

        internal ShoppingCartBuilder WithCartItems(List<CartItem> cartItems)
        {
            _cartItems = cartItems;
            return this;
        }

        internal ShoppingCartBuilder WithStatus(ShoppingCartStatus status)
        {
            _status = status;
            return this;
        }

        internal ShoppingCart Build()
        {
            var shoppingCart = new ShoppingCart(_cartId, _userId);
            shoppingCart.SetCartItems(_cartItems);
            shoppingCart.SetStatus(_status);
            return shoppingCart;
        }
    }
}
