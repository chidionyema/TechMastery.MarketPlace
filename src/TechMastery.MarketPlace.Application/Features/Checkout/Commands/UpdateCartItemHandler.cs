
using MediatR;
using Microsoft.Extensions.Logging;
using TechMastery.MarketPlace.Application.Exceptions;
using TechMastery.MarketPlace.Application.Persistence.Contracts;

namespace TechMastery.MarketPlace.Application.Features.Checkout.Handlers
{
    public class UpdateCartItem : IRequest<Unit>
    {
        public Guid ShoppingCartId { get; set; }
        public Guid CartItemId { get; set; }
        public int Quantity { get; set; }
    }

    public class UpdateCartItemHandler : IRequestHandler<UpdateCartItem, Unit>
    {
        private readonly ICartItemRepository _cartItemRepository;
        private readonly ILogger<UpdateCartItemHandler> _logger;

        public UpdateCartItemHandler(ICartItemRepository cartItemRepository, ILogger<UpdateCartItemHandler> logger)
        {
            _cartItemRepository = cartItemRepository ?? throw new ArgumentNullException(nameof(cartItemRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<Unit> Handle(UpdateCartItem request, CancellationToken cancellationToken)
        {
            if (request.Quantity <= 0)
            {
                _logger.LogError("Invalid quantity: Quantity should be greater than zero.");
                throw new ArgumentException("Quantity should be greater than zero.", nameof(request.Quantity));
            }

            var cartItem = await _cartItemRepository.GetByIdAsync(request.CartItemId);
            if (cartItem == null || cartItem.ShoppingCartId != request.ShoppingCartId)
            {
                _logger.LogError("Invalid cart item: Cart item not found or does not belong to the specified shopping cart.");
                throw new NotFoundException("Cart item not found or does not belong to the specified shopping cart.", 0);
            }

            cartItem.UpdateQuantity(request.Quantity);
            await _cartItemRepository.UpdateAsync(cartItem);

            _logger.LogInformation("Updated cart item. CartItemId: {CartItemId}", cartItem.Id);
            return Unit.Value;
        }
    }
}
