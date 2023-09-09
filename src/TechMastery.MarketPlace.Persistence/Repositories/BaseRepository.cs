using Microsoft.EntityFrameworkCore;
using TechMastery.MarketPlace.Application.Contracts.Persistence;

namespace TechMastery.MarketPlace.Persistence.Repositories
{
    public class BaseRepository<T> : IAsyncRepository<T> where T : class
    {
        protected readonly ApplicationDbContext _dbContext;
        protected readonly DbSet<T> _dbSet;

        public BaseRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
            _dbSet = _dbContext.Set<T>();
        }

        /// <summary>
        /// Retrieves an entity by its unique identifier.
        /// </summary>
        /// <param name="id">The unique identifier of the entity.</param>
        /// <param name="cancellationToken">A token to observe while waiting for the task to complete.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the entity found, or null.</returns>
        public virtual async ValueTask<T?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await _dbSet.FindAsync(new object[] { id }, cancellationToken);
        }

        /// <summary>
        /// Retrieves all entities from the database.
        /// </summary>
        /// <param name="cancellationToken">A token to observe while waiting for the task to complete.</param>
        /// <returns>A list of entities.</returns>
        public async Task<IReadOnlyList<T>> ListAllAsync(CancellationToken cancellationToken = default)
        {
            return await _dbSet.AsNoTracking().ToListAsync(cancellationToken);
        }

        /// <summary>
        /// Adds an entity to the database.
        /// </summary>
        /// <param name="entity">Entity to be added.</param>
        /// <param name="cancellationToken">A token to observe while waiting for the task to complete.</param>
        /// <returns>The added entity.</returns>
        public async Task<T> AddAsync(T entity, CancellationToken cancellationToken = default)
        {
            await _dbSet.AddAsync(entity, cancellationToken);
            await _dbContext.SaveChangesAsync(cancellationToken);
            return entity;
        }

        /// <summary>
        /// Updates an existing entity in the database.
        /// </summary>
        /// <param name="entity">Entity to be updated.</param>
        /// <param name="cancellationToken">A token to observe while waiting for the task to complete.</param>
        public async Task UpdateAsync(T entity, CancellationToken cancellationToken = default)
        {
            _dbSet.Update(entity);
            await _dbContext.SaveChangesAsync(cancellationToken);
        }

        /// <summary>
        /// Deletes an entity from the database.
        /// </summary>
        /// <param name="entity">Entity to be deleted.</param>
        /// <param name="cancellationToken">A token to observe while waiting for the task to complete.</param>
        public async Task DeleteAsync(T entity, CancellationToken cancellationToken = default)
        {
            _dbSet.Remove(entity);
            await _dbContext.SaveChangesAsync(cancellationToken);
        }

        /// <summary>
        /// Executes a given operation within a database transaction.
        /// </summary>
        /// <param name="operation">The operation to execute.</param>
        /// <param name="cancellationToken">A token to observe while waiting for the task to complete.</param>
        public async Task ExecuteWithinTransactionAsync(Func<Task> operation, CancellationToken cancellationToken = default)
        {
            using var transaction = await _dbContext.Database.BeginTransactionAsync(cancellationToken);
            await operation();
            await transaction.CommitAsync(cancellationToken);
        }

        /// <summary>
        /// Retrieves entities based on the given query options.
        /// </summary>
        /// <param name="options">The query options to apply.</param>
        /// <param name="cancellationToken">A token to observe while waiting for the task to complete.</param>
        /// <returns>A list of entities matching the query options.</returns>
        public async Task<IReadOnlyList<T>> GetAsync(IQueryOptions<T> options, CancellationToken cancellationToken = default)
        {
            var query = _dbContext.Set<T>().AsNoTracking().AsQueryable();

            if (options.Filter != null)
            {
                query = query.Where(options.Filter);
            }

            if (options.OrderBy != null)
            {
                query = options.OrderBy(query);
            }

            if (options.IsPagingEnabled)
            {
                query = query.Skip((options.PageNumber.Value - 1) * options.PageSize.Value).Take(options.PageSize.Value);
            }

            return await query.ToListAsync(cancellationToken);
        }
    }
}
