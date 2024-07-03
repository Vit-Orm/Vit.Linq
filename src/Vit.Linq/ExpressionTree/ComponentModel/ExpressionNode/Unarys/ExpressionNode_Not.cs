

namespace Vit.Linq.ExpressionTree.ComponentModel
{
    public interface ExpressionNode_Not : IExpressionNode
    {
        public ExpressionNode body { get; }
    }

    public partial class ExpressionNode : ExpressionNode_Not
    {
        //public ExpressionNode body { get; set; }

        public static ExpressionNode Not(ExpressionNode body = null)
        {
            return new ExpressionNode
            {
                nodeType = NodeType.Not,
                expressionType = "Unary",
                body = body,
            };
        }
    }
}
