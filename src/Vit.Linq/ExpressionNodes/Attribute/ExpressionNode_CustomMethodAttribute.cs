using System;
using System.Linq.Expressions;

using Vit.Linq.ExpressionNodes.ComponentModel;
using Vit.Linq.ExpressionNodes.ExpressionConvertor;

namespace Vit.Linq.ExpressionNodes
{
    /// <summary>
    /// Mark this method to be able to convert to ExpressionNode from Expression. Method arguments must be ValueType (including string).
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class ExpressionNode_CustomMethodAttribute : Attribute, Vit.Linq.ExpressionNodes.ExpressionConvertor.MethodCalls.IMethodConvertor
    {
        public int priority { get; set; }

        public virtual (bool success, ExpressionNode node) ToData(ToDataArgument arg, MethodCallExpression call) => (true, MethodCall.ConvertToData(arg, call));

        public virtual (bool success, Expression expression) ToCode(ToCodeArgument arg, ExpressionNode_MethodCall call) => throw new NotImplementedException();
    }


}
