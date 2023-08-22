using TechMastery.MarketPlace.Application.Contracts.Persistence;
using TechMastery.MarketPlace.Application.Features.Checkout.Handlers;
using Microsoft.Extensions.Logging;
using Moq;
using TechMastery.MarketPlace.Application.Validators;

namespace TechMastery.MarketPlace.Application.Tests.Integration
{
    public class AddToCartHandlerTests : IClassFixture<ApplicationTestFixture>
    {
        private readonly ApplicationTestFixture _fixture;
        private readonly IShoppingCartRepository _shoppingCartRepository;
        private readonly ICartItemRepository _cartItemRepository;
        private readonly AddCartItemHandler handler;

        public AddToCartHandlerTests(ApplicationTestFixture fixture)
        {
            _fixture = fixture;
            _shoppingCartRepository = _fixture.CreateCartRepository();
            _cartItemRepository = _fixture.CreateCartItemRepository();
            handler =  new AddCartItemHandler(_cartItemRepository, _shoppingCartRepository, new Mock<ILogger<AddCartItemHandler>>().Object, new CartItemDtoValidator());
        }

        [Fact]
        public async Task AddCommand_ShouldCreateNewCartItem()
        {
            var cartItemDto = CartItemDtoBuilder.Create()
                .Build();

            var command = AddItemToCartCommandBuilder.Create()
                .WithCartItem(cartItemDto)
                .Build();

            var cartItemIds = await handler.Handle(command, CancellationToken.None);

            Assert.NotEqual(Guid.Empty, cartItemIds);
        }

        [Fact]
        public async Task AddCommand_ShouldHandleNullCartItem()
        {
            var command = AddItemToCartCommandBuilder.Create()
                .WithCartItems(null)
                .Build();

            
            await Assert.ThrowsAsync<ArgumentNullException>(() => handler.Handle(command, CancellationToken.None));
        }

        [Fact]
        public async Task AddCommand_ShouldHandleNullShoppingCart()
        {
            var cartItemDto = CartItemDtoBuilder.Create()
                .Build();

            var command = AddItemToCartCommandBuilder.Create()
                .WithCartItem(cartItemDto)
                .Build();

            
            await Assert.ThrowsAsync<ArgumentNullException>(() => handler.Handle(command, CancellationToken.None));
        }

        [Fact]
        public async Task AddCommand_ShouldHandleDuplicateCartItem()
        {
            var existingCartItem = CartItemBuilder.Create()
                .Build();

            await _cartItemRepository.AddAsync(existingCartItem);

            var cartItemDto = CartItemDtoBuilder.Create()
                .WithShoppingCartId(existingCartItem.ShoppingCartId)
                .WithProductId(existingCartItem.ProductId)
                .Build();

            var command = AddItemToCartCommandBuilder.Create()
                .WithCartItem(cartItemDto)
                .Build();

            
            await Assert.ThrowsAsync<InvalidOperationException>(() => handler.Handle(command, CancellationToken.None));
        }

        [Fact]
        public async Task AddCommand_ShouldHandleZeroQuantity()
        {
            var cartItemDto = CartItemDtoBuilder.Create()
                .WithQuantity(0)
                .Build();

            var command = AddItemToCartCommandBuilder.Create()
                .WithCartItem(cartItemDto)
                .Build();

            
            await Assert.ThrowsAsync<ArgumentException>(() => handler.Handle(command, CancellationToken.None));
        }

        [Fact]
        public async Task AddCommand_ShouldHandleNegativeQuantity()
        {
            var cartItemDto = CartItemDtoBuilder.Create()
                .WithQuantity(-1)
                .Build();

            var command = AddItemToCartCommandBuilder.Create()
                .WithCartItem(cartItemDto)
                .Build();

            
            await Assert.ThrowsAsync<ArgumentException>(() => handler.Handle(command, CancellationToken.None));
        }

        [Fact]
        public async Task AddCommand_ShouldHandleLargeQuantity()
        {
            var cartItemDto = CartItemDtoBuilder.Create()
                .WithQuantity(int.MaxValue)
                .Build();

            var command = AddItemToCartCommandBuilder.Create()
                .WithCartItem(cartItemDto)
                .Build();

            
            var cartItemId = await handler.Handle(command, CancellationToken.None);

            Assert.NotEqual(Guid.Empty, cartItemId);
        }

        [Fact]
        public async Task AddCommand_ShouldHandleEmptyProductName()
        {
            var cartItemDto = CartItemDtoBuilder.Create()
                .WithProductName(string.Empty)
                .Build();

            var command = AddItemToCartCommandBuilder.Create()
                .WithCartItem(cartItemDto)
                .Build();

            
            var cartItemId = await handler.Handle(command, CancellationToken.None);

            Assert.NotEqual(Guid.Empty, cartItemId);
        }

        [Fact]
        public async Task AddCommand_ShouldHandleInvalidProductId()
        {
            var cartItemDto = CartItemDtoBuilder.Create()
                .WithProductId(Guid.Empty)
                .Build();

            var command = AddItemToCartCommandBuilder.Create()
                .WithCartItem(cartItemDto)
                .Build();

            
            await Assert.ThrowsAsync<ArgumentException>(() => handler.Handle(command, CancellationToken.None));
        }

        [Fact]
        public async Task AddCommand_ShouldHandleInvalidShoppingCartId()
        {
            var cartItemDto = CartItemDtoBuilder.Create()
                .WithShoppingCartId(Guid.Empty)
                .Build();

            var command = AddItemToCartCommandBuilder.Create()
                .WithCartItem(cartItemDto)
                .Build();

            
            await Assert.ThrowsAsync<ArgumentException>(() => handler.Handle(command, CancellationToken.None));
        }

        [Fact]
        public async Task AddCommand_ShouldHandleInvalidPrice()
        {
            var cartItemDto = CartItemDtoBuilder.Create()
                .WithPrice(-10.99m)
                .Build();

            var command = AddItemToCartCommandBuilder.Create()
                .WithCartItem(cartItemDto)
                .Build();

            
            await Assert.ThrowsAsync<ArgumentException>(() => handler.Handle(command, CancellationToken.None));
        }

    }
}
