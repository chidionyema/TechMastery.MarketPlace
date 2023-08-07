using TechMastery.MarketPlace.Domain.Entities;
namespace TechMastery.MarketPlace.Application.Contracts.Persistence
{
    public interface IProductRepository : IAsyncRepository<Product>
    {
        Task<IEnumerable<Product>> GetBestLatestProductsAsync();
        Task<IEnumerable<Product>> GetBestSellingProductsAsync();
        Task<IEnumerable<Product>> GetProductsByStatusAsync(ProductStatusEnum productStatus);
        Task<IEnumerable<Product>> GetProductsBySubCategoryAsync(Guid subCategoryId);
    }
}

	