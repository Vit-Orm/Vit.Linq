using System;
using System.Collections.Generic;
using System.Text;

using Vit.Linq.Filter;

 

namespace Vit.Linq.ExpressionTree.ComponentModel
{

    public interface ExpressionNode_Convert : IExpressionNode
    {
        public ValueType valueType { get;  }

        public ExpressionNode body { get;   }
    }

    public partial class ExpressionNode : ExpressionNode_Convert
    {

        //public ValueType valueType { get; set; }

        //public ExpressionNode body { get; set; }

        public static ExpressionNode Convert(ValueType valueType = null, ExpressionNode body = null)
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
