using MediatR;
using Microsoft.Extensions.Logging;
using TechMastery.MarketPlace.Application.Contracts;
using TechMastery.MarketPlace.Application.Exceptions;
using TechMastery.MarketPlace.Domain.Entities;
using TechMastery.MarketPlace.Application.Messaging;
using TechMastery.MarketPlace.Application.Persistence.Contracts;

namespace TechMastery.MarketPlace.Application.Features.ProductListing.Handlers
{
    public class AddListingHandler : IRequestHandler<AddListingCommand, Guid>
    {
        private readonly ILogger<AddListingHandler> _logger;
        private readonly IProductRepository _productRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly IAsyncRepository<Language> _languageService;
        private readonly IAsyncRepository<Framework> _frameworkService;
        private readonly IAsyncRepository<Platform> _toolService;
        private readonly IMessagePublisher _messagePublisher;

        public AddListingHandler(
            ILogger<AddListingHandler> logger,
            IProductRepository productRepository,
            ICategoryRepository categoryRepository,
            IAsyncRepository<Language> languageService,
            IAsyncRepository<Framework> frameworkService,
            IAsyncRepository<Platform> toolService,
            IMessagePublisher messagePublisher)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _productRepository = productRepository ?? throw new ArgumentNullException(nameof(productRepository));
            _categoryRepository = categoryRepository ?? throw new ArgumentNullException(nameof(categoryRepository));
            _languageService = languageService ?? throw new ArgumentNullException(nameof(languageService));
            _frameworkService = frameworkService ?? throw new ArgumentNullException(nameof(frameworkService));
            _toolService = toolService ?? throw new ArgumentNullException(nameof(toolService));
            _messagePublisher = messagePublisher ?? throw new ArgumentNullException(nameof(messagePublisher));
        }

        public async Task<Guid> Handle(AddListingCommand command, CancellationToken cancellationToken)
        {
            _logger.LogInformation($"Adding new listing: {command.Name}");

            ValidateCommand(command);

            var category = await _categoryRepository.GetByIdAsync(command.Category.CategoryId, cancellationToken) ?? throw new NotFoundException(nameof(Category), command.Category.CategoryId);

            // Retrieve entities based on provided IDs
            var languages = await _languageService.GetManyByIdAsync(command.LanguageIds, cancellationToken);
            var frameworks = await _frameworkService.GetManyByIdAsync(command.FrameworkIds, cancellationToken);
            var platforms = await _toolService.GetManyByIdAsync(command.PlatformIds, cancellationToken);

            // Create the Product entity
            var productListing = new Domain.Entities.Product(
                category.Id,
                command.Name,
                command.Description,
                command.DemoUrl,
                command.ListingPrice,
                "LICENSE",
                "OWNER",
                "");

            // Associate languages, frameworks, and platforms
            foreach (var language in languages)
            {
                if (!productListing.ProductLanguages.Any(pl => pl.LanguageId == language.Id))
                {
                    productListing.ProductLanguages.Add(new ProductLanguage { LanguageId = language.Id });
                }

            }

            foreach (var framework in frameworks)
            {
                if (!productListing.ProductFrameworks.Any(pl => pl.FrameworkId == framework.Id))
                {
                    productListing.ProductFrameworks.Add(new ProductFramework { FrameworkId = framework.Id });
                }
            }

            foreach (var platform in platforms)
            {
                if (!productListing.ProductPlatforms.Any(pl => pl.PlatformId == platform.Id))
                {
                    productListing.ProductPlatforms.Add(new ProductPlatform { PlatformId = platform.Id });
                }
            }

            foreach(var tag in command.Tags)
            {
                productListing.AddTag(new ProductTag(tag));
            }

            await _productRepository.AddAsync(productListing, cancellationToken);
            _logger.LogInformation($"Listing added with ID: {productListing.Id}");

            // Publish event and return ProductId...
            var productAddedEvent = new ProductAdded { ProductId = productListing.Id };
            await _messagePublisher.PublishAsync(productAddedEvent, "ProductAddedQueue", cancellationToken);
            _logger.LogInformation($"Published ProductAdded event for Product ID: {productListing.Id}");

            return productListing.Id;
        }

        private static void ValidateCommand(AddListingCommand command)
        {
            if (command == null) throw new ArgumentNullException(nameof(command));
            if (string.IsNullOrWhiteSpace(command.Name)) throw new BadRequestException("Product name is required.");
            if (string.IsNullOrWhiteSpace(command.Description)) throw new BadRequestException("Product description is required.");
            if (command.ListingPrice <= 0) throw new BadRequestException("Product price must be greater than 0.");
        }
    }
}
