using System;
using System.Linq.Expressions;
namespace TechMastery.MarketPlace.Application.Contracts.Persistence
{
    public interface IAsyncRepository<T> where T : class
    {
        ValueTask<T?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

        Task<IReadOnlyList<T>> ListAllAsync(CancellationToken cancellationToken = default);

        Task<T> AddAsync(T entity, CancellationToken cancellationToken = default);

        Task UpdateAsync(T entity, CancellationToken cancellationToken = default);

        Task DeleteAsync(T entity, CancellationToken cancellationToken = default);

        Task ExecuteWithinTransactionAsync(Func<Task> operation, CancellationToken cancellationToken = default);

        Task<IReadOnlyList<T>> GetAsync(IQueryOptions<T> options, CancellationToken cancellationToken = default);
    }
}
