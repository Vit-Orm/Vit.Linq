using System;
using System.Linq.Expressions;

using Vit.Linq.ExpressionNodes.ComponentModel;

namespace Vit.Linq.ExpressionNodes.ExpressionConvertor.MethodCalls
{
    /// <summary>
    /// Mark this method to be able to convert to ExpressionNode from Expression. Method arguments must be ValueType (including string).
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class ExpressionNode_CustomMethodAttribute : Attribute, IMethodConvertor
    {
        public int priority { get; set; } = 90;

        public virtual bool PredicateToData(ToDataArgument arg, MethodCallExpression call) => true;
        public virtual ExpressionNode ToData(ToDataArgument arg, MethodCallExpression call) => MethodCall.ConvertToData(arg, call);


        public virtual bool PredicateToCode(ToCodeArgument arg, ExpressionNode_MethodCall call) => true;
        public virtual Expression ToCode(ToCodeArgument arg, ExpressionNode_MethodCall call) => throw new NotImplementedException();
    }


}
