using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

using Vit.Linq;
using Vit.Linq.ComponentModel;

namespace Vit.Extensions.Linq_Extensions
{

    public static partial class Queryable_OrderBy_Reflection_Extensions
    {

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IQueryable<T> OrderBy_Reflection<T>(this IQueryable<T> query, IEnumerable<OrderParam> sort)
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


        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="query"></param>
        /// <param name="field"></param>
        /// <param name="asc"> whether sort by asc</param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IQueryable<T> OrderBy_Reflection<T>(this IQueryable<T> query, string field, bool asc = true)
           where T : class
        {
            if (query == null || string.IsNullOrEmpty(field)) return query;

            return OrderBy_Reflection(query, new[] { new OrderParam(field, asc) });
        }

    }
}