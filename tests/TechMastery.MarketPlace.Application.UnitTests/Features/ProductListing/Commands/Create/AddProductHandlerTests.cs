using TechMastery.MarketPlace.Application.Contracts.Persistence;
using TechMastery.MarketPlace.Application.Contracts.Messaging;
using TechMastery.MarketPlace.Application.Exceptions;
using TechMastery.MarketPlace.Application.Features.ProductListing.Handlers;
using TechMastery.MarketPlace.Domain.Entities;
using MediatR;
using TechMastery.MarketPlace.Application.IntegrationTests.Fakes;
using Microsoft.Extensions.Logging;
using Moq;

namespace TechMastery.MarketPlace.Application.Tests.Integration
{
    public class AddProductHandlerTests : IClassFixture<ApplicationTestFixture>
    {
        private readonly ApplicationTestFixture _fixture;
        private readonly FakeBlobStorageService _blobStorageService;
        private readonly IProductRepository _productRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly AddOrUpdateListingHandler handler;
        private readonly IMessagePublisher _publisher;
        public AddProductHandlerTests(ApplicationTestFixture fixture)
        {
            _fixture = fixture;
            _blobStorageService = new FakeBlobStorageService();
            _publisher = new FakeMessagePublisher();
            _productRepository = _fixture.CreateProductListingRepository();
            _categoryRepository = _fixture.CreateCategoryRepository();
            handler = new AddOrUpdateListingHandler(new Mock<ILogger<AddOrUpdateListingHandler>>().Object, _productRepository, _categoryRepository, _publisher);
        }

        [Fact]
        public async Task AddCommand_ShouldUploadAndAssociateArtifacts()
        {
            // Arrange
            ConfigureBlobStorageServiceFake();
            var category = await CreateAndSaveSampleCategory();
            var command = AddOrUpdateListingCommandBuilder.Create()
                .WithCategory(category)
                .WithUploadAssets(2)
                .Build();

            // Act
            var productId = await handler.Handle(command, CancellationToken.None);

            // Assert
            var product = await _productRepository.GetByIdAsync(productId);
            Assert.NotNull(product);
            Assert.Equal(command.UploadAssets.Count, product.Artifacts.Count);
        }

        [Fact]
        public async Task AddCommand_ShouldCreateProductWithCategory()
        {
            // Arrange
            ConfigureBlobStorageServiceFake();
            var category = await CreateAndSaveSampleCategory();
            var command = AddOrUpdateListingCommandBuilder.Create()
                .WithCategory(category)
                .Build();

            // Act
            var productId = await handler.Handle(command, CancellationToken.None);

            // Assert
            var product = await _productRepository.GetByIdAsync(productId);
            Assert.NotNull(product);
            Assert.Equal(category.CategoryId, product.CategoryId);
        }

        [Fact]
        public async Task AddCommand_ShouldCreateProductWithArtifactUrl()
        {
            // Arrange
            ConfigureBlobStorageServiceFake();
            var category = await CreateAndSaveSampleCategory();
            var command = AddOrUpdateListingCommandBuilder.Create()
                .WithCategory(category)
                .WithUploadAssets(2)
                .Build();

            var mockBlobUrls = new List<string>
            {
                "https://mock-blob-url1.com",
                "https://mock-blob-url2.com"
            };
            _blobStorageService.UploadFileAsyncFunc = (blobName, stream, cancellationToken) =>
            {
                return Task.FromResult(mockBlobUrls[0]);
            };

            // Act
            var productId = await handler.Handle(command, CancellationToken.None);

            // Assert
            var product = await _productRepository.GetByIdAsync(productId);
            Assert.NotNull(product);
            Assert.Equal(command.UploadAssets.Count, product.Artifacts.Count);

            var expectedArtifacts = CreateSampleProductArtifacts(command.UploadAssets.Count, mockBlobUrls[0]);

            for (int i = 0; i < command.UploadAssets.Count; i++)
            {
                Assert.Equal(expectedArtifacts[i].BlobUrl, product.Artifacts.ToArray()[i].BlobUrl);
            }
        }

