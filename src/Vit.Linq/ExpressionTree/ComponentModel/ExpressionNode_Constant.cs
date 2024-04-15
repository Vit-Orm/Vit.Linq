using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

using Vit.Extensions.Linq_Extensions;

namespace Vit.Linq.ExpressionTree.ComponentModel
{

    public interface ExpressionNode_Constant : IExpressionNode
    {
        public object value { get; set; }
        public ValueType valueType { get; set; }

        public Expression ConstantToExpression(
            //ExpressionConvertService service
            );
    }


    public partial class ExpressionNode : ExpressionNode_Constant
    {
        public object value { get; set; }
        public ValueType valueType { get; set; }


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

        public Expression ConstantToExpression(
                //ExpressionConvertService service
                )
        {
            var value = this.value;
            Type type = this.valueType?.ToType();
            if (type == null) return Expression.Constant(value);

            if (value != null && !type.IsAssignableFrom(value.GetType()))
            {
                //value = service.ConvertToType(value, type);
            }
            return Expression.Constant(value, type);
        }
    }
}
