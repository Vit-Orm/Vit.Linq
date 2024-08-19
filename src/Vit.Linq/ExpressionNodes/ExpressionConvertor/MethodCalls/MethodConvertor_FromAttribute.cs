using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

using Vit.Linq.ExpressionNodes.ComponentModel;

namespace Vit.Linq.ExpressionNodes.ExpressionConvertor.MethodCalls
{
    public class MethodConvertor_FromAttribute : IMethodConvertor
    {
        public int priority => 100000;

        public static IMethodConvertor GetConvertor(MethodInfo method)
        {
            var attr = method.GetCustomAttributes(true).FirstOrDefault(attr => attr is IMethodConvertor)
                ?? method.DeclaringType.GetCustomAttributes(true).FirstOrDefault(attr => attr is IMethodConvertor)
                ;
            return attr as IMethodConvertor;
        }

        public (bool success, ExpressionNode node) ToData(ToDataArgument arg, MethodCallExpression call)
        {
            var convertor = GetConvertor(call.Method);
            if (convertor == null) return default;

            var result = convertor.ToData(arg, call);
            result.node?.SetCodeArg("MethodCall_Convertor", convertor);
            return result;
        }

        public (bool success, Expression expression) ToCode(ToCodeArgument arg, ExpressionNode_MethodCall call)
        {
            var convertor = call.GetCodeArg("MethodCall_Convertor") as IMethodConvertor;
            if (convertor == null) return default;

            return convertor.ToCode(arg, call);
        }
    }


}
