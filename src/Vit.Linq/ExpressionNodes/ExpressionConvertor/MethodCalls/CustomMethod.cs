using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

using Vit.Linq.ExpressionNodes.ComponentModel;

namespace Vit.Linq.ExpressionNodes.ExpressionConvertor.MethodCalls
{

    public class CustomMethod : IMethodConvertor
    {
        public int priority => 90;

        public static IMethodConvertor GetConvertor(MethodInfo method)
        {
            var attr = method.GetCustomAttributes(true).FirstOrDefault(attr => attr is IMethodConvertor)
                ?? method.DeclaringType.GetCustomAttributes(true).FirstOrDefault(attr => attr is IMethodConvertor)
                ;
            return attr as IMethodConvertor;
        }

        public bool PredicateToData(ToDataArgument arg, MethodCallExpression call)
        {
            var convertor = GetConvertor(call.Method);
            return convertor?.PredicateToData(arg, call) == true;
        }
        public ExpressionNode ToData(ToDataArgument arg, MethodCallExpression call)
        {
            var convertor = GetConvertor(call.Method);
            return convertor.ToData(arg, call).SetCodeArg("MethodCall_Convertor", convertor);
        }


        public bool PredicateToCode(ToCodeArgument arg, ExpressionNode_MethodCall call)
        {
            var convertor = call.GetCodeArg("MethodCall_Convertor") as IMethodConvertor;
            return convertor?.PredicateToCode(arg, call) == true;
        }

        public Expression ToCode(ToCodeArgument arg, ExpressionNode_MethodCall call)
        {
            var convertor = call.GetCodeArg("MethodCall_Convertor") as IMethodConvertor;
            return convertor.ToCode(arg, call);
        }
    }


}
