using System;
using System.Linq;
using System.Linq.Expressions;

using Vit.Linq.ExpressionTree.ComponentModel;

namespace Vit.Linq.ExpressionTree
{
    public partial class ExpressionConvertService
    {

        public ExpressionNode ConvertToData(ToDataArgument arg, Expression expression)
        {
            if (expression == null) return null;

            foreach (var expressionConvertor in expressionConvertors)
            {
                var node = expressionConvertor.ConvertToData(arg, expression);
                if (node != null) return node;
            }

            throw new NotSupportedException($"Unsupported expression type: {expression.GetType()}");
        }



        public ExpressionNode_Lambda ConvertToData_LambdaNode(Expression expression, bool autoReduce = true, Func<object, Type, bool> isArgument = null)
        {
            return ConvertToData_LambdaNode(expression, out _, autoReduce: autoReduce, isArgument: isArgument);
        }
        public ExpressionNode_Lambda ConvertToData_LambdaNode(Expression expression, out ParameterInfo[] parameters, bool autoReduce = true, Func<object, Type, bool> isArgument = null)
        {
            var arg = new ToDataArgument { convertService = this, autoReduce = autoReduce, isArgument = isArgument };
            return ConvertToData_LambdaNode(expression, out parameters, arg);
        }
        public ExpressionNode_Lambda ConvertToData_LambdaNode(Expression expression, out ParameterInfo[] parameters, ToDataArgument arg)
        {
            arg.convertService ??= this;

            ExpressionNode body = ConvertToData(arg, expression);

            arg.GenerateGlobalParameterName();
            parameters = arg.globalParameters?.ToArray();
            var parameterNames = parameters?.Select(m => m.parameterName).ToArray();
            return ExpressionNode.Lambda(parameterNames: parameterNames, body: body);
        }



    }
}
