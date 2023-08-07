
using System.Linq.Expressions;
using TechMastery.MarketPlace.Domain.Entities;

namespace TechMastery.MarketPlace.Application.Contracts.Persistence
{
    public interface IAsyncRepository<T> where T : class
    {
        Task<T?> GetByIdAsync(Guid id);
        Task<IReadOnlyList<T>> FindAsync(Expression<Func<T, bool>> predicate);
        Task<IReadOnlyList<T>> ListAllAsync();
        Task<T> AddAsync(T entity);
        Task UpdateAsync(T entity);
        Task DeleteAsync(T entity);
        Task<IReadOnlyList<T>> GetPagedReponseAsync(int page, int size);
        Task<IReadOnlyList<T>> FindAsync(Expression<Func<T, bool>> predicate, Expression<Func<T, object>> sortExpression = null, SortDirection sortDirection = SortDirection.Ascending, int pageNumber = 1, int pageSize = 10);
    }
}
