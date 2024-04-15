using System;
using System.Collections.Generic;
using System.Text;

namespace Vit.Linq.ExpressionTree.ComponentModel
{
    public interface ExpressionNode_And : IExpressionNode
    {
        public ExpressionNode left { get; }
        public ExpressionNode right { get; }
    }
    public interface ExpressionNode_Or : IExpressionNode
    {
        public ExpressionNode left { get; }
        public ExpressionNode right { get; }
    }


    public partial class ExpressionNode : ExpressionNode_And, ExpressionNode_Or
    {
        public ExpressionNode left { get; set; }
        public ExpressionNode right { get; set; }


        public static ExpressionNode And(ExpressionNode left = null, ExpressionNode right = null)
            => new ExpressionNode { nodeType = NodeType.And, left = left, right = right };

        public static ExpressionNode Or(ExpressionNode left = null, ExpressionNode right = null)
            => new ExpressionNode { nodeType = NodeType.Or, left = left, right = right };
    }
}
