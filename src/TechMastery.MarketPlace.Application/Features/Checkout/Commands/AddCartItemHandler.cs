using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using TechMastery.MarketPlace.Application.Contracts.Persistence;
using TechMastery.MarketPlace.Domain.Entities;
using TechMastery.MarketPlace.Application.Features.Checkout.Dto;

namespace TechMastery.MarketPlace.Application.Features.Checkout.Handlers
{
    public class AddCartItem : IRequest<Guid>
    {
        public Guid ShoppingCartId { get; set; }
        public CartItemDto CartItem { get; set; }
    }

    public class AddCartItemHandler : IRequestHandler<AddCartItem, Guid>
    {
        private readonly ICartItemRepository _cartItemRepository;
        private readonly IShoppingCartRepository _shoppingCartRepository;
        private readonly ILogger<AddCartItemHandler> _logger;

        public AddCartItemHandler(
            ICartItemRepository cartItemRepository,
            IShoppingCartRepository shoppingCartRepository,
            ILogger<AddCartItemHandler> logger)
        {
            _cartItemRepository = cartItemRepository ?? throw new ArgumentNullException(nameof(cartItemRepository));
            _shoppingCartRepository = shoppingCartRepository ?? throw new ArgumentNullException(nameof(shoppingCartRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<Guid> Handle(AddCartItem command, CancellationToken cancellationToken)
        {
            ValidateCartItem(command.CartItem);

            var shoppingCart = await GetOrCreateShoppingCartAsync(command.ShoppingCartId);
            var newCartItem = new CartItem(command.CartItem.ProductId, command.CartItem.Price, command.CartItem.Quantity, shoppingCart.ShoppingCartId);
            await _cartItemRepository.AddAsync(newCartItem);

            _logger.LogInformation("Added new cart item. CartItemId: {CartItemId}", newCartItem.CartItemId);
            return newCartItem.CartItemId;
        }

        private void ValidateCartItem(CartItemDto cartItem)
        {
            if (cartItem == null)
            {
                _logger.LogError("Invalid cart item: CartItemDto cannot be null.");
                throw new ArgumentNullException(nameof(cartItem), "CartItemDto cannot be null.");
            }

            if (cartItem.ProductId == Guid.Empty)
            {
                _logger.LogError("Invalid cart item: ProductId cannot be empty.");
                throw new ArgumentException("ProductId cannot be empty.", nameof(cartItem.ProductId));
            }

            if (cartItem.Quantity <= 0)
            {
                _logger.LogError("Invalid cart item: Quantity should be greater than zero.");
                throw new ArgumentException("Quantity should be greater than zero.", nameof(cartItem.Quantity));
            }
        }

        private async Task<ShoppingCart> GetOrCreateShoppingCartAsync(Guid shoppingCartId)
        {
            var shoppingCart = await _shoppingCartRepository.GetByIdAsync(shoppingCartId) ?? new ShoppingCart(shoppingCartId);
            if (shoppingCart.ShoppingCartId == Guid.Empty)
            {
                _logger.LogInformation("Creating new shopping cart. ShoppingCartId: {ShoppingCartId}", shoppingCart.ShoppingCartId);
                await _shoppingCartRepository.AddAsync(shoppingCart);
            }
            return shoppingCart;
        }
    }
}
