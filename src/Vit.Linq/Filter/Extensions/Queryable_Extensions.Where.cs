using System.Linq;

using Vit.Linq.Filter;
using Vit.Linq.Filter.ComponentModel;

namespace Vit.Linq
{
    public static partial class Queryable_Extensions
    {

        public static IQueryable<T> Where<T>(this IQueryable<T> query, IFilterRule filter, FilterService filterService = null)
        {
            if (query == null || filter == null) return query;

            var predicate = (filterService ?? FilterService.Instance).ToExpression<T>(filter);
            if (predicate == null)
            {
                return query;
            }
            else
            {
                return query.Where(predicate);
            }
        }


    }
}