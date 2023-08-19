using Microsoft.Extensions.Logging;
using MediatR;
using TechMastery.MarketPlace.Application.Contracts.Persistence;
using TechMastery.MarketPlace.Application.Exceptions;
using TechMastery.MarketPlace.Domain.Entities;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TechMastery.MarketPlace.Application.Contracts.Messaging;
using TechMastery.MarketPlace.Application.Features.ProductListing.DataTransferObjects;
using TechMastery.MarketPlace.Application.Messaging;

namespace TechMastery.MarketPlace.Application.Features.ProductListing.Handlers
{
    public class AddOrUpdateListingHandler : IRequestHandler<AddOrUpdateListing, Guid>
    {
        private readonly ILogger<AddOrUpdateListingHandler> _logger;
        private readonly IProductRepository _productRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly IMessagePublisher _messagePublisher;

        public AddOrUpdateListingHandler(
            ILogger<AddOrUpdateListingHandler> logger,
            IProductRepository productRepository,
            ICategoryRepository categoryRepository,
            IMessagePublisher messagePublisher)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _productRepository = productRepository ?? throw new ArgumentNullException(nameof(productRepository));
            _categoryRepository = categoryRepository ?? throw new ArgumentNullException(nameof(categoryRepository));
            _messagePublisher = messagePublisher ?? throw new ArgumentNullException(nameof(messagePublisher));
        }

        public async Task<Guid> Handle(AddOrUpdateListing command, CancellationToken cancellationToken)
        {
            ValidateCommand(command);
            var category = await GetCategoryAsync(command.Category.CategoryId);
            Domain.Entities.Product productListing = await GetOrCreateProductListing(command.ListingId, category, command);

            // Assuming you still need to manage Tags and Dependencies
            ProcessTags(productListing, command.Tags);
            ProcessDependencies(productListing, command.Dependencies);

            await SaveProductListing(productListing, command.ListingId);

            await _messagePublisher.PublishAsync<ProductAdded>(new ProductAdded(), "QUEUENAME");

            return productListing.ProductId;
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
            if (command.ListingPrice <= 0)
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
                command.ListingPrice,
                "", // Assuming placeholders for missing parameters
                "",
                "");
        }

        private void ProcessTags(Domain.Entities.Product productListing, IReadOnlyList<string> tags)
        {
            if (tags == null || !tags.Any())
            {
                productListing.ClearTags();
                return;
            }

            // Remove tags that no longer exist in the provided tags
            var tagsToRemove = productListing.Tags.Where(t => !tags.Contains(t.Name)).ToList();
            foreach (var tagToRemove in tagsToRemove)
            {
                productListing.RemoveTag(tagToRemove);
            }

            // Add new tags from the provided tags
            foreach (var tag in tags)
            {
                if (!productListing.Tags.Select(s => s.Name).Contains(tag))
                {
                    productListing.AddTag(new ProductTag(tag));
                }
            }
        }

        private void ProcessDependencies(Domain.Entities.Product productListing, IReadOnlyList<ProductDependencyDto>? dependencies)
        {
            if (dependencies == null || !dependencies.Any())
            {
                productListing.ClearDependencies();
                return;
            }

            // Remove dependencies that no longer exist in the provided dependencies
            var dependenciesToRemove = productListing.Dependencies.Where(d => !dependencies.Any(dep => dep.DependencyId == d.DependencyId)).ToList();
            foreach (var dependencyToRemove in dependenciesToRemove)
            {
                productListing.RemoveDependency(dependencyToRemove);
            }

            // Add new dependencies from the provided list
            foreach (var dependency in dependencies)
            {
                if (!productListing.Dependencies.Any(d => d.DependencyId == dependency.DependencyId))
                {
                    var newDependency = new ProductDependency(dependency.DependencyId);

                    productListing.AddDependency(newDependency);
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
