namespace Vit.Linq.ExpressionTree.ComponentModel
{

    public interface ExpressionNode_Not : IExpressionNode
    {
        public ExpressionNode body { get; set; }
    }


    public partial class ExpressionNode : ExpressionNode_Not
    {
        //public ExpressionNode body { get; set; }

        public static ExpressionNode Not(ExpressionNode body = null)
            => new ExpressionNode { nodeType = NodeType.Not, body = body };
    }
}
