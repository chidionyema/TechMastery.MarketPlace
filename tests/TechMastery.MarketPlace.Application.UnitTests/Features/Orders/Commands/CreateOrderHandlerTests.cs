using Microsoft.Extensions.Logging;
using Moq;
using TechMastery.MarketPlace.Application.Exceptions;
using TechMastery.MarketPlace.Application.Features.Orders.Commands;
using TechMastery.MarketPlace.Application.Persistence.Contracts;
using TechMastery.MarketPlace.Application.Tests.Integration;
using TechMastery.MarketPlace.Domain.Entities;

namespace TechMastery.MarketPlace.Application.Tests.Features.Orders.Commands
{
    public class CreateOrderHandlerTests : IClassFixture<ApplicationTestFixture>
    {
        private readonly ApplicationTestFixture _fixture;
        private readonly IShoppingCartRepository _shoppingCartRepository;
        private readonly IOrderRepository _orderRepository;
        private readonly Mock<ILogger<CreateOrderFromCartHandler>> _mockLogger;

        public CreateOrderHandlerTests(ApplicationTestFixture fixture)
        {
            _fixture = fixture;
            _shoppingCartRepository = _fixture.CreateCartRepository();
            _orderRepository = _fixture.CreateOrderRepository();
            _mockLogger = new Mock<ILogger<CreateOrderFromCartHandler>>();
        }

        [Fact]
        public async Task Handle_NullCommand_ShouldThrowArgumentNullException()
        {
            // Arrange
            var handler = new CreateOrderFromCartHandler(_shoppingCartRepository, _orderRepository, _mockLogger.Object);

            // Act & Assert
            await Assert.ThrowsAsync<BadRequestException>(() => handler.Handle(null, CancellationToken.None));
        }

        [Fact]
        public async Task Handle_ValidCommand_ShouldCreateOrderAndReturnOrderId()
        {
            // Arrange
            var cartItems = CartItemBuilder.CreateListOfSize(2);

            var shoppingCart = ShoppingCartBuilder.Create()
                .WithCartItems(cartItems)
                .Build();

            await _shoppingCartRepository.AddAsync(shoppingCart);

            var handler = new CreateOrderFromCartHandler(_shoppingCartRepository, _orderRepository, _mockLogger.Object);

            var command = new CreateOrderFromCart { CartId = shoppingCart.Id };

            // Act
            var orderId = await handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.NotEqual(Guid.Empty, orderId);
            var createdOrder = await _orderRepository.GetByIdAsync(orderId);
            Assert.NotNull(createdOrder);
            Assert.Equal(cartItems.Count(), createdOrder.OrderLineItems.Count);
            Assert.Equal(cartItems.Sum(item => item.Quantity), createdOrder.OrderLineItems.Sum(item => item.Quantity));

            var expectedTotal = cartItems.Sum(item => item.Price * item.Quantity);
            Assert.Equal(expectedTotal, createdOrder.OrderTotal);
        }

        [Fact]
        public async Task Handle_ValidCommand_ShouldAddOrderLineItemsWithCorrectValues()
        {
            // Arrange
            var cartItems = CartItemBuilder.CreateListOfSize(2);
            var shoppingCart = ShoppingCartBuilder.Create()
                .WithCartItems(cartItems)
                .Build();

            await _shoppingCartRepository.AddAsync(shoppingCart);

            var handler = new CreateOrderFromCartHandler(_shoppingCartRepository, _orderRepository, _mockLogger.Object);
            var command = new CreateOrderFromCart { CartId = shoppingCart.Id };

            // Act
            var orderId = await handler.Handle(command, CancellationToken.None);

            // Assert
            var createdOrder = await _orderRepository.GetByIdAsync(orderId);
            Assert.Equal(cartItems.Count, createdOrder?.OrderLineItems.Count);

            foreach (var cartItem in cartItems)
            {
                var matchingOrderLineItem = createdOrder?.OrderLineItems.FirstOrDefault(oli => oli.ProductId == cartItem.ProductId);
                Assert.NotNull(matchingOrderLineItem);
                Assert.Equal(cartItem.Quantity, matchingOrderLineItem.Quantity);
                Assert.Equal(cartItem.Price, matchingOrderLineItem.UnitPrice);
            }
        }

        [Fact]
        public async Task Handle_ValidCommand_ShouldUpdateShoppingCartStatus()
        {
            // Arrange
            var cartItems = CartItemBuilder.CreateListOfSize(2);

            var shoppingCart = ShoppingCartBuilder.Create()
                .WithCartItems(cartItems)
                .Build();
            await _shoppingCartRepository.AddAsync(shoppingCart);

            var handler = new CreateOrderFromCartHandler(_shoppingCartRepository, _orderRepository, _mockLogger.Object);
            var command = new CreateOrderFromCart { CartId = shoppingCart.Id };

            // Act
            await handler.Handle(command, CancellationToken.None);

            // Assert
            var updatedShoppingCart = await _shoppingCartRepository.GetByIdAsync(shoppingCart.Id);
            Assert.Equal(ShoppingCartStatus.InOrderState, updatedShoppingCart?.Status);
        }

        [Fact]
        public async Task Handle_CartNotFound_ShouldThrowApplicationException()
        {
            // Arrange
            var cartId = Guid.NewGuid();
            var command = new CreateOrderFromCart { CartId = cartId };

            var handler = new CreateOrderFromCartHandler(_shoppingCartRepository, _orderRepository, _mockLogger.Object);

            // Act & Assert
            await Assert.ThrowsAsync<ApplicationException>(() => handler.Handle(command, CancellationToken.None));
        }
    }
}
