using MediatR;
using Microsoft.Extensions.Logging;
using TechMastery.MarketPlace.Application.Contracts.Persistence;
using TechMastery.MarketPlace.Domain.Entities;
using TechMastery.MarketPlace.Application.Exceptions;

namespace TechMastery.MarketPlace.Application.Features.Orders.Commands
{
    public class CreateOrder : IRequest<Guid>
    {
        public Guid CartId { get; set; }
    }

    public class CreateOrderHandler : IRequestHandler<CreateOrder, Guid>
    {
        private readonly IShoppingCartRepository _shoppingCartRepository;
        private readonly IOrderRepository _orderRepository;
        private readonly ILogger<CreateOrderHandler> _logger;

        public CreateOrderHandler(IShoppingCartRepository shoppingCartRepository, IOrderRepository orderRepository, ILogger<CreateOrderHandler> logger)
        {
            _shoppingCartRepository = shoppingCartRepository ?? throw new ArgumentNullException(nameof(shoppingCartRepository));
            _orderRepository = orderRepository ?? throw new ArgumentNullException(nameof(orderRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<Guid> Handle(CreateOrder command, CancellationToken cancellationToken)
        {
            ValidateCommand(command);

            var shoppingCart = await GetValidShoppingCartAsync(command.CartId);

            var order = CreateOrderFromShoppingCart(shoppingCart);

            await UpdateShoppingCartAndPersistOrder(shoppingCart, order);

            _logger.LogInformation("Order created successfully with OrderId: {OrderId}", order.OrderId);
            return order.OrderId;
        }

        private void ValidateCommand(CreateOrder command)
        {
            if (command == null || command.CartId == Guid.Empty)
            {
                _logger.LogError("Invalid CreateOrder command received.");
                throw new BadRequestException("Invalid command details.");
            }
        }

        private async Task<ShoppingCart> GetValidShoppingCartAsync(Guid cartId)
        {
            var shoppingCart = await _shoppingCartRepository.GetByIdAsync(cartId);
            if (shoppingCart == null || !shoppingCart.Items.Any())
            {
                _logger.LogError("Shopping cart not found or empty for CartId: {CartId}", cartId);
                throw new NotFoundException($"Shopping cart with ID {cartId} not found or is empty.", cartId);
            }
            return shoppingCart;
        }

        private Order CreateOrderFromShoppingCart(ShoppingCart shoppingCart)
        {
            var order = new Order(shoppingCart.UserId);
            shoppingCart.Items.ToList().ForEach(cartItem =>
            {
                var orderLineItem = CreateOrderLineItemFromCartItem(cartItem);
                order.AddOrderLineItem(orderLineItem.ProductId, orderLineItem.UnitPrice, orderLineItem.Quantity);
            });
            return order;
        }

        private OrderLineItem CreateOrderLineItemFromCartItem(CartItem cartItem)
        {
            return new OrderLineItem(cartItem.Price, cartItem.ProductId, cartItem.Quantity);
        }

        private async Task UpdateShoppingCartAndPersistOrder(ShoppingCart shoppingCart, Order order)
        {
          //  using (var transaction = await _shoppingCartRepository.BeginTransactionAsync()) // Assuming IShoppingCartRepository supports async transactions
           // {
                try
                {
                    shoppingCart.SetStatus(ShoppingCartStatus.InOrderState); // Assuming a method like this exists on ShoppingCart.
                    await _shoppingCartRepository.UpdateAsync(shoppingCart);
                    await _orderRepository.AddAsync(order);

                  //  await transaction.CommitAsync(); // Commit the transaction if everything succeeds
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "An error occurred while updating the shopping cart and persisting the order.");
                  //  await transaction.RollbackAsync();
                    throw;
                }
           // }
        }
    }
}
