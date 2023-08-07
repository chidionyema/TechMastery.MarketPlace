using System;
using System.Linq.Expressions;
namespace TechMastery.MarketPlace.Domain.Entities
{
    public class SearchCriteria<T>
    {
        public Expression<Func<T, bool>> Filter { get; set; }
        public List<Expression<Func<T, object>>> Includes { get; } = new List<Expression<Func<T, object>>>();
        public Expression<Func<T, object>> OrderBy { get; set; }
        public Expression<Func<T, object>> OrderByDescending { get; set; }
        public int? Skip { get; set; }
        public int? Take { get; set; }
        public bool IsPagingEnabled => Skip.HasValue && Take.HasValue;

        public void AddInclude(Expression<Func<T, object>> includeExpression)
        {
            Includes.Add(includeExpression);
        }
    }

    public enum SortDirection
    {
        Ascending,
        Descending
    }

}

