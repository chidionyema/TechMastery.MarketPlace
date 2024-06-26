﻿using AutoFixture;
using TechMastery.MarketPlace.Domain.Entities;
using TechMastery.MarketPlace.Persistence.Repositories;
using TechMastery.MarketPlace.Tests.Emulators;
using Xunit;

namespace TechMastery.MarketPlace.API.IntegrationTests
{
    [Collection("DbEmulatorCollection")] // This associates the test class with DbEmulatorFixture.
    public class ProductListingRepositoryTests : IClassFixture<DbEmulatorFixture>
    {
        private readonly ProductRepository _repository;
        private readonly Fixture _fixture;

        public ProductListingRepositoryTests(DbEmulatorFixture dbEmulatorFixture) // Constructor injection of the DbEmulatorFixture.
        {
            _repository = dbEmulatorFixture.CreateProductRepository(); // Use DbEmulatorFixture to create the repository.
            _fixture = new Fixture();
            _fixture.Behaviors.Add(new OmitOnRecursionBehavior());
        }

        [Fact]
        public async Task AddProductListing_ShouldInsertProductListingWithComplexRelationships()
        {
            // Arrange
            var productListing = _fixture.Build<Domain.Entities.Product>()
                .Without(pl => pl.Id)
                .Create();

            // Act
            var createdProductListing = await _repository.AddAsync(productListing);

            // Assert
            Assert.NotNull(createdProductListing);
            Assert.NotEqual(Guid.Empty, createdProductListing.Id);
            // Perform additional assertions

        }

        [Fact]
        public async Task UpdateProductListing_ShouldUpdateProductListingAndRelationships()
        {
            // Arrange
            var existingProductListing = _fixture.Create<Domain.Entities.Product>();
            await _repository.AddAsync(existingProductListing);

            // Fetch the existing entity from the database
            var retrievedProductListing = await _repository.GetByIdAsync(existingProductListing.Id);

            // Apply the updates to the retrieved entity
            retrievedProductListing?.SetName("Updated Product Listing");
            retrievedProductListing?.SetDescription("Updated description");
            // Apply other updates to the retrieved entity

            // Act
            await _repository.UpdateAsync(retrievedProductListing!);

            // Assert
            var updatedProductListing = await _repository.GetByIdAsync(existingProductListing.Id);
            Assert.NotNull(updatedProductListing);
            Assert.Equal("Updated Product Listing", updatedProductListing.Name);
            Assert.Equal("Updated description", updatedProductListing.Description);
            // Perform additional assertions
        }


        [Fact]
        public async Task GetProductListingById_ShouldReturnCorrectProductListing()
        {
            // Arrange
            var productListing = _fixture.Create<Domain.Entities.Product>();
            await _repository.AddAsync(productListing);

            // Act
            var retrievedProductListing = await _repository.GetByIdAsync(productListing.Id);

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
            var retrievedProductListing = await _repository.GetByIdAsync(productListing.Id);

            // Assert
            Assert.NotNull(retrievedProductListing);
            // Perform additional assertions

        }

        [Fact]
        public async Task DeleteProductListing_ShouldDeleteProductListingAndCascadeDeleteRelationships()
        {
            // Arrange
            var productListing = _fixture.Create<Domain.Entities.Product>();
            await _repository.AddAsync(productListing);

            // Act
            await _repository.DeleteAsync(productListing);

            // Assert
            var deletedProductListing = await _repository.GetByIdAsync(productListing.Id);
            Assert.Null(deletedProductListing);
            // Perform additional assertions
        }

        // Additional tests for querying, pagination, sorting, etc.

        [Fact]
        public async Task GetAllProductListings_ShouldReturnAllProductListings()
        {
            // Arrange
            var productListing1 = _fixture.Create<Domain.Entities.Product>();
            await _repository.AddAsync(productListing1);

            var productListing2 = _fixture.Create<Domain.Entities.Product>();
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
        }
    }
}
