using TechMastery.MarketPlace.Domain.Entities;
namespace TechMastery.MarketPlace.Application.Contracts.Persistence
{
    public interface ICategoryRepository : IAsyncRepository<Category>
    {
        Task<IReadOnlyList<Category>> GetTopLevelCategoriesAsync();

        Task<IReadOnlyList<Category>> GetSubcategoriesAsync(Guid parentId);
    }
}