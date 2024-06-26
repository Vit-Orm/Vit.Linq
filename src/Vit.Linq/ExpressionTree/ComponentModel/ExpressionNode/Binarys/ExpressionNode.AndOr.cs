namespace Vit.Linq.ExpressionTree.ComponentModel
{

    public interface ExpressionNode_AndAlso : IExpressionNode
    {
        public ExpressionNode left { get; }
        public ExpressionNode right { get; }
    }

    public interface ExpressionNode_OrElse : IExpressionNode
    {
        public ExpressionNode left { get; }
        public ExpressionNode right { get; }
    }

    public partial class ExpressionNode : ExpressionNode_AndAlso, ExpressionNode_OrElse
    {
        //public ExpressionNode left { get; set; }
        //public ExpressionNode right { get; set; }

        public static ExpressionNode AndAlso(ExpressionNode left = null, ExpressionNode right = null)
             => new ExpressionNode { nodeType = NodeType.AndAlso, expressionType = "Binary", left = left, right = right };

        public static ExpressionNode OrElse(ExpressionNode left = null, ExpressionNode right = null)
             => new ExpressionNode { nodeType = NodeType.OrElse, expressionType = "Binary", left = left, right = right };

    }
}
