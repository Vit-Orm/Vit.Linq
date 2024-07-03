using System.Collections.Generic;
using System.Linq;

using Vit.Linq.ComponentModel;

namespace Vit.Linq
{
    public static partial class Queryable_Extensions
    {

        public static IQueryable<T> OrderByReflection<T>(this IQueryable<T> query, IEnumerable<OrderField> sort)
            where T : class
        {
            if (query == null || sort?.Any() != true) return query;

            IOrderedQueryable<T> orderedQuery = null;
            foreach (var item in sort)
            {
                var keySelector = LinqHelp.GetFieldExpression_ByReflection<T>(item.field);

                if (keySelector == null)
                {
                    continue;
                }
                if (item.asc)
                {
                    if (orderedQuery == null)
                    {
                        orderedQuery = query.OrderBy(keySelector);
                    }
                    else
                    {
                        orderedQuery = orderedQuery.ThenBy(keySelector);
                    }
                }
                else
                {
                    if (orderedQuery == null)
                    {
                        orderedQuery = query.OrderByDescending(keySelector);
                    }
                    else
                    {
                        orderedQuery = orderedQuery.ThenByDescending(keySelector);
                    }
                }
            }
            return orderedQuery;
        }


        public static IQueryable<T> OrderByReflection<T>(this IQueryable<T> query, string field, bool asc = true)
           where T : class
        {
            if (query == null || string.IsNullOrEmpty(field)) return query;

            return OrderByReflection(query, new[] { new OrderField(field, asc) });
        }

    }
}