using Roaa.Rosas.Common.Models;
using System.Linq.Expressions;

namespace Roaa.Rosas.Common.Extensions
{
    public static class EFCoreOrderExtentions
    {
        public static IOrderedQueryable<T> OrderBy<T>(this IQueryable<T> source, SortItem sort)
        {
            return source.OrderBy(sort.Field, sort.Direction.IsDesc());
        }
        public static IOrderedQueryable<T> OrderBy<T>(this IQueryable<T> source, string propertyName, bool descending, bool anotherLevel = false)
        {
            var param = Expression.Parameter(typeof(T), string.Empty);
            var property = Expression.PropertyOrField(param, propertyName);
            var sort = Expression.Lambda(property, param);

            var call = Expression.Call(
                typeof(Queryable),
                (!anotherLevel ? "OrderBy" : "ThenBy") +
                (descending ? "Descending" : string.Empty),
                new[] { typeof(T), property.Type },
                source.Expression,
                Expression.Quote(sort));

            return (IOrderedQueryable<T>)source.Provider.CreateQuery<T>(call);
        }
    }
}
