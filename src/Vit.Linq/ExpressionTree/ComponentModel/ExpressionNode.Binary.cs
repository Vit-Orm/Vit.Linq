using System;
using System.Collections.Generic;
using System.Text;

namespace Vit.Linq.ExpressionTree.ComponentModel
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
        //public ExpressionNode left { get; set; }
        //public ExpressionNode right { get; set; }


        public static ExpressionNode Equal(ExpressionNode left = null, ExpressionNode right = null)
            => new ExpressionNode { nodeType = NodeType.Equal, left = left, right = right };
        public static ExpressionNode NotEqual(ExpressionNode left = null, ExpressionNode right = null)
            => new ExpressionNode { nodeType = NodeType.NotEqual, left = left, right = right };
        public static ExpressionNode LessThan(ExpressionNode left = null, ExpressionNode right = null)
            => new ExpressionNode { nodeType = NodeType.LessThan, left = left, right = right };
        public static ExpressionNode LessThanOrEqual(ExpressionNode left = null, ExpressionNode right = null)
            => new ExpressionNode { nodeType = NodeType.LessThanOrEqual, left = left, right = right };
        public static ExpressionNode GreaterThan(ExpressionNode left = null, ExpressionNode right = null)
            => new ExpressionNode { nodeType = NodeType.GreaterThan, left = left, right = right };
        public static ExpressionNode GreaterThanOrEqual(ExpressionNode left = null, ExpressionNode right = null)
            => new ExpressionNode { nodeType = NodeType.GreaterThanOrEqual, left = left, right = right };
    }
}
