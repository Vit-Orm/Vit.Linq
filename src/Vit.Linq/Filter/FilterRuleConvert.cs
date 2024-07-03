using System;
using System.Linq;
using System.Linq.Expressions;

using Vit.Linq;
using Vit.Linq.Filter.ComponentModel;
using Vit.Linq.Filter.MethodCalls;

namespace Vit.Linq.Filter
{
    public partial class FilterRuleConvert
    {


        public FilterRuleConvert()
        {
            // populate MethodConvertor
            var types = GetType().Assembly.GetTypes().Where(type => type.IsClass
                    && !type.IsAbstract
                    && typeof(IMethodConvertor).IsAssignableFrom(type)
                    && type.GetConstructor(Type.EmptyTypes) != null
            ).ToList();

            types.ForEach(type => RegisterMethodConvertor(Activator.CreateInstance(type) as IMethodConvertor));
        }


        public virtual IFilterRule ConvertToFilterRule<T>(Expression<Func<T, bool>> predicate)
        {
            return ConvertToQueryAction(predicate.Body).filter;
        }


        public virtual IFilterRule ConvertToFilterRule(Expression expression)
        {
            return ConvertToQueryAction(expression).filter;
        }

        public virtual QueryAction ConvertToQueryAction(Expression expression)
        {
            var queryAction = new QueryAction();
            queryAction.filter = ConvertToFilterRule(queryAction, expression);
            return queryAction;
        }


        public virtual FilterRule ConvertToFilterRule(QueryAction queryAction, Expression expression)
        {
            if (expression is ConstantExpression constant && constant.Value is bool v)
            {
                return new FilterRule { @operator = v ? "true" : "false" };
            }
            else if (expression is BinaryExpression binary)
            {
                return ConvertToData_Binary(queryAction, binary);
            }
            else if (expression is UnaryExpression unary)
            {
                return ConvertToData_Unary(queryAction, unary);
            }
            else if (expression is LambdaExpression lambda)
            {
                return ConvertToData_Lambda(queryAction, lambda);
            }
            else if (expression is MethodCallExpression call)
            {
                return ConvertToData_MethodCall(queryAction, call);
            }

            throw new NotSupportedException($"Unsupported expression type: {expression.GetType()}");
        }


        public virtual object GetValue(Expression expression)
        {
            if (expression is ConstantExpression constant)
            {
                return constant.Value;
            }
            else if (expression is UnaryExpression unary)
            {
                if (ExpressionType.Convert == unary.NodeType)
                {
                    var del = Expression.Lambda(unary).Compile();
                    var value = del.DynamicInvoke();
                    return value;
                }
            }
            else if (expression is MemberExpression member)
            {
                if (ExpressionType.MemberAccess == member.NodeType)
                {
                    var del = Expression.Lambda(member).Compile();
                    var value = del.DynamicInvoke();
                    return value;
                }
            }
            throw new NotSupportedException($"GetValue failed, Unsupported expression type: {expression.GetType()}");
        }


        public virtual string GetMemberName(Expression expression)
        {
            if (expression is ParameterExpression parameter)
            {
                // top level, no need to return parameterName
                return null;
                //return parameter.Name;
            }
            else if (expression is MemberExpression member)
            {
                // get nested member
                var name = member.Member.Name;
                if (member.Expression == null) return name;
                string parentName = GetMemberName(member.Expression);
                return parentName == null ? name : $"{parentName}.{name}";
            }
            else if (expression is UnaryExpression unary)
            {
                if (ExpressionType.Quote == unary.NodeType)
                {
                    return GetMemberName(unary.Operand);
                }
            }
            else if (expression is LambdaExpression lambda)
            {
                return GetMemberName(lambda.Body);
            }

            throw new NotSupportedException($"GetMemberName failed, Unsupported expression type: {expression.GetType()}");
        }




    }
}