        [Fact]
        public async Task AddCommand_ShouldCreateProductWithArtifacts()
        {
            // Arrange
            ConfigureBlobStorageServiceFake();
            var category = await CreateAndSaveSampleCategory();
            var command = AddOrUpdateListingCommandBuilder.Create()
                .WithCategory(category)
                .WithUploadAssets(2)
                .Build();

            // Act
            var productId = await handler.Handle(command, CancellationToken.None);

            // Assert
            var product = await _productRepository.GetByIdAsync(productId);
            Assert.NotNull(product);
            Assert.Equal(command.UploadAssets.Count, product.Artifacts.Count);
        }

        [Fact]
        public async Task AddCommand_ShouldCreateProductWithDependencies()
        {
            // Arrange
            ConfigureBlobStorageServiceFake();
            var category = await CreateAndSaveSampleCategory();
            var command = AddOrUpdateListingCommandBuilder.Create()
                .WithCategory(category)
                .WithDependencies(1)
                .Build();

            // Act
            var productId = await handler.Handle(command, CancellationToken.None);

            // Assert
            var product = await _productRepository.GetByIdAsync(productId);
            Assert.NotNull(product);
            Assert.Collection(product.Dependencies, dependencies =>
            {
                Assert.Equal(command.Dependencies[0].Name, dependencies.Dependency.Name);
                Assert.Equal(command.Dependencies[0].Version, dependencies.Dependency.Version);
                Assert.Equal(command.Dependencies[0].DependencyType, dependencies.Dependency.DependencyTypeEnum);
            });
        }

        // ... (previous code)

        [Fact]
        public async Task AddCommand_ShouldCreateProductWithNoTags()
        {
            // Arrange
            ConfigureBlobStorageServiceFake();
            var category = await CreateAndSaveSampleCategory();
            var command = AddOrUpdateListingCommandBuilder.Create()
                .WithCategory(category)
                .WithNoTags()
                .Build();

            // Act
            var productId = await handler.Handle(command, CancellationToken.None);

            // Assert
            var product = await _productRepository.GetByIdAsync(productId);
            Assert.NotNull(product);
            Assert.Empty(product.Tags);
        }

        [Fact]
        public async Task AddCommand_ShouldCreateProductWithNoDependencies()
        {
            // Arrange
            ConfigureBlobStorageServiceFake();
            var category = await CreateAndSaveSampleCategory();
            var command = AddOrUpdateListingCommandBuilder.Create()
                .WithCategory(category)
                .WithNoDependencies()
                .Build();

            // Act
            var productId = await handler.Handle(command, CancellationToken.None);

            // Assert
            var product = await _productRepository.GetByIdAsync(productId);
            Assert.NotNull(product);
            Assert.Empty(product.Dependencies);
        }

        [Fact]
        public async Task AddCommand_ShouldCreateProductWithNoArtifacts()
        {
            // Arrange
            ConfigureBlobStorageServiceFake();
            var category = await CreateAndSaveSampleCategory();
            var command = AddOrUpdateListingCommandBuilder.Create()
                .WithCategory(category)
                .WithNoUploadAssets()
                .Build();

            // Act
            var productId = await handler.Handle(command, CancellationToken.None);

            // Assert
            var product = await _productRepository.GetByIdAsync(productId);
            Assert.NotNull(product);
            Assert.Empty(product.Artifacts);
        }

        [Fact]
        public async Task AddCommand_ShouldNotCreateProductWithInvalidCategory()
        {
            // Arrange
            ConfigureBlobStorageServiceFake();
            var invalidCategoryId = Guid.NewGuid();
            var command = AddOrUpdateListingCommandBuilder.Create()
                .WithInvalidCategory(invalidCategoryId)
                .Build();

            // Act and Assert
            await Assert.ThrowsAsync<NotFoundException>(() => handler.Handle(command, CancellationToken.None));
        }

        private async Task<Category> CreateAndSaveSampleCategory()
        {
            var category = CategoryBuilder.Create().Build();
            var createdCategory = await _categoryRepository.AddAsync(category);
            return createdCategory;
        }

        private void ConfigureBlobStorageServiceFake()
        {
            _blobStorageService.UploadFileAsyncFunc = async (blobName, stream, cancellationToken) =>
            {
                return $"https://example.com/{blobName}";
            };
        }

        private List<ProductArtifact> CreateSampleProductArtifacts(int count, string blobUrl)
        {
            var artifacts = new List<ProductArtifact>();
            for (int i = 0; i < count; i++)
            {
                artifacts.Add(new ProductArtifact(new ProductArtifactTypeEnum(), blobUrl, DateTime.UtcNow));
            }
            return artifacts;
        }
    }
}

