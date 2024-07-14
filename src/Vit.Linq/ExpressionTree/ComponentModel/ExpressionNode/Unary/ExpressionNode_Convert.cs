

namespace Vit.Linq.ExpressionTree.ComponentModel
{

    public interface ExpressionNode_Convert : IExpressionNode
    {
        public NodeValueType valueType { get; }

        public ExpressionNode body { get; }
    }

    public partial class ExpressionNode : ExpressionNode_Convert
    {

        //public ValueType valueType { get; set; }

        //public ExpressionNode body { get; set; }

        public static ExpressionNode Convert(NodeValueType valueType = null, ExpressionNode body = null)
        {
            return new ExpressionNode
            {
                nodeType = NodeType.Convert,
                expressionType = "Unary",
                valueType = valueType,
                body = body,
            };
        }

    }
}
