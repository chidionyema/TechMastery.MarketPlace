using TechMastery.MarketPlace.Application.Contracts.Persistence;
using TechMastery.MarketPlace.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace TechMastery.MarketPlace.Persistence.Repositories
{
    public class CategoryRepository : BaseRepository<Category>, ICategoryRepository
    {
        public CategoryRepository(ApplicationDbContext dbContext) : base(dbContext) { }

        public async Task<List<Category>> GetTopLevelCategoriesAsync()
        {
            return await _dbContext.Categories
                .Where(c => c.ParentCategory == null)
                .ToListAsync();
        }

        public async Task<List<Category>> GetSubcategoriesAsync(Guid parentId)
        {
            return await _dbContext.Categories
                .Where(c => c.ParentCategory != null && c.ParentCategoryId == parentId)
                .ToListAsync();
        }
    }
}
