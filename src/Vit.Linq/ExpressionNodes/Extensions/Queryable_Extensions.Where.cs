using System.Linq;

using Vit.Linq.ExpressionNodes;
using Vit.Linq.ExpressionNodes.ComponentModel;

namespace Vit.Linq
{
    public static partial class Queryable_Extensions
    {

        public static IQueryable<T> Where<T>(this IQueryable<T> query, ExpressionNode_Lambda lambda, ExpressionConvertService expressionConvertService = null)
        {
            if (query == null || lambda == null) return query;

            var predicate = (expressionConvertService ?? ExpressionConvertService.Instance).ConvertToCode_PredicateExpression<T>(lambda);
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