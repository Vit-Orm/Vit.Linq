

using System.Linq.Expressions;

namespace Vit.Linq.ExpressionTree.ComponentModel
{
    public static class ExpressionNode_Conditional_Extensions
    {
        public static ExpressionNode Conditional_GetTest(this ExpressionNode_Conditional data)
        {
            var node = data as ExpressionNode;
            if (node?.arguments?.Length == 3) return node.arguments[0];
            return null;
        }
        public static ExpressionNode Conditional_GetIfTrue(this ExpressionNode_Conditional data)
        {
            var node = data as ExpressionNode;
            if (node?.arguments?.Length == 3) return node.arguments[1];
            return null;
        }
        public static ExpressionNode Conditional_GetIfFalse(this ExpressionNode_Conditional data)
        {
            var node = data as ExpressionNode;
            if (node?.arguments?.Length == 3) return node.arguments[2];
            return null;
        }
    }
    public interface ExpressionNode_Conditional : IExpressionNode
    {
    }

    public partial class ExpressionNode : ExpressionNode_Conditional
    {
        public static ExpressionNode Conditional(ExpressionNode test, ExpressionNode ifTrue, ExpressionNode ifFalse)
        {
            return new ExpressionNode
            {
                nodeType = nameof(ExpressionType.Conditional),
                arguments = new ExpressionNode[] { test, ifTrue, ifFalse }
            };
        }

    }
}
