using System;
using TechMastery.MarketPlace.Domain.Entities;

namespace TechMastery.MarketPlace.Application.Features.Checkout.Dto
{
	public class CartItemDto
	{
        public Guid CartItemId { get; private set; }
        public Guid CartId { get; private set; }
        public Guid ProductId { get; private set; }
        public decimal Price { get; private set; }
        public int Quantity { get; private set; }
        public Guid UserId { get; internal set; }
        public string? ProductName { get; internal set; }

        public CartItemDto(CartItem cartItem)
        {
            if (cartItem == null)
            {
                throw new ArgumentNullException(nameof(cartItem));
            }

            CartItemId = cartItem.CartItemId;
            CartId = cartItem.ShoppingCartId;
            ProductId = cartItem.ProductId;
            Price = cartItem.Price;
            Quantity = cartItem.Quantity;
            ProductName = cartItem.ProductName;
        }


        public CartItemDto(Guid cartItemId, Guid cartId, Guid productId, decimal price, int quantity, Guid userId, string? productName)
        {
            CartItemId = cartItemId;
            CartId = cartId;
            ProductId = productId;
            Price = price;
            Quantity = quantity;
            UserId = userId;
            ProductName = productName;
        }

        public CartItemDto(Guid cartId, Guid productId)
        {
            CartId = cartId;
            ProductId = productId;
        }

        public void Validate()
        {
            if (ProductId == Guid.Empty)
            {
                throw new ArgumentException("Product ID cannot be empty.", nameof(ProductId));
            }

            if (Price <= 0)
            {
                throw new ArgumentException("Price must be greater than zero.", nameof(Price));
            }
        }
    }
}

