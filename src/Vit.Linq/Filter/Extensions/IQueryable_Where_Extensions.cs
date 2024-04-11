using System;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using Vit.Linq.Filter;
using Vit.Linq.Filter.ComponentModel;

namespace Vit.Extensions.Linq_Extensions
{
    public static partial class IQueryable_Where_Extensions
    {
        #region Where

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IQueryable IQueryable_Where(this IQueryable query, IFilterRule rule, FilterService service = null)
        {
            LambdaExpression lambda = (service ?? FilterService.Instance).ToLambdaExpression(rule, query.ElementType);
            if (lambda == null) return query;
            return query.Provider.CreateQuery(
                Expression.Call(
                    typeof(Queryable), "Where",
                    new Type[] { query.ElementType },
                    query.Expression, Expression.Quote(lambda)));
        }
        #endregion

    }
}