using System;
using System.Collections.Generic;
using System.Text;

namespace Vit.Linq.ExpressionTree.ComponentModel
{
    /// <summary>
    /// left:   object
    /// right:  arguments[0]
    /// </summary>
    public interface ExpressionNode_Call : IExpressionNode
    {
        /// <summary>
        ///   the System.Linq.Expressions.Expression that represents the instance for instance method calls or null for static method calls.
        /// </summary>
        public ExpressionNode instance { get; }

        public string methodName { get; }

        public ExpressionNode[] methodArguments { get; }
    }


    public partial class ExpressionNode : ExpressionNode_Call
    {

        /// <summary>
        ///   the System.Linq.Expressions.Expression that represents the instance for instance method calls or null for static method calls.
        /// </summary>
        public ExpressionNode instance { get; set; }

        public string methodName { get; set; }

        public ExpressionNode[] methodArguments { get; set; }

        public static ExpressionNode Call(ExpressionNode instance = null, string methodName = null, ExpressionNode[] methodArguments = null)
        {
            return new ExpressionNode
            {
                nodeType = NodeType.Call,
                instance = instance,
                methodName = methodName,
                methodArguments = methodArguments
            };
        }
    }
}
