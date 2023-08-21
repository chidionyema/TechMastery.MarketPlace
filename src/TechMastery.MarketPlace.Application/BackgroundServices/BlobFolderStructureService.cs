
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using TechMastery.MarketPlace.Application.Contracts.Infrastructure;
using TechMastery.MarketPlace.Application.Contracts.Persistence;
using TechMastery.MarketPlace.Application.Models;
using TechMastery.MarketPlace.Domain.Entities;

namespace BlobFolderStructureService
{
    public class BlobFolderStructureService : BackgroundService
    {
        private readonly ILogger<BlobFolderStructureService> _logger;
        private readonly IServiceProvider _serviceProvider;

        public BlobFolderStructureService(ILogger<BlobFolderStructureService> logger, IServiceProvider serviceProvider)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("BlobFolderStructureService is starting.");

            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("BlobFolderStructureService is running.");

                try
                {
                    using (var scope = _serviceProvider.CreateScope())
                    {
                        var productRepository = scope.ServiceProvider.GetRequiredService<IProductRepository>();
                        var storageProvider = scope.ServiceProvider.GetRequiredService<IStorageProvider>();
                        //var currentUserService = scope.ServiceProvider.GetRequiredService<ICurrentUserService>();

                        var queryOptions = new QueryOptions<Product>
                        {
                            Filter = product => product.Status == ProductStatusEnum.NewlyListed
                        };

                        var newProducts = await productRepository.GetProductsAsync(queryOptions);

                        foreach (var product in newProducts)
                        {
                            // Replace this with the actual logic to fetch the email of the user who uploaded the product
                            string ownerEmail = "";
                            foreach (ProductArtifactTypeEnum artifactType in Enum.GetValues(typeof(ProductArtifactTypeEnum)))
                            {
                                // check if top level folder exists
                                await storageProvider.CreateBlobFolderStructureAsync(ownerEmail, artifactType.ToString());
                                _logger.LogInformation($"Blob folder structure created for {ownerEmail}/{artifactType}");
                            }
             
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "An error occurred while creating blob folder structure.");
                }

                await Task.Delay(TimeSpan.FromMinutes(5), stoppingToken);
            }

            _logger.LogInformation("BlobFolderStructureService is stopping.");
        }
    }

    // ... (IStorageProvider, IProductRepository, ICurrentUserService, Product, and other classes as shown in previous responses)
}
