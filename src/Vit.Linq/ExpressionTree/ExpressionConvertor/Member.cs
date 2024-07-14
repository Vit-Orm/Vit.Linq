using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

using Vit.Linq.ExpressionTree.ComponentModel;

namespace Vit.Linq.ExpressionTree.ExpressionConvertor
{

    public class Member : IExpressionConvertor
    {
        public virtual int priority { get; set; } = 100;
        public ExpressionNode ConvertToData(ToDataArgument arg, Expression expression)
        {
            if (expression is MemberExpression member)
            {
                if (arg.ReduceValue(member, out object constValue))
                {
                    return ExpressionNode.Constant(value: constValue, type: expression.Type);
                }

                var name = member.Member?.Name;

                // Get Class status property
                if (member.Expression == null)
                {
                    if (member.Member is PropertyInfo property && property.GetMethod.IsStatic)
                    {
                        var value = property.GetValue(null);
                        return ExpressionNode.Constant(value, type: expression.Type);
                    }
                    if (member.Member is FieldInfo field && field.IsStatic)
                    {
                        var value = field.GetValue(null);
                        return ExpressionNode.Constant(value, type: expression.Type);
                    }
                    throw new NotSupportedException($"Unsupported MemberExpression : {name}");
                }

                if (member.Expression is ParameterExpression parameter)
                    return ExpressionNode.Member(parameterName: parameter.Name, memberName: name).Member_SetType(expression.Type);

                var objectValue = arg.convertService.ConvertToData(arg, member.Expression);

                return ExpressionNode.Member(objectValue: objectValue, memberName: name).Member_SetType(expression.Type);
            }
            else if (expression is ParameterExpression parameter)
            {
                return ExpressionNode.Member(parameterName: parameter.Name, memberName: null).Member_SetType(expression.Type);
            }

            return null;
        }

        public Expression ConvertToCode(ToCodeArgument arg, ExpressionNode data)
        {
            if (data.nodeType != NodeType.Member) return null;

            ExpressionNode_Member member = data;

            if (member.objectValue == null)
            {
                ParameterExpression paramExp = null;
                if (member.parameterName != null) paramExp = arg.parameters.FirstOrDefault(p => p.Name == member.parameterName);

                //if (paramExp == null) paramExp = arg.parameters.First();
                if (paramExp == null) throw new NotSupportedException($"can not find parameter : {member.parameterName}");

                return LinqHelp.GetFieldMemberExpression(paramExp, member.memberName);
            }
            else
            {
                var instanceExp = arg.convertService.ConvertToCode(arg, member.objectValue);
                return LinqHelp.GetFieldMemberExpression(instanceExp, member.memberName);
            }
        }

    }
}
