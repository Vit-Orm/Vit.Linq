using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Vit.Extensions.Linq_Extensions;
using Vit.Linq.ComponentModel;
using Vit.Linq.Filter.ComponentModel;

namespace Vit.Linq.Filter
{
    public class FilterRuleConvert
    {
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
            queryAction.RemoveKey("_gettedOrder");
            return queryAction;
        }


        protected virtual FilterRule ConvertToFilterRule(QueryAction queryAction, Expression expression)
        {
            if (expression is ConstantExpression constant && constant.Value is bool v)
            {
                return new FilterRule { @operator = v ? "true" : "false" };
            }
            else if (expression is BinaryExpression binary)
            {
                return ConvertBinaryExpressionToFilterRule(queryAction, binary);
            }
            else if (expression is UnaryExpression unary)
            {
                return ConvertUnaryExpressionToFilterRule(queryAction, unary);
            }
            else if (expression is LambdaExpression lambda)
            {
                return ConvertLambdaExpressionToFilterRule(queryAction, lambda);
            }
            else if (expression is MethodCallExpression methodExp)
            {
                return ConvertMethodExpressionToFilterRule(queryAction, methodExp);
            }

            throw new NotSupportedException($"Unsupported expression type: {expression.GetType()}");
        }

        protected virtual FilterRule ConvertLambdaExpressionToFilterRule(QueryAction queryAction, LambdaExpression lambda)
        {
            return ConvertToFilterRule(queryAction, lambda.Body);
        }

        protected virtual FilterRule ConvertUnaryExpressionToFilterRule(QueryAction queryAction, UnaryExpression unary)
        {
            switch (unary.NodeType)
            {
                case ExpressionType.Not:
                    var rule = ConvertToFilterRule(queryAction, unary.Operand);
                    if (rule.condition?.StartsWith(RuleCondition.Not, StringComparison.OrdinalIgnoreCase) == true)
                    {
                        rule.condition = rule.condition.Substring(3);
                    }
                    else
                    {
                        rule.condition = RuleCondition.Not;
                    }
                    return rule;
                case ExpressionType.Quote:
                    var exp = unary.Operand;
                    return ConvertToFilterRule(queryAction, exp);
            }
            throw new NotSupportedException($"Unsupported binary operator: {unary.NodeType}");
        }

        protected virtual FilterRule ConvertBinaryExpressionToFilterRule(QueryAction queryAction, BinaryExpression binary)
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
                    return new FilterRule { condition = "and", rules = new List<FilterRule> { ConvertToFilterRule(queryAction, binary.Left), ConvertToFilterRule(queryAction, binary.Right) } };

                case ExpressionType.OrElse:
                    return new FilterRule { condition = "or", rules = new List<FilterRule> { ConvertToFilterRule(queryAction, binary.Left), ConvertToFilterRule(queryAction, binary.Right) } };
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



        #region MethodConverter
        protected class MethodConverter
        {
            //public int? order;
            public Func<QueryAction, MethodCallExpression, bool> predicate;
            public Func<QueryAction, MethodCallExpression, FilterRule> converter;
        }
        protected List<MethodConverter> methodConvertors = new List<MethodConverter>();

        public virtual void RegisterMethodConvertor(Func<QueryAction, MethodCallExpression, bool> predicate, Func<QueryAction, MethodCallExpression, FilterRule> converter)
        {
            methodConvertors.Add(new MethodConverter { predicate = predicate, converter = converter });
        }
        public virtual void RegisterMethodConvertor(Type operatorType, string operatorName, Func<QueryAction, MethodCallExpression, FilterRule> convertor)
        {
            Func<QueryAction, MethodCallExpression, bool> predicate = (arg, methodExp) =>
            {
                var type = methodExp.Method.DeclaringType;
                var name = methodExp.Method.Name;
                return operatorType == type && name == operatorName;
            };
            RegisterMethodConvertor(predicate, convertor);
        }

        public virtual void RegisterMethodsConvertor(Type operatorType, string operatorName, Func<QueryAction, MethodCallExpression, FilterRule> convertor)
        {
            Func<QueryAction, MethodCallExpression, bool> predicate = (queryAction, methodExp) =>
            {
                var type = methodExp.Method.DeclaringType;
                var name = methodExp.Method.Name;
                return operatorType.IsAssignableFrom(type) && name == operatorName;
            };
            RegisterMethodConvertor(predicate, convertor);
        }

