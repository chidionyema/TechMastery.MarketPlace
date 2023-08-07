using AutoFixture;
using TechMastery.MarketPlace.Domain.Entities;
using TechMastery.MarketPlace.Persistence.Repositories;
using Xunit;

namespace TechMastery.MarketPlace.Infrastructure.IntegrationTests.Repository
{
    public class ProductListingRepositoryTests : IntegrationTestFixture, IDisposable
    {
        private readonly ProductRepository _repository;
        private readonly Fixture _fixture;

        public ProductListingRepositoryTests() : base()
        {
            _repository = CreateProductListingRepository();
            _fixture = new Fixture();
            _fixture.Behaviors.Add(new OmitOnRecursionBehavior());
        }

        [Fact]
        public async Task AddProductListing_ShouldInsertProductListingWithComplexRelationships()
        {
            // Arrange
            var productListing = _fixture.Build<Product>()
                .Without(pl => pl.ProductId)
                .Create();

            // Act
            var createdProductListing = await _repository.AddAsync(productListing);

            // Assert
            Assert.NotNull(createdProductListing);
            Assert.NotEqual(Guid.Empty, createdProductListing.ProductId);
            // Perform additional assertions

        }

        [Fact]
        public async Task UpdateProductListing_ShouldUpdateProductListingAndRelationships()
        {
            // Arrange
            var existingProductListing = _fixture.Create<Product>();
            await _repository.AddAsync(existingProductListing);

            // Fetch the existing entity from the database
            var retrievedProductListing = await _repository.GetByIdAsync(existingProductListing.ProductId);

            // Apply the updates to the retrieved entity
            retrievedProductListing.SetName ("Updated Product Listing");
            retrievedProductListing.SetDescription ("Updated description");
            // Apply other updates to the retrieved entity

            // Act
            await _repository.UpdateAsync(retrievedProductListing);

            // Assert
            var updatedProductListing = await _repository.GetByIdAsync(existingProductListing.ProductId);
            Assert.NotNull(updatedProductListing);
            Assert.Equal("Updated Product Listing", updatedProductListing.Name);
            Assert.Equal("Updated description", updatedProductListing.Description);
            // Perform additional assertions
        }


        [Fact]
        public async Task GetProductListingById_ShouldReturnCorrectProductListing()
        {
            // Arrange
            var productListing = _fixture.Create<Product>();
            await _repository.AddAsync(productListing);

            // Act
            var retrievedProductListing = await _repository.GetByIdAsync(productListing.ProductId);

            // Assert
            Assert.NotNull(retrievedProductListing);
            // Perform additional assertions

        }

        [Fact]
        public async Task GetProductListingWithRelationships_ShouldReturnProductListingWithAllRelatedEntities()
        {
            // Arrange
            var productListing = _fixture.Create<Product>();
            await _repository.AddAsync(productListing);

            // Act
            var retrievedProductListing = await _repository.GetByIdAsync(productListing.ProductId);

            // Assert
            Assert.NotNull(retrievedProductListing);
            // Perform additional assertions

        }

        [Fact]
        public async Task DeleteProductListing_ShouldDeleteProductListingAndCascadeDeleteRelationships()
        {
            // Arrange
            var productListing = _fixture.Create<Product>();
            await _repository.AddAsync(productListing);

            // Act
            await _repository.DeleteAsync(productListing);

            // Assert
            var deletedProductListing = await _repository.GetByIdAsync(productListing.ProductId);
            Assert.Null(deletedProductListing);
            // Perform additional assertions
        }

        // Additional tests for querying, pagination, sorting, etc.

        [Fact]
        public async Task GetAllProductListings_ShouldReturnAllProductListings()
        {
            // Arrange
            var productListing1 = _fixture.Create<Product>();
            await _repository.AddAsync(productListing1);

            var productListing2 = _fixture.Create<Product>();
            await _repository.AddAsync(productListing2);

            // Act
            var productListings = await _repository.ListAllAsync();

            // Assert
            Assert.NotNull(productListings);
            Assert.Equal(2, productListings.Count);
            // Perform additional assertions

        }

        // teardown
        public void Dispose()
        {
            DisposeContext();
        }
    }
}
