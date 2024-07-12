using System;
using System.Linq.Expressions;

using Vit.Linq.ExpressionTree.ComponentModel;

namespace Vit.Linq.ExpressionTree
{
    public partial class ExpressionConvertService
    {
        public virtual Expression ConvertToCode(ToCodeArgument arg, ExpressionNode data)
        {
            if (data == null) return null;

            foreach (var expressionConvertor in expressionConvertors)
            {
                var exp = expressionConvertor.ConvertToCode(arg, data);
                if (exp != null) return exp;
            }

            throw new NotSupportedException(data.nodeType);
        }


        public virtual LambdaExpression ConvertToCode_LambdaExpression(ToCodeArgument arg, ExpressionNode_Lambda lambda, params Type[] paramTypes)
        {
            lambda.Lambda_SetParamTypes(paramTypes);
            return ConvertToCode(arg, lambda as ExpressionNode) as LambdaExpression;
        }




        #region Extension Methods
        public Func<T, bool> ConvertToCode_Predicate<T>(ExpressionNode data)
        {
            return ConvertToCode_PredicateExpression<T>(data)?.Compile();
        }

        public string ConvertToCode_ExpressionString<T>(ExpressionNode data)
        {
            return ConvertToCode_PredicateExpression<T>(data)?.ToString();
        }


        public Expression<Func<T, bool>> ConvertToCode_PredicateExpression<T>(ExpressionNode_Lambda data)
        {
            var exp = ConvertToCode_LambdaExpression(data, typeof(T));
            return (Expression<Func<T, bool>>)exp;
        }

        public LambdaExpression ConvertToCode_LambdaExpression(ExpressionNode_Lambda lambda, params Type[] paramTypes)
        {
            var arg = ToCodeArgument.WithParams(this);
            return ConvertToCode_LambdaExpression(arg, lambda, paramTypes);
        }

        public LambdaExpression ConvertToCode_LambdaExpression(ExpressionNode_Lambda lambda, Type[] paramTypes, Type[] newNodeResultTypes)
        {
            int i = 0;
            var arg = ToCodeArgument.WithParams(this, getResultTypeForNewNode: (newNode) => newNodeResultTypes[i++]);
            return ConvertToCode_LambdaExpression(arg, lambda, paramTypes);
        }
        #endregion




    }
}
