using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

using Vit.Linq.ExpressionTree.ComponentModel;

namespace Vit.Linq.ExpressionTree
{
    public class CodeConvertArgument
    {
        public List<ParameterExpression> parameters;

        public CodeConvertArgument WithParams(params ParameterExpression[] newParams)
        {
            CodeConvertArgument source = this;

            var arg = new CodeConvertArgument();
            if (source?.parameters?.Any() == true)
            {
                arg.parameters = source.parameters.ToList();
            }
            else
            {
                arg.parameters = new List<ParameterExpression>();
            }
            if (newParams.Any())
            {
                arg.parameters.AddRange(newParams);
            }
            return arg;
        }
    }

    public partial class ExpressionConvertService
    {

        public Func<T, bool> ToPredicate<T>(ExpressionNode data)
        {
            return ToPredicateExpression<T>(data)?.Compile();
        }

        public string ToExpressionString<T>(ExpressionNode data)
        {
            return ToPredicateExpression<T>(data)?.ToString();
        }
        public Expression<Func<T, bool>> ToPredicateExpression<T>(ExpressionNode data, string paramName = null)
        {
            var exp = ToLambdaExpression(data, typeof(T), paramName);
            return (Expression<Func<T, bool>>)exp;
        }

        public LambdaExpression ToLambdaExpression(ExpressionNode data, Type paramType, string paramName = null)
        {
            ExpressionNode_Lambda lambda;

            if (data.nodeType == NodeType.Lambda)
            {
                lambda = data;
            }
            else
            {
                lambda = ExpressionNode.Lambda(parameterNames: new string[] { paramName }, body: data);
            }
            return ToLambdaExpression(null, lambda, paramType);
        }

        public LambdaExpression ToLambdaExpression(CodeConvertArgument arg, ExpressionNode_Lambda lambda, params Type[] paramTypes)
        {
            string[] parameterNames = lambda.parameterNames ?? new string[0];

            var parameters = parameterNames.Select((name, i) =>
            {
                var type = paramTypes.Length > i ? paramTypes[i] : typeof(object);

                if (string.IsNullOrWhiteSpace(name))
                    return Expression.Parameter(type);
                return Expression.Parameter(type, name);
            }).ToList();

            arg = arg.WithParams(parameters.ToArray());

            var expression = ToExpression(arg, lambda.body);
            if (expression == null) return null;

            return Expression.Lambda(expression, parameters);
        }

        public virtual Expression ToExpression(CodeConvertArgument arg, ExpressionNode data)
        {
            if (data == null) return null;

            switch (data.nodeType)
            {
                case NodeType.Member:
                    ExpressionNode_Member member = data;
                    ParameterExpression paramExp = null;
                    if (member.parameterName != null) paramExp = arg.parameters.FirstOrDefault(p => p.Name == member.parameterName);
                    if (paramExp == null) paramExp = arg.parameters.First();

                    if (member.objectValue == null)
                    {
                        return LinqHelp.GetFieldMemberExpression(paramExp, member.memberName);
                    }
                    else
                    {
                        var instanceExp = ToExpression(arg, member.objectValue);
                        return LinqHelp.GetFieldMemberExpression(instanceExp, member.memberName);
                    }
                case NodeType.Constant:
                    ExpressionNode_Constant constant = data;
                    return constant.ConstantToExpression(this);

                case NodeType.Convert:
                    {
                        ExpressionNode_Convert convert = data;
                        var value = ToExpression(arg, convert.body);

                        Type type = convert.valueType?.ToType();
                        if (type == null) type = value?.Type;

                        return Expression.Convert(value, type);
                    }

                case NodeType.And:
                    ExpressionNode_And and = data;
                    return Expression.AndAlso(ToExpression(arg, and.left), ToExpression(arg, and.right));

                case NodeType.Or:
                    ExpressionNode_Or or = data;
                    return Expression.OrElse(ToExpression(arg, or.left), ToExpression(arg, or.right));

                case NodeType.Not:
                    ExpressionNode_Not not = data;
                    return Expression.Not(ToExpression(arg, not.body));

                case NodeType.ArrayIndex:
                    ExpressionNode_ArrayIndex arrayIndex = data;
                    return Expression.ArrayIndex(ToExpression(arg, arrayIndex.left), ToExpression(arg, arrayIndex.right));

                case NodeType.Equal: return Expression.Equal(ToExpression(arg, data.left) ?? Expression.Constant(null), ToExpression(arg, data.right) ?? Expression.Constant(null));

                case NodeType.NotEqual: return Expression.NotEqual(ToExpression(arg, data.left) ?? Expression.Constant(null), ToExpression(arg, data.right) ?? Expression.Constant(null));
                case NodeType.LessThan: return Expression.LessThan(ToExpression(arg, data.left) ?? Expression.Constant(null), ToExpression(arg, data.right) ?? Expression.Constant(null));
                case NodeType.LessThanOrEqual: return Expression.LessThanOrEqual(ToExpression(arg, data.left) ?? Expression.Constant(null), ToExpression(arg, data.right) ?? Expression.Constant(null));
                case NodeType.GreaterThan: return Expression.GreaterThan(ToExpression(arg, data.left) ?? Expression.Constant(null), ToExpression(arg, data.right) ?? Expression.Constant(null));
                case NodeType.GreaterThanOrEqual: return Expression.GreaterThanOrEqual(ToExpression(arg, data.left) ?? Expression.Constant(null), ToExpression(arg, data.right) ?? Expression.Constant(null));

                case NodeType.Call:
                    ExpressionNode_Call call = data;
                    return ConvertMethodToExpression(arg, call);
            }
            throw new NotSupportedException(data.nodeType);
        }


    }
}
