using System;
using System.Linq;

using Vit.Linq;


namespace Vit.Linq.ExpressionTree.ComponentModel
{

    public interface ExpressionNode_Constant : IExpressionNode
    {
        public ValueType valueType { get; set; }

        public object value { get; set; }

    }

    public partial class ExpressionNode : ExpressionNode_Constant
    {

        public ValueType valueType { get; set; }

        public object value { get; set; }


        public static ExpressionNode Constant(object value = null, Type type = null)
        {
            if (value is IQueryable query)
            {
                value = query.IQueryable_ToList();
            }

            var valueType = ValueType.FromType(type);

            return new ExpressionNode
            {
                nodeType = NodeType.Constant,
                value = value,
                valueType = valueType,
            };
        }

    }


   
}
