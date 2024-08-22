using System.Linq.Expressions;

using Vit.Linq.ExpressionNodes.ComponentModel;

namespace Vit.Linq.ExpressionNodes.ExpressionConvertor.MethodCalls
{
    public abstract class MethodConvertor_Base : IMethodConvertor
    {
        public virtual int priority { get; set; } = 100;

        (bool success, ExpressionNode node) IMethodConvertor.ToData(ToDataArgument arg, MethodCallExpression call)
        {
            return PredicateToData(arg, call) ? (true, ToData(arg, call)) : default;
        }

        public abstract bool PredicateToData(ToDataArgument arg, MethodCallExpression call);
        public virtual new ExpressionNode ToData(ToDataArgument arg, MethodCallExpression call) => MethodCall.ConvertToData(arg, call);




        (bool success, Expression expression) IMethodConvertor.ToCode(ToCodeArgument arg, ExpressionNode_MethodCall call)
        {
            return PredicateToCode(arg, call) ? (true, ToCode(arg, call)) : default;
        }
        public abstract bool PredicateToCode(ToCodeArgument arg, ExpressionNode_MethodCall call);
        public abstract new Expression ToCode(ToCodeArgument arg, ExpressionNode_MethodCall call);


    }
}
