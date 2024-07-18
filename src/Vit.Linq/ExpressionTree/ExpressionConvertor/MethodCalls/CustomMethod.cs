using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

using Vit.Linq.ExpressionTree.ComponentModel;

namespace Vit.Linq.ExpressionTree.ExpressionConvertor.MethodCalls
{



    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class CustomMethodAttribute : Attribute
    {
        public CustomMethodAttribute()
        {
        }
        public virtual bool PredicateToData(ToDataArgument arg, MethodCallExpression call) => true;
        public virtual ExpressionNode ToData(ToDataArgument arg, MethodCallExpression call) => MethodConvertor_Base.ConvertToData(arg, call);


        public virtual bool PredicateToCode(ToCodeArgument arg, ExpressionNode_MethodCall call) => true;
        public virtual Expression ToCode(ToCodeArgument arg, ExpressionNode_MethodCall call) => throw new NotImplementedException();
    }



    public class CustomMethod : IMethodConvertor
    {
        public int priority => 90;

        public bool PredicateToData(ToDataArgument arg, MethodCallExpression call)
        {
            var attr = call.Method.GetCustomAttributes(typeof(CustomMethodAttribute), true)?.FirstOrDefault()
                ?? call.Method.DeclaringType.GetCustomAttribute(typeof(CustomMethodAttribute), true)
                ;
            var convertor = attr as CustomMethodAttribute;
            return convertor?.PredicateToData(arg, call) == true;
        }
        public ExpressionNode ToData(ToDataArgument arg, MethodCallExpression call)
        {
            var attr = call.Method.GetCustomAttributes(typeof(CustomMethodAttribute), true)?.FirstOrDefault()
               ?? call.Method.DeclaringType.GetCustomAttribute(typeof(CustomMethodAttribute), true)
                ;
            var convertor = attr as CustomMethodAttribute;

            return convertor.ToData(arg, call).SetCodeArg("MethodCall_CustomMethod", convertor);
        }


        public bool PredicateToCode(ToCodeArgument arg, ExpressionNode_MethodCall call)
        {
            var convertor = call.GetCodeArg("MethodCall_CustomMethod") as CustomMethodAttribute;
            return convertor?.PredicateToCode(arg, call) == true;
        }

        public Expression ToCode(ToCodeArgument arg, ExpressionNode_MethodCall call)
        {
            var convertor = call.GetCodeArg("MethodCall_CustomMethod") as CustomMethodAttribute;
            return convertor.ToCode(arg, call);
        }
    }


}
