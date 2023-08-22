using Microsoft.Extensions.Logging;
using Moq;
using TechMastery.MarketPlace.Application.Models.Search;
using TechMastery.MarketPlace.Infrastructure.Search;
using TechMastery.MarketPlace.Tests.Emulators;

namespace TechMastery.MarketPlace.Infrastructure.IntegrationTests
{
    [Collection("ElasticSearchTestCollection")]
    public class ProductSearchServiceIntegrationTests 
    {
        private readonly ElasticSearchFixture _fixture;

        public ProductSearchServiceIntegrationTests(ElasticSearchFixture fixture)
        {
            _fixture = fixture;
        }

        [Fact]
        public async Task IndexAndSearchProducts_ShouldReturnMatchingProducts()
        {
            // Arrange
            var productSearchService = new ProductSearchService(
                Mock.Of<ILogger<ProductSearchService>>(),
                _fixture.ElasticClient,
                "products");

            var products = new List<ProductSearch>
            {
                new ProductSearch { ProductId = Guid.NewGuid(), Title = "Product 1", Description = "Description 1", Category = "Category 1", Price = 10.0m },
                new ProductSearch { ProductId = Guid.NewGuid(), Title = "Product 2", Description = "Description 2", Category = "Category 2", Price = 20.0m }
            };

            // Act
            await productSearchService.IndexProducts(products);

            var searchRequest = new ProductSearchRequest
            {
                Query = "Product",
                Category = "Category 1",
                MinPrice = 0.0m,
                MaxPrice = 15.0m
            };

            var searchResults = productSearchService.SearchProducts(searchRequest);

            // Assert
            Assert.Equal(2, searchResults.Count);
            Assert.Equal("Product 1", searchResults.First().Title);
        }

        // Add more test cases to cover other scenarios
    }
}
