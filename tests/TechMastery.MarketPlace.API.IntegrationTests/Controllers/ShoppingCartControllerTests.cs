using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using TechMastery.MarketPlace.API.IntegrationTests.Base;
using TechMastery.MarketPlace.Application.Features.Checkout.Dto;
using TechMastery.MarketPlace.Application.Features.Checkout.Handlers;
using TechMastery.MarketPlace.Application.Features.Checkout.ViewModels;
using TechMastery.MarketPlace.Application.Features.ProductListing.Dto;
using TechMastery.MarketPlace.Application.Features.ProductListing.Handlers;
using Xunit;

namespace TechMastery.MarketPlace.API.IntegrationTests
{
    public class SimulationTests : IClassFixture<CustomWebApplicationFactory<TestStartup>>
    {
        private readonly HttpClient _client;

        public SimulationTests(CustomWebApplicationFactory<TestStartup> testStartup)
        {
            _client = testStartup.GetAuthenticatedClient();
        }

        [Fact]
        public async Task FullShoppingCartWorkflow_SuccessScenario()
        {
            // Arrange: Create and add a new product listing
            var newListing = new AddListingCommand
            {
                Name = "Sample Product",
                Description = "This is a sample product for testing purposes.",
                ListingPrice = 99.99m,
                PromotionPrice = 89.99m,
                Category = new CategoryDto() // Initialize category properties as needed
            };

            var addListingResponse = await _client.PostAsJsonAsync("/api/Product/AddListing", newListing);
            addListingResponse.EnsureSuccessStatusCode();
            var productId = await addListingResponse.Content.ReadFromJsonAsync<Guid>();
            Assert.NotEqual(Guid.Empty, productId);

            // Arrange: Add the new product to the shopping cart
            var addItemCommand = new AddCartItem(Guid.NewGuid(), new CartItemDto(Guid.Empty, productId));
            var addCartItemResponse = await _client.PostAsJsonAsync("/api/ShoppingCart/add-item", addItemCommand);
            addCartItemResponse.EnsureSuccessStatusCode();
            var cartItemIds = await addCartItemResponse.Content.ReadFromJsonAsync<Guid[]>();
            Assert.NotEmpty(cartItemIds);
            var createdCartItemId = cartItemIds[0];

            // Act: Fetch the shopping cart by user
            var fetchResponse = await _client.GetAsync("/api/ShoppingCart/get-cart-by-user");
            fetchResponse.EnsureSuccessStatusCode();
            var fetchedCartDto = await fetchResponse.Content.ReadFromJsonAsync<ShoppingCartVm>();
            Assert.NotNull(fetchedCartDto);
            Assert.Contains(fetchedCartDto.CartItems, item => item.CartItemId == createdCartItemId);

            // Act: Update the cart item
            var updateCommand = new UpdateCartItem
            {
                CartItemId = createdCartItemId,
                Quantity = 2
            };
            var updateResponse = await _client.PutAsJsonAsync("/api/ShoppingCart/update-item", updateCommand);
            updateResponse.EnsureSuccessStatusCode();

            // Act: Fetch the shopping cart again
            fetchResponse = await _client.GetAsync("/api/ShoppingCart/get-cart-by-user");
            fetchResponse.EnsureSuccessStatusCode();
            var updatedCartDto = await fetchResponse.Content.ReadFromJsonAsync<ShoppingCartVm>();
            var updatedItem = updatedCartDto.CartItems.First(item => item.CartItemId == createdCartItemId);
            Assert.Equal(2, updatedItem.Quantity);
        }
    }
}
