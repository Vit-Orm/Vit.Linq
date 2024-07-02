using System;
using System.Linq;
using System.Linq.Expressions;

using Vit.Linq.Filter;
using Vit.Linq.Filter.ComponentModel;

namespace Vit.Linq
{
    public static partial class IQueryable_Where_Extensions
    {
        #region Where
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