        public FilterRuleConvert()
        {
            Func<QueryAction, MethodCallExpression, FilterRule> converter;
            Func<QueryAction, MethodCallExpression, bool> predicate;


            // #1 string.Contains
            converter = (queryAction, method) =>
            {
                var left = GetMemberName(method.Object);
                var right = GetValue(method.Arguments[0]);
                return new FilterRule { field = left?.ToString(), @operator = "Contains", value = right };
            };
            RegisterMethodsConvertor(typeof(string), "Contains", converter);


            // #2 string.IsNullOrEmpty
            converter = (queryAction, method) =>
            {
                string left = GetMemberName(method.Arguments[0]);
                return new FilterRule { field = left, @operator = "IsNull" };
            };
            RegisterMethodsConvertor(typeof(string), "IsNullOrEmpty", converter);


            // #3 Enumerable.Contains
            converter = (queryAction, method) =>
            {
                var left = GetMemberName(method.Arguments[1]);
                var right = GetValue(method.Arguments[0]);
                return new FilterRule { field = left, @operator = "In", value = right };
            };
            RegisterMethodConvertor(typeof(Enumerable), "Contains", converter);


            #region #4 Queryable.Take Queryable.Skip
            // # Queryable.Take
            converter = (queryAction, method) =>
            {
                // method.Arguments   [0]: expression       [1]: takeCount
                var expression = method.Arguments[0];
                var take = (int)GetValue(method.Arguments[1]);
                queryAction.take = take;

                return ConvertToFilterRule(queryAction, expression);
            };
            RegisterMethodConvertor(typeof(Queryable), "Take", converter);

            // # Queryable.Skip
            converter = (queryAction, method) =>
            {
                // method.Arguments   [0]: expression       [1]: skipCount
                var expression = method.Arguments[0];
                var skip = (int)GetValue(method.Arguments[1]);
                queryAction.skip = skip;

                return ConvertToFilterRule(queryAction, expression);
            };
            RegisterMethodConvertor(typeof(Queryable), "Skip", converter);
            #endregion

            #region #5 Queryable   OrderBy OrderByDescending  ThenBy  ThenByDescending
            // # Queryable.OrderBy
            converter = (queryAction, method) =>
            {
                // method.Arguments   [0]: expression       [1]: takeCount
                var expression = method.Arguments[0];
                var memberName = GetMemberName(method.Arguments[1]);

                if (!"true".Equals(queryAction.GetValue("_gettedOrder")))
                {
                    var methodName = method.Method.Name;
                    var orderParam = new OrderField(memberName, !methodName.EndsWith("Descending"));

                    if (methodName.StartsWith("Order"))
                    {
                        queryAction.SetValue("_gettedOrder", "true");
                    }

                    if (queryAction.orders == null)
                        queryAction.orders = new List<OrderField> { orderParam };
                    else
                        queryAction.orders.Insert(0, orderParam);
                }

                return ConvertToFilterRule(queryAction, expression);
            };
            RegisterMethodConvertor(typeof(Queryable), "OrderBy", converter);
            RegisterMethodConvertor(typeof(Queryable), "OrderByDescending", converter);
            RegisterMethodConvertor(typeof(Queryable), "ThenBy", converter);
            RegisterMethodConvertor(typeof(Queryable), "ThenByDescending", converter);
            #endregion



            #region #6 Queryable.Where
            converter = (queryAction, method) =>
            {
                // method.Arguments   [0]: expression       [1]: Where
                var expression = method.Arguments[0];
                var where = method.Arguments[1];
                if (expression is ConstantExpression)
                {
                    return ConvertToFilterRule(queryAction, where);
                }

                return new FilterRule { condition = "and", rules = new List<FilterRule> { ConvertToFilterRule(queryAction, expression), ConvertToFilterRule(queryAction, where) } };
            };
            RegisterMethodConvertor(typeof(Queryable), "Where", converter);
            #endregion


            #region #7 Queryable    Count / First / FirstOrDefault / Last / LastOrDefault
            predicate = (arg, methodExp) =>
            {
                var type = methodExp.Method.DeclaringType;
                var name = methodExp.Method.Name;

                if (type != typeof(Queryable)) return false;

                var methods = new[] { "Count", "First", "FirstOrDefault", "Last", "LastOrDefault" };

                return methods.Contains(name);
            };
            converter = (queryAction, method) =>
            {
                // method.Arguments   [0]: expression
                var expression = method.Arguments[0];
                queryAction.method = method.Method.Name;

                return ConvertToFilterRule(queryAction, expression);
            };
            RegisterMethodConvertor(predicate, converter);
            #endregion



            #region #8 IQueryable_Extensions.TotalCount
            converter = (queryAction, method) =>
            {
                // method.Arguments   [0]: expression
                var expression = method.Arguments[0];
                queryAction.method = "TotalCount";

                return ConvertToFilterRule(queryAction, expression);
            };
            RegisterMethodConvertor(typeof(IQueryable_TotalCount_Extensions), "TotalCount", converter);
            #endregion


        }
        #endregion

        protected virtual FilterRule ConvertMethodExpressionToFilterRule(QueryAction queryAction, MethodCallExpression methodExp)
        {
            var convertor = methodConvertors.FirstOrDefault(m => m.predicate(queryAction, methodExp));

            return convertor?.converter(queryAction, methodExp) ?? throw new NotSupportedException($"Unsupported method call: {methodExp.Method.Name}");
        }
    }
}
