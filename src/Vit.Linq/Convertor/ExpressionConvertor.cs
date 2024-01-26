using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

using Vit.Linq.ComponentModel;
using Vit.Linq.Filter;

namespace Vit.Linq.Convertor
{
    public class ExpressionConvertor
    {
        public class ConvertArg
        {
            public QueryParam query = new QueryParam();

            public bool gettedOrder = false;
        }

        public virtual IFilterRule ConvertToFilterRule<T>(Expression<Func<T, bool>> predicate)
        {
            return ConvertToQueryParam(predicate.Body).filter;
        }


        public virtual IFilterRule ConvertToFilterRule(Expression expression)
        {
            return ConvertToQueryParam(expression).filter;
        }

        public virtual QueryParam ConvertToQueryParam(Expression expression)
        {
            var arg = new ConvertArg();
            arg.query.filter = ConvertToFilterRule(arg, expression);
            return arg.query;
        }


        protected virtual FilterRule ConvertToFilterRule(ConvertArg arg, Expression expression)
        {
            if (expression is ConstantExpression constant && constant.Value is bool v)
            {
                return new FilterRule { @operator = v ? "true" : "false" };
            }
            else if (expression is BinaryExpression binary)
            {
                return ConvertBinaryExpressionToFilterRule(arg, binary);
            }
            else if (expression is UnaryExpression unary)
            {
                return ConvertUnaryExpressionToFilterRule(arg, unary);
            }
            else if (expression is LambdaExpression lambda)
            {
                return ConvertLambdaExpressionToFilterRule(arg, lambda);
            }
            else if (expression is MethodCallExpression methodExp)
            {
                return ConvertMethodExpressionToFilterRule(arg, methodExp);
            }

            throw new NotSupportedException($"Unsupported expression type: {expression.GetType()}");
        }

        protected virtual FilterRule ConvertLambdaExpressionToFilterRule(ConvertArg arg, LambdaExpression lambda) 
        {
            return ConvertToFilterRule(arg, lambda.Body);
        }

        protected virtual FilterRule ConvertUnaryExpressionToFilterRule(ConvertArg arg, UnaryExpression unary)
        {
            switch (unary.NodeType)
            {
                case ExpressionType.Not:
                    var rule = ConvertToFilterRule(arg, unary.Operand);
                    if (rule.condition?.StartsWith(FilterRuleCondition.Not, StringComparison.OrdinalIgnoreCase) == true)
                    {
                        rule.condition = rule.condition.Substring(3);
                    }
                    else
                    {
                        rule.condition = FilterRuleCondition.Not;
                    }
                    return rule;
                case ExpressionType.Quote:
                    var exp = unary.Operand;
                    return ConvertToFilterRule(arg, exp);
            }
            throw new NotSupportedException($"Unsupported binary operator: {unary.NodeType}");
        }

        protected virtual FilterRule ConvertBinaryExpressionToFilterRule(ConvertArg arg, BinaryExpression binary)
        {
            switch (binary.NodeType)
            {
                case ExpressionType.Equal:
                case ExpressionType.NotEqual:
                case ExpressionType.GreaterThan:
                case ExpressionType.GreaterThanOrEqual:
                case ExpressionType.LessThan:
                case ExpressionType.LessThanOrEqual:
                    string opt = null;
                    switch (binary.NodeType)
                    {
                        case ExpressionType.Equal: opt = "="; break;
                        case ExpressionType.NotEqual: opt = "!="; break;
                        case ExpressionType.GreaterThan: opt = ">"; break;
                        case ExpressionType.GreaterThanOrEqual: opt = ">="; break;
                        case ExpressionType.LessThan: opt = "<"; break;
                        case ExpressionType.LessThanOrEqual: opt = "<="; break;
                    }
                    var left = GetMemberName(binary.Left);
                    var right = GetValue(binary.Right);
                    return new FilterRule { field = left?.ToString(), @operator = opt, value = right };

                case ExpressionType.AndAlso:
                    return new FilterRule { condition = "and", rules = new List<FilterRule> { ConvertToFilterRule(arg, binary.Left), ConvertToFilterRule(arg, binary.Right) } };

                case ExpressionType.OrElse:
                    return new FilterRule { condition = "or", rules = new List<FilterRule> { ConvertToFilterRule(arg, binary.Left), ConvertToFilterRule(arg, binary.Right) } };
            }
            throw new NotSupportedException($"Unsupported binary operator: {binary.NodeType}");
        }


        protected virtual object GetValue(Expression expression)
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
            throw new NotSupportedException($"GetValue failed, Unsupported expression type: {expression.GetType()}");
        }


        protected virtual string GetMemberName(Expression expression)
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



        #region MethodConvertor
        protected class MethodConvertor
        {
            //public int? order;
            public Func<ConvertArg, MethodCallExpression, bool> predicate;
            public Func<ConvertArg, MethodCallExpression, FilterRule> convert;
        }
        protected List<MethodConvertor> methodConvertors = new List<MethodConvertor>();

