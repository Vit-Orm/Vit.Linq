using System;
using System.Linq;
using System.Linq.Expressions;

using Vit.Linq.ExpressionTree;
using Vit.Linq.ExpressionTree.ComponentModel;

namespace Vit.Linq
{
    public static partial class IQueryable_Extensions
    {
        public static IQueryable IQueryable_Where(this IQueryable query, ExpressionNode_Lambda lambda, ExpressionConvertService expressionConvertService = null)
        {
            LambdaExpression lambdaExpression = (expressionConvertService ?? ExpressionConvertService.Instance).ConvertToCode_LambdaExpression(lambda, query.ElementType);
            if (lambdaExpression == null) return query;
            return query.Provider.CreateQuery(
                Expression.Call(
                    typeof(Queryable), nameof(Queryable.Where),
                    new Type[] { query.ElementType },
                    query.Expression, Expression.Quote(lambdaExpression)));
        }


    }
}