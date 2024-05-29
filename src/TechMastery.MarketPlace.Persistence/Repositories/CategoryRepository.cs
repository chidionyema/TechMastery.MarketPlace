
using TechMastery.MarketPlace.Domain.Entities;
using TechMastery.MarketPlace.Application.Models;
using TechMastery.MarketPlace.Application.Persistence.Contracts;

namespace TechMastery.MarketPlace.Persistence.Repositories
{
    public class CategoryRepository : BaseRepository<Category>, ICategoryRepository
    {
        public CategoryRepository(ApplicationDbContext dbContext) : base(dbContext) { }

        public async Task<IReadOnlyList<Category>> GetTopLevelCategoriesAsync()
        {
            var options = new QueryOptions<Category>
            {
                Filter = c => c.ParentCategory == null
            };

            return await GetAsync(options);
        }

        public async Task<IReadOnlyList<Category>> GetSubcategoriesAsync(Guid parentId)
        {
            var options = new QueryOptions<Category>
            {
                Filter = c => c.ParentCategory != null && c.ParentCategoryId == parentId
            };

            return await GetAsync(options);
        }
    }
}