        public virtual void RegisterMethodConvertor(Func<ConvertArg, MethodCallExpression, bool> predicate, Func<ConvertArg, MethodCallExpression, FilterRule> convertor)
        {
            methodConvertors.Add(new MethodConvertor { predicate = predicate, convert = convertor });
        }
        public virtual void RegisterMethodConvertor(Type operatorType, string operatorName, Func<ConvertArg, MethodCallExpression, FilterRule> convertor)
        {
            Func<ConvertArg, MethodCallExpression, bool> predicate = (arg, methodExp) =>
            {
                var type = methodExp.Method.DeclaringType;
                var name = methodExp.Method.Name;
                return operatorType == type && name == operatorName;
            };
            RegisterMethodConvertor(predicate, convertor);
        }

        public virtual void RegisterMethodsConvertor(Type operatorType, string operatorName, Func<ConvertArg, MethodCallExpression, FilterRule> convertor)
        {
            Func<ConvertArg, MethodCallExpression, bool> predicate = (arg, methodExp) =>
            {
                var type = methodExp.Method.DeclaringType;
                var name = methodExp.Method.Name;
                return operatorType.IsAssignableFrom(type) && name == operatorName;
            };
            RegisterMethodConvertor(predicate, convertor);
        }

        public ExpressionConvertor()
        {
            Func<ConvertArg, MethodCallExpression, FilterRule> convertor;


            // #1 string.Contains
            convertor = (arg, method) =>
            {
                var left = GetMemberName(method.Object);
                var right = GetValue(method.Arguments[0]);
                return new FilterRule { field = left?.ToString(), @operator = "Contains", value = right };
            };
            RegisterMethodsConvertor(typeof(string), "Contains", convertor);


            // #2 string.IsNullOrEmpty
            convertor = (arg, method) =>
            {
                string left = GetMemberName(method.Arguments[0]);
                return new FilterRule { field = left, @operator = "IsNull" };
            };
            RegisterMethodsConvertor(typeof(string), "IsNullOrEmpty", convertor);


            // #3 IEnumerable.Contains
            convertor = (arg, method) =>
            {
                var left = GetMemberName(method.Object);
                var right = GetValue(method.Arguments[0]);
                return new FilterRule { field = left?.ToString(), @operator = "In", value = right };
            };
            RegisterMethodsConvertor(typeof(IEnumerable), "Contains", convertor);

            #region #4 Queryable.Take Queryable.Skip
            // # Queryable.Take
            convertor = (arg, method) =>
            {
                // method.Arguments   [0]: expression       [1]: takeCount
                var expression = method.Arguments[0];
                var take = (int)GetValue(method.Arguments[1]);
                arg.query.take = take;

                return ConvertToFilterRule(arg, expression);
            };
            RegisterMethodConvertor(typeof(Queryable), "Take", convertor);

            // # Queryable.Skip
            convertor = (arg, method) =>
            {
                // method.Arguments   [0]: expression       [1]: skipCount
                var expression = method.Arguments[0];
                var skip = (int)GetValue(method.Arguments[1]);
                arg.query.skip = skip;

                return ConvertToFilterRule(arg, expression);
            };
            RegisterMethodConvertor(typeof(Queryable), "Skip", convertor);
            #endregion

            #region #5 Queryable   OrderBy OrderByDescending  ThenBy  ThenByDescending
            // # Queryable.OrderBy
            convertor = (arg, method) =>
            {
                // method.Arguments   [0]: expression       [1]: takeCount
                var expression = method.Arguments[0];
                var memberName = GetMemberName(method.Arguments[1]);

                if (!arg.gettedOrder)
                {
                    var methodName = method.Method.Name;
                    var orderParam = new OrderParam(memberName, !methodName.EndsWith("Descending"));

                    if (methodName.StartsWith("Order"))
                    {
                        arg.gettedOrder = true;
                    }

                    if (arg.query.orders == null)
                        arg.query.orders = new List<OrderParam> { orderParam };
                    else
                        arg.query.orders.Insert(0, orderParam);
                }

                return ConvertToFilterRule(arg, expression);
            };
            RegisterMethodConvertor(typeof(Queryable), "OrderBy", convertor);
            RegisterMethodConvertor(typeof(Queryable), "OrderByDescending", convertor);
            RegisterMethodConvertor(typeof(Queryable), "ThenBy", convertor);
            RegisterMethodConvertor(typeof(Queryable), "ThenByDescending", convertor);
            #endregion



            #region #6 Queryable.Where
            convertor = (arg, method) =>
            {
                // method.Arguments   [0]: expression       [1]: Where
                var expression = method.Arguments[0];
                var where = method.Arguments[1];
                if (expression is ConstantExpression)
                {
                    return ConvertToFilterRule(arg, where);
                }

                return new FilterRule { condition = "and", rules = new List<FilterRule> { ConvertToFilterRule(arg, expression), ConvertToFilterRule(arg, where) } };
            };
            RegisterMethodConvertor(typeof(Queryable), "Where", convertor);
            #endregion


        }
        #endregion

        protected virtual FilterRule ConvertMethodExpressionToFilterRule(ConvertArg arg, MethodCallExpression methodExp)
        {
            var convertor = methodConvertors.FirstOrDefault(m => m.predicate(arg, methodExp));

            return convertor?.convert(arg, methodExp) ?? throw new NotSupportedException($"Unsupported method call: {methodExp.Method.Name}");
        }
    }
}
