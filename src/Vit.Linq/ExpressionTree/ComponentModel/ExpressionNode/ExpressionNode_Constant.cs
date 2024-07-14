using System;
using System.Linq;


namespace Vit.Linq.ExpressionTree.ComponentModel
{

    public interface ExpressionNode_Constant : IExpressionNode
    {
        NodeValueType valueType { get; set; }

        object value { get; set; }

        ExpressionNode Constant_SetType(Type type);
        Type Constant_GetType();
    }

    public partial class ExpressionNode : ExpressionNode_Constant
    {

        public NodeValueType valueType { get; set; }

        public object value { get; set; }

        public ExpressionNode Constant_SetType(Type type)
        {
            SetCodeArg("Constant_Type", type);
            return this;
        }
        public Type Constant_GetType()
        {
            return GetCodeArg("Constant_Type") as Type;
        }



        public static ExpressionNode Constant(object value = null, Type type = null)
        {
            if (value is IQueryable query)
            {
                value = query.IQueryable_ToList();
            }

            var valueType = NodeValueType.FromType(type);

            var node = new ExpressionNode
            {
                nodeType = NodeType.Constant,
                value = value,
                valueType = valueType,
            };
            if (type != null) node.Constant_SetType(type);
            return node;
        }

    }



}
