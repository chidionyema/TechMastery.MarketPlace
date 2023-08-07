using TechMastery.MarketPlace.Domain.Entities;
namespace TechMastery.MarketPlace.Application.Contracts.Persistence
{
    public interface ICategoryRepository : IAsyncRepository<Category>
    {
        Task<List<Category>> GetTopLevelCategoriesAsync();

        Task<List<Category>> GetSubcategoriesAsync(Guid parentId);
    }
}