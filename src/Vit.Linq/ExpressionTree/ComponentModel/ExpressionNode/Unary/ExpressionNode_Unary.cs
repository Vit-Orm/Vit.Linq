namespace Vit.Linq.ExpressionTree.ComponentModel
{
    public interface ExpressionNode_Unary : IExpressionNode
    {
        public ExpressionNode body { get; }
    }

    public partial class ExpressionNode : ExpressionNode_Unary
    {

        //public ExpressionNode body { get; set; }

        public static ExpressionNode Unary(string nodeType, ExpressionNode body = null)
        {
            return new ExpressionNode
            {
                nodeType = nodeType,
                expressionType = "Unary",
                body = body,
            };
        }

    }
}
