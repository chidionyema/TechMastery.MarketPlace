using Microsoft.Extensions.Logging;
using MediatR;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TechMastery.MarketPlace.Application.Contracts.Infrastructure;
using TechMastery.MarketPlace.Application.Contracts.Persistence;
using TechMastery.MarketPlace.Application.DataTransferObjects;
using TechMastery.MarketPlace.Application.Exceptions;
using TechMastery.MarketPlace.Application.Features.ProductListing.DataTransferObjects;
using TechMastery.MarketPlace.Domain.Entities;
using TechMastery.MarketPlace.Application.Messaging;
using TechMastery.MarketPlace.Application.Contracts.Messaging;
using TechMastery.MarketPlace.Application.Features.ProductListing.Dto;

namespace TechMastery.MarketPlace.Application.Features.ProductListing.Handlers
{
    public class AddOrUpdateListing : IRequest<Guid>
    {
        public Guid ListingId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string DemoUrl { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public CategoryDto Category { get; set; } = new CategoryDto();
        public IList<ProductAssetDto>? UploadAssets { get; set; }
        public IList<ProductDependencyDto>? Dependencies { get; set; }
        public IList<string>? Tags { get; set; }
        public bool IsMarkedForDeletion { get; set; } = false;
    }

    public class AddOrUpdateListingHandler : IRequestHandler<AddOrUpdateListing, Guid>
    {
        private readonly ILogger<AddOrUpdateListingHandler> _logger;
        private readonly IProductRepository _productRepository;
        private readonly IStorageProvider _storageProvider;
        private readonly ICategoryRepository _categoryRepository;
        private readonly IMessagePublisher _messagePublisher;

        public AddOrUpdateListingHandler(ILogger<AddOrUpdateListingHandler> logger, IProductRepository productRepository, IStorageProvider storageProvider, ICategoryRepository categoryRepository, IMessagePublisher messagePublisher)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _storageProvider = storageProvider ?? throw new ArgumentNullException(nameof(storageProvider));
            _productRepository = productRepository ?? throw new ArgumentNullException(nameof(productRepository));
            _categoryRepository = categoryRepository ?? throw new ArgumentNullException(nameof(categoryRepository));
            _messagePublisher = messagePublisher ?? throw new ArgumentNullException(nameof(messagePublisher));
        }

        public async Task<Guid> Handle(AddOrUpdateListing command, CancellationToken cancellationToken)
        {
            ValidateCommand(command);

            var category = await GetCategoryAsync(command.Category.CategoryId);

            Domain.Entities.Product productListing = await GetOrCreateProductListing(command.ListingId, category, command);

            UpdateProductCoreProperties(productListing, command);

            UpdateDependencies(productListing, command.Dependencies);
            await UpdateArtifacts(productListing, command.UploadAssets, cancellationToken);
            MarkArtifactsForDeletion(productListing, command.UploadAssets);

            await SaveProductListing(productListing, command.ListingId);

            await _messagePublisher.PublishAsync<ProductAdded>(new ProductAdded(), "QUEUENAME");

            return productListing.ProductId;
        }

        // UpdateDependencies method: Prevent duplicates and update or add new dependencies
        private void UpdateDependencies(Domain.Entities.Product productListing, IList<ProductDependencyDto>? dependencies)
        {
            if (dependencies == null || !dependencies.Any())
            {
                // No incoming dependencies, clear existing dependencies and exit
                productListing.ClearDependencies();
                return;
            }

            var updatedOrNewDependencies = new List<ProductDependency>();

            foreach (var incomingDependencyDto in dependencies)
            {
                var existingDependency = updatedOrNewDependencies
                    .FirstOrDefault(d => d.Name == incomingDependencyDto.Name && d.DependencyTypeEnum == incomingDependencyDto.DependencyType);

                updatedOrNewDependencies.Add(existingDependency ?? new ProductDependency(
                    incomingDependencyDto.Name,
                    incomingDependencyDto.Version,
                    incomingDependencyDto.DependencyType));
            }

            productListing.UpdateDependencies(updatedOrNewDependencies);
        }


