using System.Linq;

using Vit.Linq.Filter;
using Vit.Linq.Filter.ComponentModel;

namespace Vit.Linq
{
    public static partial class Queryable_Where_Extensions
    {

        public static IQueryable<T> Where<T>(this IQueryable<T> query, IFilterRule rule, FilterService service = null)
        {
            if (query == null || rule == null) return query;

            var predicate = (service ?? FilterService.Instance).ToExpression<T>(rule);
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