using System;
using System.Linq.Expressions;
using TechMastery.MarketPlace.Application.Contracts.Persistence;

namespace TechMastery.MarketPlace.Application.Models
{
    public class QueryOptions<T> : IQueryOptions<T>
    {
        public Expression<Func<T, bool>> Filter { get; set; }
        public Func<IQueryable<T>, IOrderedQueryable<T>> OrderBy { get; set; }
        public int? PageNumber { get; set; }
        public int? PageSize { get; set; }
        public bool IsPagingEnabled => PageNumber.HasValue && PageSize.HasValue;
    }
}

