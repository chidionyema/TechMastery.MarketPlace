using MediatR;
using Microsoft.Extensions.Logging;
using TechMastery.MarketPlace.Application.Contracts.Persistence;
using TechMastery.MarketPlace.Domain.Entities;
using TechMastery.MarketPlace.Application.Features.Checkout.Dto;
using TechMastery.MarketPlace.Application.Exceptions;

namespace TechMastery.MarketPlace.Application.Features.Checkout.Handlers
{
    public class AddItemToCart : IRequest<List<Guid>>
    {
        public Guid ShoppingCartId { get; set; }
        public List<CartItemDto> CartItems { get; set; } = new List<CartItemDto>();
    }

    public class AddItemToCartHandler : IRequestHandler<AddItemToCart, List<Guid>>
    {
        private readonly ICartItemRepository _cartItemRepository;
        private readonly IShoppingCartRepository _shoppingCartRepository;
        private readonly ILogger<AddItemToCartHandler> _logger;

        public AddItemToCartHandler(ICartItemRepository cartItemRepository, IShoppingCartRepository shoppingCartRepository, ILogger<AddItemToCartHandler> logger)
        {
            _cartItemRepository = cartItemRepository ?? throw new ArgumentNullException(nameof(cartItemRepository));
            _shoppingCartRepository = shoppingCartRepository ?? throw new ArgumentNullException(nameof(shoppingCartRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<List<Guid>> Handle(AddItemToCart command, CancellationToken cancellationToken)
        {
            if (command.CartItems == null || !command.CartItems.Any())
            {
                _logger.LogError("Invalid request: {Command}", nameof(command));
                throw new BadRequestException(nameof(command));
            }

            var cartItemIds = new List<Guid>();
            var shoppingCart = await GetOrCreateShoppingCartAsync(command.ShoppingCartId);

            foreach (var cartItem in command.CartItems)
            {
                ValidateCartItem(cartItem);
                var cartItemId = await AddOrUpdateCartItemAsync(shoppingCart, cartItem);
                cartItemIds.Add(cartItemId);
            }

            _logger.LogInformation("Successfully added items to cart. ShoppingCartId: {ShoppingCartId}, CartItemIds: {CartItemIds}",
                shoppingCart.ShoppingCartId, string.Join(",", cartItemIds));
            return cartItemIds;

        }

        private void ValidateCartItem(CartItemDto cartItem)
        {
            if (cartItem == null)
            {
                _logger.LogError("Invalid cart item: CartItemDto cannot be null.");
                throw new ArgumentNullException(nameof(cartItem), "CartItemDto cannot be null.");
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

        private async Task<Guid> AddOrUpdateCartItemAsync(ShoppingCart shoppingCart, CartItemDto cartItem)
        {
            var existingCartItem = shoppingCart.Items.SingleOrDefault(ci => ci.ProductId == cartItem.ProductId);
            if (existingCartItem != null)
            {
                existingCartItem.UpdateQuantity(cartItem.Quantity);
                await _cartItemRepository.UpdateAsync(existingCartItem);
                _logger.LogInformation("Updated cart item. CartItemId: {CartItemId}", existingCartItem.CartItemId);
                return existingCartItem.CartItemId;
            }
            else
            {
                var newCartItem = new CartItem(cartItem.ProductId, cartItem.Price, cartItem.Quantity, shoppingCart.ShoppingCartId);
                await _cartItemRepository.AddAsync(newCartItem);
                _logger.LogInformation("Added new cart item. CartItemId: {CartItemId}", newCartItem.CartItemId);
                return newCartItem.CartItemId;
            }
        }
    }
}
