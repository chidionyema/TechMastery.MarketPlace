using TechMastery.MarketPlace.Application.Contracts.Persistence;
using TechMastery.MarketPlace.Application.Features.Checkout.Handlers;
using TechMastery.MarketPlace.Application.Features.Checkout.Dto;

namespace TechMastery.MarketPlace.Application.Tests.Integration
{
    public class AddToCartHandlerTests : IClassFixture<ApplicationTestFixture>
    {
        private readonly ApplicationTestFixture _fixture;
        private readonly IShoppingCartRepository _shoppingCartRepository;
        private readonly ICartItemRepository _cartItemRepository;

        public AddToCartHandlerTests(ApplicationTestFixture fixture)
        {
            _fixture = fixture;
            _shoppingCartRepository = _fixture.CreateCartRepository();
            _cartItemRepository = _fixture.CreateCartItemRepository();
        }

        [Fact]
        public async Task AddCommand_ShouldCreateNewCartItem()
        {
            var cartItemDto = CartItemDtoBuilder.Create()
                .Build();

            var command = AddItemToCartCommandBuilder.Create()
                .WithCartItem(cartItemDto)
                .Build();

            var handler = new AddItemToCartHandler(_cartItemRepository, _shoppingCartRepository);
            var cartItemIds = await handler.Handle(command, CancellationToken.None);

            Assert.Single(cartItemIds);
            Assert.NotEqual(Guid.Empty, cartItemIds.Single());
        }

        [Fact]
        public async Task AddCommand_ShouldHandleNullCartItem()
        {
            var command = AddItemToCartCommandBuilder.Create()
                .WithCartItems(null)
                .Build();

            var handler = new AddItemToCartHandler(_cartItemRepository, _shoppingCartRepository);
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

            var handler = new AddItemToCartHandler(_cartItemRepository, _shoppingCartRepository);
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

            var handler = new AddItemToCartHandler(_cartItemRepository, _shoppingCartRepository);
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

            var handler = new AddItemToCartHandler(_cartItemRepository, _shoppingCartRepository);
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

            var handler = new AddItemToCartHandler(_cartItemRepository, _shoppingCartRepository);
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

            var handler = new AddItemToCartHandler(_cartItemRepository, _shoppingCartRepository);
            var cartItemIds = await handler.Handle(command, CancellationToken.None);

            Assert.Single(cartItemIds);
            Assert.NotEqual(Guid.Empty, cartItemIds.Single());
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

            var handler = new AddItemToCartHandler(_cartItemRepository, _shoppingCartRepository);
            var cartItemIds = await handler.Handle(command, CancellationToken.None);

            Assert.Single(cartItemIds);
            Assert.NotEqual(Guid.Empty, cartItemIds.Single());
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

            var handler = new AddItemToCartHandler(_cartItemRepository, _shoppingCartRepository);
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

            var handler = new AddItemToCartHandler(_cartItemRepository, _shoppingCartRepository);
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

            var handler = new AddItemToCartHandler(_cartItemRepository, _shoppingCartRepository);
            await Assert.ThrowsAsync<ArgumentException>(() => handler.Handle(command, CancellationToken.None));
        }

        [Fact]
        public async Task AddCommand_ShouldHandleMultipleCartItems()
        {
            var cartItem1 = CartItemDtoBuilder.Create()
                .Build();

            var cartItem2 = CartItemDtoBuilder.Create()
                .WithPrice(5.99m)
                .Build();

            var command = AddItemToCartCommandBuilder.Create()
                .WithCartItems(new List<CartItemDto> { cartItem1, cartItem2 })
                .Build();

            var handler = new AddItemToCartHandler(_cartItemRepository, _shoppingCartRepository);
            var cartItemIds = await handler.Handle(command, CancellationToken.None);

            Assert.Equal(2, cartItemIds.Count);
            Assert.True(cartItemIds.All(id => id != Guid.Empty));
        }

        // Add more test cases as needed to cover various scenarios.
    }
}
