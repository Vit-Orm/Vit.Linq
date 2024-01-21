using System.Linq;
using System.Runtime.CompilerServices;
using Vit.Linq.ComponentModel;
using Vit.Linq.Filter;

namespace Vit.Extensions.Linq_Extensions
{
    public static partial class Queryable_Where_Extensions
    {
        #region Where
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IQueryable<T> Where<T>(this IQueryable<T> query, IFilterRule rule, FilterService service = null)
        where T : class
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
        #endregion

    }
}