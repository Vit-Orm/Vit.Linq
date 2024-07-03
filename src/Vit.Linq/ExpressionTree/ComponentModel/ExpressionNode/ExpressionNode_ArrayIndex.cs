namespace Vit.Linq.ExpressionTree.ComponentModel
{

    /// <summary>
    /// left:  object
    /// right:  arguments[0]
    /// </summary>
    public interface ExpressionNode_ArrayIndex : ExpressionNode_Binary
    {
        //public ExpressionNode left { get; }
        //public ExpressionNode right { get; }
    }

    public partial class ExpressionNode : ExpressionNode_ArrayIndex
    {
        //public ExpressionNode left { get; set; }
        //public ExpressionNode right { get; set; }


        public static ExpressionNode ArrayIndex(ExpressionNode left = null, ExpressionNode right = null)
            => new ExpressionNode { nodeType = NodeType.ArrayIndex, expressionType = "Binary", left = left, right = right };
    }
}
