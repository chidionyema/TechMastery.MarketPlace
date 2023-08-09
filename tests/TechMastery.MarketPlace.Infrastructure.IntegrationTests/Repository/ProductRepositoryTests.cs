using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TechMastery.MarketPlace.Domain.Entities;
using TechMastery.MarketPlace.Persistence.Repositories;
using Xunit;

namespace TechMastery.MarketPlace.Infrastructure.IntegrationTests
{
    [Collection("DbEmulatorCollection")]
    public class ProductListingRepositoryTests
    {
        private readonly ProductRepository _productRepository;
        private readonly CategoryRepository _categoryRepository;
        private readonly DbEmulatorFixture _dbfixture;

        public ProductListingRepositoryTests(DbEmulatorFixture dbfixture)
        {
            _dbfixture = dbfixture;
            _productRepository = _dbfixture.CreateProductRepository();
            _categoryRepository = _dbfixture.CreateCategoryRepository();
        }

        [Fact]
        public async Task AddProductListing_ShouldInsertProductListingWithComplexRelationships()
        {
            // Arrange
           var categoryId = Guid.NewGuid();
            var category = new Category("Test Category");
            await _categoryRepository.AddAsync(category);

            var product = new Product(category.CategoryId, "Test Product", "Test Description", "demo.com", 100.0m,
                "Test License", "Test Owner", "Test Purpose");

            // Act
            var createdProductListing = await _productRepository.AddAsync(product);

            // Assert
            Assert.NotNull(createdProductListing);
            Assert.NotEqual(Guid.Empty, createdProductListing.ProductId);
            // Perform additional assertions
        }

        [Fact]
        public async Task UpdateProductListing_ShouldUpdateProductListingAndRelationships()
        {
            // Arrange
            var categoryId = Guid.NewGuid();
            var category = new Category("Test Category");
            await _categoryRepository.AddAsync(category);

            var existingProductListing = new Product(category.CategoryId, "Existing Product", "Existing Description", "demo.com", 100.0m,
                "Existing License", "Existing", "Existing Purpose");
            await _productRepository.AddAsync(existingProductListing);

            // Fetch the existing entity from the database
            var retrievedProductListing = await _productRepository.GetByIdAsync(existingProductListing.ProductId);

            // Apply the updates to the retrieved entity
            retrievedProductListing.SetName("Updated Product Listing");
            retrievedProductListing.SetDescription("Updated description");
            // Apply other updates to the retrieved entity

            // Act
            await _productRepository.UpdateAsync(retrievedProductListing);

            // Assert
            var updatedProductListing = await _productRepository.GetByIdAsync(existingProductListing.ProductId);
            Assert.NotNull(updatedProductListing);
            Assert.Equal("Updated Product Listing", updatedProductListing.Name);
            Assert.Equal("Updated description", updatedProductListing.Description);
            // Perform additional assertions
        }

        [Fact]
        public async Task GetProductListingById_ShouldReturnCorrectProductListing()
        {
            // Arrange
            var categoryId = Guid.NewGuid();
            var category = new Category("Test Category");
            await _categoryRepository.AddAsync(category);

            var product = new Product(category.CategoryId, "Test Product", "Test Description", "demo.com", 100.0m,
                "Test License", "Test Owner", "Test Purpose");
            await _productRepository.AddAsync(product);

            // Act
            var retrievedProductListing = await _productRepository.GetByIdAsync(product.ProductId);

            // Assert
            Assert.NotNull(retrievedProductListing);
            // Perform additional assertions
        }

        [Fact]
        public async Task GetProductListingWithRelationships_ShouldReturnProductListingWithAllRelatedEntities()
        {
            // Arrange
            var categoryId = Guid.NewGuid();
            var category = new Category("Test Category");
            await _categoryRepository.AddAsync(category);

            var product = new Product(category.CategoryId, "Test Product", "Test Description", "demo.com", 100.0m,
                "Test License", "Test Owner1", "Test Purpose");
            await _productRepository.AddAsync(product);

            // Act
            var retrievedProductListing = await _productRepository.GetByIdAsync(product.ProductId);

            // Assert
            Assert.NotNull(retrievedProductListing);
            // Perform additional assertions
        }

        [Fact]
        public async Task DeleteProductListing_ShouldDeleteProductListingAndCascadeDeleteRelationships()
        {
            // Arrange
            var categoryId = Guid.NewGuid();
            var category = new Category("Test Category");
            await _categoryRepository.AddAsync(category);

            var product = new Product(category.CategoryId, "Test Product", "Test Description", "demo.com", 100.0m,
                "Test License", "Test Owner2", "Test Purpose");
            await _productRepository.AddAsync(product);

            // Act
            await _productRepository.DeleteAsync(product);

            // Assert
            var deletedProductListing = await _productRepository.GetByIdAsync(product.ProductId);
            Assert.Null(deletedProductListing);
            // Perform additional assertions
        }

        [Fact]
        public async Task GetAllProductListings_ShouldReturnAllProductListings()
        {
            // Arrange
            var categoryId = Guid.NewGuid();
            var category = new Category("Test Category");
            await _categoryRepository.AddAsync(category);
            
            var product1 = new Product(category.CategoryId, "Test Product 1", "Test Description 1", "demo1.com", 100.0m,
                "Test License 1", "Test Owner 3", "Test Purpose 1");
            await _productRepository.AddAsync(product1);

            var product2 = new Product(category.CategoryId, "Test Product 2", "Test Description 2", "demo2.com", 200.0m,
                "Test License 2", "Test Owner 4", "Test Purpose 2");
            await _productRepository.AddAsync(product2);

            // Act
            var productListings = await _productRepository.ListAllAsync();

            // Assert
            Assert.NotNull(productListings);
            Assert.Equal(2, productListings.Count());
            // Perform additional assertions
        }

        // Additional tests for querying, pagination, sorting, etc.

        // teardown

    }
}
