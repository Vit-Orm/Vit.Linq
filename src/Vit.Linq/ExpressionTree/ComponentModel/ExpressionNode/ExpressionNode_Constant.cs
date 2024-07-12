using System;
using System.Linq;


namespace Vit.Linq.ExpressionTree.ComponentModel
{

    public interface ExpressionNode_Constant : IExpressionNode
    {
        public NodeValueType valueType { get; set; }

        public object value { get; set; }

    }

    public partial class ExpressionNode : ExpressionNode_Constant
    {

        public NodeValueType valueType { get; set; }

        public object value { get; set; }


        public static ExpressionNode Constant(object value = null, Type type = null)
        {
            if (value is IQueryable query)
            {
                value = query.IQueryable_ToList();
            }

            var valueType = NodeValueType.FromType(type);

            return new ExpressionNode
            {
                nodeType = NodeType.Constant,
                value = value,
                valueType = valueType,
            };
        }

    }



}
