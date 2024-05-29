using TechMastery.MarketPlace.Domain.Entities;
using TechMastery.MarketPlace.Application.Contracts;
namespace TechMastery.MarketPlace.Application.Persistence.Contracts
{
    public interface ICategoryRepository : IAsyncRepository<Category>
    {
        Task<IReadOnlyList<Category>> GetTopLevelCategoriesAsync();

        Task<IReadOnlyList<Category>> GetSubcategoriesAsync(Guid parentId);
    }
}