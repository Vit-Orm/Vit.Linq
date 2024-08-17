using System;
using System.Linq.Expressions;

namespace Vit.Linq.ExpressionNodes.ComponentModel
{

    public interface ExpressionNode_Binary : IExpressionNode
    {
        public ExpressionNode left { get; }
        public ExpressionNode right { get; }
    }

    public interface ExpressionNode_Equal : ExpressionNode_Binary { }
    public interface ExpressionNode_NotEqual : ExpressionNode_Binary { }
    public interface ExpressionNode_LessThan : ExpressionNode_Binary { }
    public interface ExpressionNode_LessThanOrEqual : ExpressionNode_Binary { }
    public interface ExpressionNode_GreaterThan : ExpressionNode_Binary { }
    public interface ExpressionNode_GreaterThanOrEqual : ExpressionNode_Binary { }


    public partial class ExpressionNode :
        ExpressionNode_Equal, ExpressionNode_NotEqual,
        ExpressionNode_LessThan, ExpressionNode_LessThanOrEqual,
        ExpressionNode_GreaterThan, ExpressionNode_GreaterThanOrEqual
    {
        public ExpressionNode left { get; set; }
        public ExpressionNode right { get; set; }


        public static ExpressionNode Binary(string nodeType, ExpressionNode left, ExpressionNode right)
            => new ExpressionNode { expressionType = "Binary", nodeType = nodeType, left = left, right = right };

        public static ExpressionNode Add(ExpressionNode left, ExpressionNode right, Type valueType)
            => new ExpressionNode { expressionType = "Binary", nodeType = nameof(ExpressionType.Add), left = left, right = right, valueType = ComponentModel.NodeValueType.FromType(valueType) };

    }
}
