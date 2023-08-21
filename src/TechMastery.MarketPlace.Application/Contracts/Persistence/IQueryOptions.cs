using System;
using System.Linq.Expressions;

namespace TechMastery.MarketPlace.Application.Contracts.Persistence
{
    public interface IQueryOptions<T>
    {
        Expression<Func<T, bool>> Filter { get; set; }
        Func<IQueryable<T>, IOrderedQueryable<T>> OrderBy { get; set; }
        int? PageNumber { get; set; }
        int? PageSize { get; set; }
        bool IsPagingEnabled => PageNumber.HasValue && PageSize.HasValue;
    }

}