        private void ValidateCommand(AddOrUpdateListing command)
        {
            if (command == null)
            {
                throw new ArgumentNullException(nameof(command));
            }

            if (string.IsNullOrWhiteSpace(command.Name))
            {
                throw new BadRequestException("Product name is required.");
            }

            if (string.IsNullOrWhiteSpace(command.Description))
            {
                throw new BadRequestException("Product description is required.");
            }

            if (command.Price <= 0)
            {
                throw new BadRequestException("Product price must be greater than 0.");
            }
        }

        private async Task<Category> GetCategoryAsync(Guid categoryId)
        {
            var category = await _categoryRepository.GetByIdAsync(categoryId);
            if (category == null)
            {
                throw new NotFoundException("Category", categoryId);
            }
            return category;
        }

        private async Task<Domain.Entities.Product> GetOrCreateProductListing(Guid listingId, Category category, AddOrUpdateListing command)
        {
            if (listingId != Guid.Empty)
            {
                return await GetExistingProductListing(listingId);
            }
            return CreateNewProductListing(category, command);
        }

        private async Task<Domain.Entities.Product> GetExistingProductListing(Guid listingId)
        {
            var productListing = await _productRepository.GetByIdAsync(listingId);
            if (productListing == null)
            {
                throw new NotFoundException("Product", listingId);
            }
            return productListing;
        }

        private Domain.Entities.Product CreateNewProductListing(Category category, AddOrUpdateListing command)
        {
            return new Domain.Entities.Product(
                category.CategoryId,
                command.Name,
                command.Description,
                command.DemoUrl,
                command.Price,
                "",
                "",
                "");
        }

        private void UpdateProductCoreProperties(Domain.Entities.Product productListing, AddOrUpdateListing command)
        {
            productListing.UpdateCoreProperties(
                command.Name,
                command.Description,
                command.DemoUrl,
                command.Price);
        }

        private async Task UpdateArtifacts(Domain.Entities.Product productListing, IList<ProductAssetDto>? uploadAssets, CancellationToken cancellationToken)
        {
            if (uploadAssets == null || !uploadAssets.Any())
            {
                return;
            }

            foreach (var uploadArtifact in uploadAssets)
            {
                if (string.IsNullOrEmpty(uploadArtifact.BlobUrl))
                {
                    var blobUrl = await UploadBlobArtifact(uploadArtifact, cancellationToken);
                    productListing.AddArtifact(new ProductArtifact(uploadArtifact.ArtifactType, blobUrl, DateTime.UtcNow));
                }
            }
        }

        private async Task<string> UploadBlobArtifact(ProductAssetDto uploadAssetData, CancellationToken cancellationToken)
        {
            string fileName = GenerateUniqueFileName(uploadAssetData.FormFile.FileName);
            var blobUri = await _storageProvider.UploadFileAsync(fileName, uploadAssetData.FormFile.OpenReadStream(), cancellationToken);
            return blobUri.ToString();
        }

        private string GenerateUniqueFileName(string originalFileName)
        {
            string fileName = Guid.NewGuid().ToString("N") + Path.GetExtension(originalFileName);
            return fileName;
        }

        private void MarkArtifactsForDeletion(Domain.Entities.Product productListing, IList<ProductAssetDto>? uploadAssets)
        {
            if (uploadAssets == null)
            {
                return;
            }

            foreach (var artifact in productListing.Artifacts)
            {
                var matchingUploadAsset = uploadAssets.FirstOrDefault(a => a.AssetId == artifact.ProductArtifactId);
                if (matchingUploadAsset == null)
                {
                    artifact.MarkForDeletion();
                }
            }
        }

        private async Task SaveProductListing(Domain.Entities.Product productListing, Guid listingId)
        {
            if (listingId == Guid.Empty)
            {
                await _productRepository.AddAsync(productListing);
            }
            else
            {
                await _productRepository.UpdateAsync(productListing);
            }
        }
    }
}
