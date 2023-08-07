using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TechMastery.MarketPlace.Application.Contracts.Persistence;
using TechMastery.MarketPlace.Domain.Entities;
using TechMastery.MarketPlace.Application.Features.Checkout.Dto;
using TechMastery.MarketPlace.Application.Exceptions;

namespace TechMastery.MarketPlace.Application.Features.Checkout.Handlers
{
    public class AddItemToCart : IRequest<List<Guid>>
    {
        public Guid ShoppingCartId { get; set; }
        public required List<CartItemDto> CartItems { get; set; }
    }

    public class AddItemToCartHandler : IRequestHandler<AddItemToCart, List<Guid>>
    {
        private readonly ICartItemRepository _cartItemRepository;
        private readonly IShoppingCartRepository _shoppingCartRepository;

        public AddItemToCartHandler(ICartItemRepository cartItemRepository, IShoppingCartRepository shoppingCartRepository)
        {
            _cartItemRepository = cartItemRepository ?? throw new ArgumentNullException(nameof(cartItemRepository));
            _shoppingCartRepository = shoppingCartRepository ?? throw new ArgumentNullException(nameof(shoppingCartRepository));
        }

        public async Task<List<Guid>> Handle(AddItemToCart command, CancellationToken cancellationToken)
        {
            if (command == null || !command.CartItems.Any())
            {
                throw new BadRequestException(nameof(command));
            }

            var cartItemIds = new List<Guid>();

            var shoppingCart = await GetOrCreateShoppingCart(command.ShoppingCartId);

            foreach (var cartItem in command.CartItems)
            {
                ValidateCartItem(cartItem);

                var cartItemId = await AddOrUpdateCartItem(shoppingCart, cartItem);
                cartItemIds.Add(cartItemId);
            }

            return cartItemIds;
        }

        private void ValidateCartItem(CartItemDto cartItem)
        {
            if (cartItem == null)
            {
                throw new ArgumentNullException(nameof(cartItem), "CartItemDto cannot be null.");
            }
        }

        private async Task<ShoppingCart> GetOrCreateShoppingCart(Guid shoppingCartId)
        {
            var shoppingCart = await _shoppingCartRepository.GetByIdAsync(shoppingCartId);

            if (shoppingCart == null)
            {
                shoppingCart = new ShoppingCart(shoppingCartId);
                await _shoppingCartRepository.AddAsync(shoppingCart);
            }

            return shoppingCart;
        }

        private async Task<Guid> AddOrUpdateCartItem(ShoppingCart shoppingCart, CartItemDto cartItem)
        {
            var existingCartItem = shoppingCart.FindCartItem(cartItem.ProductId);

            if (existingCartItem != null)
            {
                existingCartItem.UpdateQuantity(cartItem.Quantity);
                await _cartItemRepository.UpdateAsync(existingCartItem);
                return existingCartItem.CartItemId;
            }
            else
            {
                var newCartItem = new CartItem(cartItem.ProductId, cartItem.Price, cartItem.Quantity, shoppingCart.ShoppingCartId);
                await _cartItemRepository.AddAsync(newCartItem);
                return newCartItem.CartItemId;
            }
        }
    }
}
