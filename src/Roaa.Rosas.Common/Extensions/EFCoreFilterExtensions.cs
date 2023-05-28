using Roaa.Rosas.Common.Models;
using System.Linq.Expressions;

namespace Roaa.Rosas.Common.Extensions
{
    public static class EFCoreFilterExtensions
    {
        public static Expression<Func<TEntity, bool>> BuildWhereExpression<TEntity>(string propertyName, object value, FilterOperator @operator = FilterOperator.Equal)
        {
            var parameter = Expression.Parameter(typeof(TEntity), "x");
            var property = Expression.Property(parameter, propertyName);
            var constant = Expression.Constant(value);

            BinaryExpression body;
            switch (@operator)
            {
                case FilterOperator.Contains:
                    var method = typeof(string).GetMethod("Contains", new[] { typeof(string) });
                    var contains = Expression.Call(property, method, constant);
                    body = Expression.MakeBinary(ExpressionType.AndAlso, contains, Expression.NotEqual(property, Expression.Constant(null, property.Type)));
                    break;
                case FilterOperator.GreaterThanOrEqual:
                    if (property.Type == typeof(DateTime) || property.Type == typeof(DateTime?))
                    {
                        var dateValue = DateTime.Parse(value.ToString());
                        constant = Expression.Constant(dateValue);
                        body = Expression.GreaterThanOrEqual(property, constant);
                    }
                    else
                    {
                        body = Expression.GreaterThanOrEqual(property, constant);
                    }
                    break;
                case FilterOperator.LessThanOrEqual:
                    if (property.Type == typeof(DateTime) || property.Type == typeof(DateTime?))
                    {
                        var dateValue = DateTime.Parse(value.ToString());
                        constant = Expression.Constant(dateValue);
                        body = Expression.LessThanOrEqual(property, constant);
                    }
                    else
                    {
                        body = Expression.LessThanOrEqual(property, constant);
                    }
                    break;
                default:
                    body = Expression.Equal(property, constant);
                    break;
            }
            return Expression.Lambda<Func<TEntity, bool>>(body, parameter);
        }
        public static Expression<Func<TEntity, bool>> BuildWhereExpression<TEntity>(string[] propertyNames, string value)
        {


            var parameter = Expression.Parameter(typeof(TEntity), "x");
            Expression body = null;

            for (var i = 0; i < propertyNames.Count(); i++)
            {
                var propertyName = propertyNames[i];
                var property = Expression.Property(parameter, propertyName);
                var constant = Expression.Constant(value, typeof(string));
                var method = typeof(string).GetMethod("Contains", new[] { typeof(string) });
                var contains = Expression.Call(property, method, constant);
                var nullCheck = Expression.NotEqual(property, Expression.Constant(null, property.Type));

                if (i == 0)
                {
                    body = Expression.MakeBinary(ExpressionType.AndAlso, contains, nullCheck);
                }
                else
                {
                    var andAlso = Expression.MakeBinary(ExpressionType.AndAlso, contains, nullCheck);
                    body = Expression.MakeBinary(ExpressionType.Or, body, andAlso);
                }
            }

            return Expression.Lambda<Func<TEntity, bool>>(body, parameter);
        }
        private static FilterOperator TryGetOptimizeOperator(this FilterItem filter)
        {
            switch (filter.Field.ToLower())
            {
                case "fromdate":
                    return FilterOperator.GreaterThanOrEqual;
                case "todate":
                    return FilterOperator.LessThanOrEqual;
                default:
                    return filter.Operator;
            }
        }

        public static IQueryable<TSource> Where<TSource>(this IQueryable<TSource> source, List<FilterItem> filters, string[] filterColumns, string betweenDateFilter = null)
        {
            if (filters is not null)
            {
                foreach (var filter in filters)
                {
                    if (!string.IsNullOrWhiteSpace(filter.Field) && !string.IsNullOrWhiteSpace(filter.Value))
                    {
                        string propertyName = filter.Field;

                        switch (filter.Field.ToLower())
                        {

                            case "fromdate":
                                {
                                    if (string.IsNullOrWhiteSpace(betweenDateFilter))
                                        break;
                                    propertyName = betweenDateFilter;
                                    break;
                                }
                            case "todate":
                                {
                                    if (string.IsNullOrWhiteSpace(betweenDateFilter))
                                        break;
                                    propertyName = betweenDateFilter;
                                    break;
                                }
                            case "searchterm":
                                {
                                    if (filterColumns.Any(x => x.StartsWith('_')))
                                    {
                                        propertyName = string.Empty;
                                    }
                                    break;
                                }
                            default:
                                {
                                    if (!filterColumns.Any(column => filter.Field.Equals(column.TrimStart('_'), StringComparison.OrdinalIgnoreCase)))
                                        break;
                                    propertyName = filterColumns.First(column => filter.Field.Equals(column.TrimStart('_'), StringComparison.OrdinalIgnoreCase));
                                    break;
                                }
                        }

                        // filter by search-term
                        if (string.IsNullOrWhiteSpace(propertyName))
                        {
                            source = source.Where(BuildWhereExpression<TSource>(filterColumns.Where(x => x.StartsWith('_')).Select(x => x.TrimStart('_')).ToArray(), filter.Value));
                        }
                        else
                        {
                            source = source.Where(BuildWhereExpression<TSource>(propertyName.TrimStart('_'), filter.Value, filter.TryGetOptimizeOperator()));
                        }
                    }
                }
            }
            return source;
        }


        /// <summary>
        /// Tries find the value of the specified filter field name
        /// </summary>
        /// <param name="filters"></param>
        /// <param name="fieldName">filter field name</param>
        /// <returns>nullable filter value</returns>
        public static string TryFind(this List<FilterItem> filters, string fieldName)
        {
            if (filters != null && filters.Any())
            {
                foreach (var filter in filters)
                {
                    if (!string.IsNullOrEmpty(fieldName) && filter.Field == fieldName)
                    {
                        return filter.Value;
                    }
                }
            }

            return null;
        }
    }
}
