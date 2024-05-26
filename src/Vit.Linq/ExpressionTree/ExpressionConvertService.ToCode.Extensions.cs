using System;
using System.Linq.Expressions;

using Vit.Linq.ExpressionTree.ComponentModel;

namespace Vit.Linq.ExpressionTree
{

    public partial class ExpressionConvertService
    {
        public Func<T, bool> ToPredicate<T>(ExpressionNode data)
        {
            return ToPredicateExpression<T>(data)?.Compile();
        }

        public string ToExpressionString<T>(ExpressionNode data)
        {
            return ToPredicateExpression<T>(data)?.ToString();
        }


        public Expression<Func<T, bool>> ToPredicateExpression<T>(ExpressionNode data)
        {
            var exp = ToLambdaExpression(data, typeof(T));
            return (Expression<Func<T, bool>>)exp;
        }

        public LambdaExpression ToLambdaExpression(ExpressionNode_Lambda lambda, params Type[] paramTypes)
        {
            var arg = CodeConvertArgument.WithParams(this);
            return ToLambdaExpression(arg, lambda, paramTypes);
        }

        public LambdaExpression ToLambdaExpression(ExpressionNode_Lambda lambda, Type[] paramTypes, Type[] newNodeResultTypes)
        {
            int i = 0;
            var arg = CodeConvertArgument.WithParams(this, getResultTypeForNewNode: (newNode) => newNodeResultTypes[i++]);
            return ToLambdaExpression(arg, lambda, paramTypes);
        }

    }
}
