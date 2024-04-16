using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

using Vit.Extensions.Linq_Extensions;

namespace Vit.Linq.ExpressionTree.ComponentModel
{

    public interface ExpressionNode_Convert : IExpressionNode
    {
        public ValueType valueType { get; set; }
        public ExpressionNode body { get; set; }
    }


    public partial class ExpressionNode : ExpressionNode_Convert
    {
        //public ValueType valueType { get; set; }
        //public ExpressionNode body { get; set; }


        public static ExpressionNode Convert(ValueType valueType = null, ExpressionNode body = null)
            => new ExpressionNode { nodeType = NodeType.Convert, valueType = valueType, body = body };

    }
}
