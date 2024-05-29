using MediatR;
using Microsoft.Extensions.Logging;
using TechMastery.MarketPlace.Domain.Entities;
using TechMastery.MarketPlace.Application.Features.Checkout.Dto;
using FluentValidation;
using TechMastery.MarketPlace.Application.Exceptions;
using ValidationException = TechMastery.MarketPlace.Application.Exceptions.ValidationException;
using TechMastery.MarketPlace.Application.Persistence.Contracts;

namespace TechMastery.MarketPlace.Application.Features.Checkout.Handlers
{
    public class AddCartItem : IRequest<Guid>
    {
        public AddCartItem (Guid userId)
        {
            UserId = userId;
        }

        public AddCartItem(Guid userId, CartItemDto? cartItem)
        {
            UserId = userId;
            CartItem = cartItem;
        }

        public Guid UserId { get; private set; }
        public CartItemDto? CartItem { get; private set; }
    }

    public class AddCartItemHandler : IRequestHandler<AddCartItem, Guid>
    {
        private readonly ICartItemRepository _cartItemRepository;
        private readonly IShoppingCartRepository _shoppingCartRepository;
        private readonly ILogger<AddCartItemHandler> _logger;
        private readonly IValidator<CartItemDto> _cartItemValidator;
        public AddCartItemHandler(
            ICartItemRepository cartItemRepository,
            IShoppingCartRepository shoppingCartRepository,
            ILogger<AddCartItemHandler> logger,
            IValidator<CartItemDto> cartItemValidator)
        {
            _cartItemRepository = cartItemRepository ?? throw new ArgumentNullException(nameof(cartItemRepository));
            _shoppingCartRepository = shoppingCartRepository ?? throw new ArgumentNullException(nameof(shoppingCartRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _cartItemValidator = cartItemValidator;
        }

        public async Task<Guid> Handle(AddCartItem command, CancellationToken cancellationToken)
        {
            if (command.CartItem == null)
            {     
                throw new BadRequestException($"command.CartItem is empty" + command.CartItem);
            }

            ValidateCartItem(command.CartItem);

            var shoppingCart = await GetOrCreateShoppingCartAsync(command.UserId);
            var newCartItem = new CartItem(command.CartItem.ProductId, command.CartItem.Price, command.CartItem.Quantity, shoppingCart.Id);
            await _cartItemRepository.AddAsync(newCartItem);

            _logger.LogInformation("Added new cart item. CartItemId: {CartItemId}", newCartItem.Id);
            return newCartItem.Id;
        }

        private void ValidateCartItem(CartItemDto cartItem)
        {
            // VALIDATE incoming price
            var validationResult = _cartItemValidator.Validate(cartItem);
            if (!validationResult.IsValid)
            {
                _logger.LogError("Invalid cart item. Errors: {ValidationErrors}", string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage)));
                throw new ValidationException(validationResult);
            }
        }

        private async Task<ShoppingCart> GetOrCreateShoppingCartAsync(Guid userId)
        {
            var shoppingCart = await _shoppingCartRepository.GetByUserIdAsync(userId) ?? new ShoppingCart(userId);
            if (shoppingCart.Id == Guid.Empty)
            {
                _logger.LogInformation("Creating new shopping cart. userId: {userId}", userId);
                await _shoppingCartRepository.AddAsync(shoppingCart);
            }
            return shoppingCart;
        }
    }
}
