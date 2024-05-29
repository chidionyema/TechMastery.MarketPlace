using TechMastery.MarketPlace.Application.Exceptions;
using TechMastery.MarketPlace.Application.Features.ProductListing.Handlers;
using TechMastery.MarketPlace.Domain.Entities;
using TechMastery.MarketPlace.Application.IntegrationTests.Fakes;
using Microsoft.Extensions.Logging;
using Moq;
using TechMastery.MarketPlace.Application.Persistence.Contracts;
using TechMastery.MarketPlace.Application.Contracts;

namespace TechMastery.MarketPlace.Application.Tests.Integration
{
    public class AddProductHandlerTests : IClassFixture<ApplicationTestFixture>
    {
        private readonly ApplicationTestFixture _fixture;
        private readonly FakeBlobStorageService _blobStorageService;
        private readonly IProductRepository _productRepository;
        private readonly IAsyncRepository<Language> _languageRepository;
        private readonly IAsyncRepository<Platform> _platformRepository;
        private readonly IAsyncRepository<Framework> _frameworkRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly AddListingHandler handler;
        private readonly IMessagePublisher _publisher;

        public AddProductHandlerTests(ApplicationTestFixture fixture)
        {
            _fixture = fixture;
            _blobStorageService = new FakeBlobStorageService();
            _publisher = new FakeMessagePublisher();
            _productRepository = _fixture.CreateProductListingRepository();
            _categoryRepository = _fixture.CreateCategoryRepository();
            _languageRepository = _fixture.CreateRepository<Language>();
            _platformRepository = _fixture.CreateRepository<Platform>();
            _frameworkRepository = _fixture.CreateRepository<Framework>();
            handler = new AddListingHandler(new Mock<ILogger<AddListingHandler>>().Object, _productRepository, _categoryRepository, _languageRepository, _frameworkRepository, _platformRepository, _publisher);
        }

        [Fact]
        public async Task AddCommand_ShouldCorrectlyAssociateLanguagePlatformFramework()
        {
            // Arrange
            ConfigureBlobStorageServiceFake();
            var category = await CreateAndSaveSampleCategory();

            // Fetching existing entities for the test
            var platform = (await _platformRepository.ListAllAsync()).First();
            var language = (await _languageRepository.ListAllAsync()).First();
            var framework = (await _frameworkRepository.ListAllAsync()).First();

            // Ensure these entities are present
            Assert.NotNull(language);
            Assert.NotNull(platform);
            Assert.NotNull(framework);

            var command = AddOrUpdateListingCommandBuilder.Create()
                .WithCategory(category)
                .WithLanguage(language.Id)
                .WithPlatform(platform.Id)
                .WithFramework(framework.Id)
                .Build();

            // Act
            var productId = await handler.Handle(command, CancellationToken.None);

            // Assert
            var product = await _productRepository.GetByIdAsync(productId);
            Assert.NotNull(product);
            Assert.Equal(category.Id, product.CategoryId);

            // Additional assertions to ensure that language, platform, and framework associations are correctly made
            Assert.Contains(product.ProductLanguages, pl => pl.LanguageId == language.Id);
            Assert.Contains(product.ProductPlatforms, pp => pp.PlatformId == platform.Id);
            Assert.Contains(product.ProductFrameworks, pf => pf.FrameworkId == framework.Id);
        }


        [Fact]
        public async Task AddCommand_ShouldCreateProductWithCategory()
        {
            // Arrange
            ConfigureBlobStorageServiceFake();
            var category = await CreateAndSaveSampleCategory();
            var platform = await _platformRepository.ListAllAsync();
            var language = await _languageRepository.ListAllAsync();
            var framework = await _frameworkRepository.ListAllAsync();

            // Ensure these entities are not null (assuming seed data is present)
            Assert.NotNull(language);
            Assert.NotNull(platform);
            Assert.NotNull(framework);

            var command = AddOrUpdateListingCommandBuilder.Create()
                .WithCategory(category)
                .WithLanguage(language.First().Id)
                .WithPlatform(platform.First().Id)
                .WithFramework(framework.First().Id)
                .Build();

            // Act
            var productId = await handler.Handle(command, CancellationToken.None);

            // Assert
            var product = await _productRepository.GetByIdAsync(productId);
            Assert.NotNull(product);
            Assert.Equal(category.Id, product.CategoryId);
        }

  

        [Fact]
        public async Task AddCommand_ShouldCreateProductWithNoTags()
        {
            // Arrange
            ConfigureBlobStorageServiceFake();
            var category = await CreateAndSaveSampleCategory();
            var platform = await _platformRepository.ListAllAsync();
            var language = await _languageRepository.ListAllAsync();
            var framework = await _frameworkRepository.ListAllAsync();

            // Ensure these entities are not null (assuming seed data is present)
            Assert.NotNull(language);
            Assert.NotNull(platform);
            Assert.NotNull(framework);

            var command = AddOrUpdateListingCommandBuilder.Create()
                .WithCategory(category)
                .WithLanguage(language.First().Id)
                .WithPlatform(platform.First().Id)
                .WithFramework(framework.First().Id)
               // .WithNoTags()
                .Build();

            // Act
            var productId = await handler.Handle(command, CancellationToken.None);

            // Assert
            var product = await _productRepository.GetByIdAsync(productId);
            Assert.NotNull(product);
            Assert.Empty(product.Tags);
        }

      
        [Fact]
        public async Task AddCommand_ShouldCreateProductWithNoArtifacts()
        {
            // Arrange
            ConfigureBlobStorageServiceFake();
            var category = await CreateAndSaveSampleCategory();
            var platform = await _platformRepository.ListAllAsync();
            var language = await _languageRepository.ListAllAsync();
            var framework = await _frameworkRepository.ListAllAsync();

            // Ensure these entities are not null (assuming seed data is present)
            Assert.NotNull(language);
            Assert.NotNull(platform);
            Assert.NotNull(framework);

       
            var command = AddOrUpdateListingCommandBuilder.Create()
                .WithCategory(category)
                .WithLanguage(language.First().Id)
                .WithPlatform(platform.First().Id)
                .WithFramework(framework.First().Id)
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

        private async Task<Domain.Entities.Category> CreateAndSaveSampleCategory()
        {
            var category = new CategoryBuilder().WithName("category").Build();
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

