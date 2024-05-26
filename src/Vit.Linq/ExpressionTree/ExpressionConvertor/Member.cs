using System;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Xml.Linq;

using Vit.Linq.ExpressionTree.ComponentModel;

namespace Vit.Linq.ExpressionTree.ExpressionConvertor
{

    public class Member : IExpressionConvertor
    {
        public ExpressionNode ConvertToData(DataConvertArgument arg, Expression expression)
        {
            if (expression is MemberExpression member)
            {
                if (typeof(IQueryable).IsAssignableFrom(member.Type) && arg.ReduceValue(member, out object query))
                {
                    var value = query;
                    var type = expression.Type;
                    if (value != null)
                    {
                        return arg.GetParameter(value, type);
                    }
                }

                var name = member.Member?.Name;
                if (member.Expression == null)
                    throw new NotSupportedException($"Unsupported MemberExpression : {name}");

                if (member.Expression is ParameterExpression parameter)
                    return ExpressionNode.Member(parameterName: parameter.Name, memberName: name).Member_SetType(expression.Type);

                var objectValue = arg.convertService.ConvertToData(arg, member.Expression);
                if (objectValue?.nodeType == NodeType.Constant && arg.ReduceValue(member, out object constValue))
                {
                    return ExpressionNode.Constant(value: constValue, type: expression.Type);
                }
                return ExpressionNode.Member(objectValue: objectValue, memberName: name).Member_SetType(expression.Type);
            }
            else if (expression is ParameterExpression parameter)
            {
                return ExpressionNode.Member(parameterName: parameter.Name, memberName: null).Member_SetType(expression.Type);
            }

            return null;
        }

        public Expression ConvertToCode(CodeConvertArgument arg, ExpressionNode data)
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
                var instanceExp = arg.convertService.ToExpression(arg, member.objectValue);
                return LinqHelp.GetFieldMemberExpression(instanceExp, member.memberName);
            }
        }

    }
}
