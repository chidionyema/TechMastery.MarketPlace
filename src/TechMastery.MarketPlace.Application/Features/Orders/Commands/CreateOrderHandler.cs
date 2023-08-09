using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
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
            try
            {
                ValidateCommand(command);

                var shoppingCart = await GetValidShoppingCartAsync(command.CartId);

                var order = CreateOrderFromShoppingCart(shoppingCart);

                await UpdateShoppingCartAndReturnOrderId(shoppingCart, order);

                return order.OrderId;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while processing CreateOrder command.");
                throw; // Re-throw the exception to preserve the original exception stack trace.
            }
        }

        private static void ValidateCommand(CreateOrder command)
        {
            if (command == null)
            {
                throw new BadRequestException(nameof(command));
            }
        }

        private async Task<ShoppingCart> GetValidShoppingCartAsync(Guid cartId)
        {
            var shoppingCart = await _shoppingCartRepository.GetByIdAsync(cartId);
            if (shoppingCart == null)
            {
                throw new NotFoundException("Shopping cart not found.", cartId);
            }
            return shoppingCart;
        }

        private Order CreateOrderFromShoppingCart(ShoppingCart shoppingCart)
        {
            var orderLineItems = ToOrderLineItems(shoppingCart.Items.ToList());
            return new Order(shoppingCart.UserId)
            {
                OrderLineItems = orderLineItems
            };
        }

        private List<OrderLineItem> ToOrderLineItems(List<CartItem> cartItems)
        {
            return cartItems.Select(CreateOrderLineItemFromCartItem).ToList();
        }

        private OrderLineItem CreateOrderLineItemFromCartItem(CartItem cartItem)
        {
            return new OrderLineItem(cartItem.Price, cartItem.ProductId, cartItem.Quantity, cartItem.ProductName);
        }

        private async Task UpdateShoppingCartAndReturnOrderId(ShoppingCart shoppingCart, Order order)
        {
           // using (var transaction = _shoppingCartRepository.BeginTransaction()) // Assuming IShoppingCartRepository supports transactions
            //{
                shoppingCart.SetStatus(ShoppingCartStatus.InOrderState);
                await _shoppingCartRepository.UpdateAsync(shoppingCart);
                await _orderRepository.AddAsync(order);

               // transaction.Commit(); // Commit the transaction if everything succeeds
           // }
        }
    }
}
