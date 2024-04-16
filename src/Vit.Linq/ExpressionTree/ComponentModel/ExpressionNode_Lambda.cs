namespace Vit.Linq.ExpressionTree.ComponentModel
{

    public interface ExpressionNode_Lambda : IExpressionNode
    {
        public string[] parameterNames { get; set; }
        public ExpressionNode body { get; set; }
    }


    public partial class ExpressionNode : ExpressionNode_Lambda
    {
        public string[] parameterNames { get; set; }
        public ExpressionNode body { get; set; }


        public static ExpressionNode Lambda(string[] parameterNames = null, ExpressionNode body = null)
            => new ExpressionNode { nodeType = NodeType.Lambda, parameterNames = parameterNames, body = body };

    }
}
