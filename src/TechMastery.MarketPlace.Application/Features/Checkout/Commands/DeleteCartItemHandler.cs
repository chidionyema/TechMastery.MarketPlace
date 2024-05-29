using System;
using MediatR;
using Microsoft.Extensions.Logging;
using TechMastery.MarketPlace.Application.Exceptions;
using TechMastery.MarketPlace.Application.Persistence.Contracts;
using TechMastery.MarketPlace.Domain.Entities;

namespace TechMastery.MarketPlace.Application.Features.Checkout.Commands
{
    public class DeleteCartItem : IRequest
    {
        public Guid CartItemId { get; set; }
    }

    public class DeleteCartItemHandler : IRequestHandler<DeleteCartItem>
    {
        private readonly ICartItemRepository _cartItemRepository;
        private readonly ILogger<DeleteCartItemHandler> _logger;

        public DeleteCartItemHandler(
            ICartItemRepository cartItemRepository,
            ILogger<DeleteCartItemHandler> logger)
        {
            _cartItemRepository = cartItemRepository ?? throw new ArgumentNullException(nameof(cartItemRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<Unit> Handle(DeleteCartItem command, CancellationToken cancellationToken)
        {
            try
            {
                var cartItem = await _cartItemRepository.GetByIdAsync(command.CartItemId);
                if (cartItem == null)
                {
                    throw new NotFoundException(nameof(CartItem), command.CartItemId);
                }

                await _cartItemRepository.DeleteAsync(cartItem);

                _logger.LogInformation("Successfully deleted cart item. CartItemId: {CartItemId}", cartItem.Id);
                return Unit.Value;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while processing the DeleteCartItem request.");
                throw; // Re-throw the exception to propagate it upwards
            }
        }
    }
}

