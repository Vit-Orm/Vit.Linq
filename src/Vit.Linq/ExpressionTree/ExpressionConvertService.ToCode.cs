using System;
using System.Linq.Expressions;

using Vit.Linq.ExpressionTree.ComponentModel;

namespace Vit.Linq.ExpressionTree
{
    public partial class ExpressionConvertService
    {
        public virtual LambdaExpression ToLambdaExpression(CodeConvertArgument arg, ExpressionNode_Lambda lambda, params Type[] paramTypes)
        {
            lambda.Lambda_SetParamTypes(paramTypes);
            return ToExpression(arg, lambda as ExpressionNode) as LambdaExpression;
        }

        public virtual Expression ToExpression(CodeConvertArgument arg, ExpressionNode data)
        {
            if (data == null) return null;

            foreach (var expressionConvertor in expressionConvertors)
            {
                var exp = expressionConvertor.ConvertToCode(arg, data);
                if (exp != null) return exp;
            }

            throw new NotSupportedException(data.nodeType);
        }


      

     

    }
}
