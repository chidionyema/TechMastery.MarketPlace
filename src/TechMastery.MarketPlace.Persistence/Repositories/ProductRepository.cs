using Microsoft.EntityFrameworkCore;
using TechMastery.MarketPlace.Application.Contracts.Persistence;
using TechMastery.MarketPlace.Domain.Entities;

namespace TechMastery.MarketPlace.Persistence.Repositories
{
    public class ProductRepository : BaseRepository<Product>, IProductRepository
    {
        public ProductRepository(ApplicationDbContext dbContext) : base(dbContext) { }

        public async Task<IEnumerable<Product>> GetBestLatestProductsAsync()
        {
            // Retrieve best and latest products logic here
            var products = await _dbContext.Products.OrderByDescending(p => p.Price).ThenByDescending(p => p.CreatedDate).ToListAsync();
            return products;
        }

        public async Task<IEnumerable<Product>> GetBestSellingProductsAsync()
        {
            // Retrieve best selling products logic here
            var products = await _dbContext.Products
                .OrderByDescending(p => p.Orders != null ? p.Orders.Count : 0) // Use 0 if Orders is null
                .ToListAsync();

            return products;
        }

        public async Task<IEnumerable<Product>> GetProductsByStatusAsync(ProductStatusEnum productStatus)
        {
            var products = await _dbContext.Products.Where(p => p.Status == productStatus).ToListAsync();
            return products;
        }

        public async Task<IEnumerable<Product>> GetProductsBySubCategoryAsync(Guid subCategoryId)
        {
            // Retrieve products by _dbContext logic here
            var products = await _dbContext.Products.Where(p => p.CategoryId == subCategoryId).ToListAsync();
            return products;
        }
    }
}
