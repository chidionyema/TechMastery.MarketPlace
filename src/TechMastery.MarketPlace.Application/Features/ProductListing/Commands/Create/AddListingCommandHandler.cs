using MediatR;
using Microsoft.Extensions.Logging;
using TechMastery.MarketPlace.Application.Contracts.Messaging;
using TechMastery.MarketPlace.Application.Contracts.Persistence;
using TechMastery.MarketPlace.Application.Exceptions;
using TechMastery.MarketPlace.Domain.Entities;
using TechMastery.MarketPlace.Application.Features.ProductListing.DataTransferObjects;
using TechMastery.MarketPlace.Application.Messaging;

namespace TechMastery.MarketPlace.Application.Features.ProductListing.Handlers
{
    public class AddListingHandler : IRequestHandler<AddListingCommand, Guid>
    {
        private readonly ILogger<AddListingHandler> _logger;
        private readonly IProductRepository _productRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly IMessagePublisher _messagePublisher;

        public AddListingHandler(
            ILogger<AddListingHandler> logger,
            IProductRepository productRepository,
            ICategoryRepository categoryRepository,
            IMessagePublisher messagePublisher)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _productRepository = productRepository ?? throw new ArgumentNullException(nameof(productRepository));
            _categoryRepository = categoryRepository ?? throw new ArgumentNullException(nameof(categoryRepository));
            _messagePublisher = messagePublisher ?? throw new ArgumentNullException(nameof(messagePublisher));
        }

        public async Task<Guid> Handle(AddListingCommand command, CancellationToken cancellationToken)
        {
            ValidateCommand(command);

            var category = await GetCategoryAsync(command.Category.CategoryId);
            var productListing = CreateNewProductListing(category, command);

            AddTags(productListing, command.Tags);
            AddDependencies(productListing, command.Dependencies);

            await _productRepository.AddAsync(productListing);

            await _messagePublisher.PublishAsync<ProductAdded>(new ProductAdded(), "QUEUENAME");

            return productListing.ProductId;
        }

        private void ValidateCommand(AddListingCommand command)
        {
            if (command == null) throw new ArgumentNullException(nameof(command));
            if (string.IsNullOrWhiteSpace(command.Name)) throw new BadRequestException("Product name is required.");
            if (string.IsNullOrWhiteSpace(command.Description)) throw new BadRequestException("Product description is required.");
            if (command.ListingPrice <= 0) throw new BadRequestException("Product price must be greater than 0.");
        }

        private async Task<Category> GetCategoryAsync(Guid categoryId)
        {
            var category = await _categoryRepository.GetByIdAsync(categoryId);
            if (category == null) throw new NotFoundException("Category", categoryId);
            return category;
        }

        private Domain.Entities.Product CreateNewProductListing(Category category, AddListingCommand command)
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

        private void AddTags(Domain.Entities.Product productListing, IReadOnlyList<string> tags)
        {
            if (tags == null || !tags.Any()) return;

            foreach (var tag in tags)
            {
                if (!productListing.Tags.Any(t => t.Name == tag))
                {
                    productListing.AddTag(new ProductTag(tag));
                }
            }
        }

        private void AddDependencies(Domain.Entities.Product productListing, IReadOnlyList<ProductDependencyDto> dependencies)
        {
            if (dependencies == null || !dependencies.Any()) return;

            foreach (var dependency in dependencies)
            {
                if (!productListing.Dependencies.Any(d => d.DependencyId == dependency.DependencyId))
                {
                    var newDependency = new ProductDependency(dependency.DependencyId);
                    productListing.AddDependency(newDependency);
                }
            }
        }
    }
}

