using MediatR;
using Microsoft.Extensions.Logging;
using TechMastery.MarketPlace.Domain.Entities;
using TechMastery.MarketPlace.Application.Exceptions;
using TechMastery.MarketPlace.Application.Persistence.Contracts;

namespace TechMastery.MarketPlace.Application.Features.Orders.Commands
{
    public class CreateOrderFromCart : IRequest<Guid>
    {
        public Guid CartId { get; set; }
    }

    public class CreateOrderFromCartHandler : IRequestHandler<CreateOrderFromCart, Guid>
    {
        private readonly IShoppingCartRepository _shoppingCartRepository;
        private readonly IOrderRepository _orderRepository;
        private readonly ILogger<CreateOrderFromCartHandler> _logger;

        public CreateOrderFromCartHandler(IShoppingCartRepository shoppingCartRepository, IOrderRepository orderRepository, ILogger<CreateOrderFromCartHandler> logger)
        {   
            _shoppingCartRepository = shoppingCartRepository ?? throw new ArgumentNullException(nameof(shoppingCartRepository));
            _orderRepository = orderRepository ?? throw new ArgumentNullException(nameof(orderRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<Guid> Handle(CreateOrderFromCart command, CancellationToken cancellationToken)
        {
            ValidateCommand(command);

            var shoppingCart = await GetValidShoppingCartAsync(command.CartId);

            var orderFromCart = CreateOrderFromShoppingCart(shoppingCart);

            await UpdateShoppingCartAndPersistOrderAsync(shoppingCart, orderFromCart);

            _logger.LogInformation("Order created successfully with OrderId: {OrderId}", orderFromCart.Id);
            return orderFromCart.Id;
        }

        private void ValidateCommand(CreateOrderFromCart command)
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

        private async Task UpdateShoppingCartAndPersistOrderAsync(ShoppingCart shoppingCart, Order order)
        {
            await _orderRepository.ExecuteWithinTransactionAsync(async () =>
            {
                shoppingCart.SetStatus(ShoppingCartStatus.InOrderState);
                await _shoppingCartRepository.UpdateAsync(shoppingCart);
                await _orderRepository.AddAsync(order);
            });

        }
    }
}
