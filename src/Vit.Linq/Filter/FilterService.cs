using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;


namespace Vit.Linq.Filter
{
    public class FilterService
    {
        public static FilterService Instance = new FilterService();

        /// <summary>
        /// operatorName -> operatorType(in class FilterRuleOperator)
        /// </summary>
        public Dictionary<string, string> operatorMap = new Dictionary<string, string>();
        public bool operatorIsIgnoreCase = true;
        public bool ignoreError = false;

        public FilterService AddOperatorMap(string operatorName, string operatorType)
        {
            if (operatorIsIgnoreCase) operatorName = operatorName?.ToLower();
            operatorMap[operatorName] = operatorType;
            return this;
        }

        public FilterService AddOperatorMap(IEnumerable<(string operatorName, string operatorType)> maps)
        {
            foreach (var map in maps)
                AddOperatorMap(map.operatorName, map.operatorType);
            return this;
        }



        public Func<T, bool> ToPredicate<T>(IFilterRule rule)
        {
            return ToExpression<T>(rule)?.Compile();
        }

        public string ToExpressionString<T>(IFilterRule rule)
        {
            return ToExpression<T>(rule)?.ToString();
        }


        public Expression<Func<T, bool>> ToExpression<T>(IFilterRule rule)
        {
            var exp = ToLambdaExpression(rule, typeof(T));
            return (Expression<Func<T, bool>>)exp;
        }


        public LambdaExpression ToLambdaExpression(IFilterRule rule, Type targetType)
        {
            ParameterExpression parameter = Expression.Parameter(targetType);
            var expression = ConvertToExpression(rule, parameter);
            if (expression == null)
            {
                return null;
            }
            return Expression.Lambda(expression, parameter);
        }


        public virtual ECondition GetCondition(IFilterRule filter)
        {
            return filter.condition?.ToLower() == "or" ? ECondition.or : ECondition.and;
        }



        protected virtual Expression GetLeftValueExpression(IFilterRule rule, ParameterExpression valueExpression)
        {
            return rule.GetLeftValueExpression(valueExpression); 
        }


        public virtual string GetOperator(IFilterRule filter)
        {
            var operate = filter.@operator ?? "";
            if (operatorIsIgnoreCase) operate = operate.ToLower();
            if (operatorMap.TryGetValue(operate, out var op2)) return operatorIsIgnoreCase ? op2?.ToLower() : op2;
            return operate;
        }


        #region GetRightValueExpression

        public Func<object, IFilterRule, Type, object> GetRuleValue { get; set; }
        protected virtual object GetRulePrimitiveValue(object value, IFilterRule rule, Type fieldType)
        {
            if (GetRuleValue != null) return GetRuleValue(value, rule, fieldType);
            return value;
        }




        protected virtual Expression GetRightValueExpression(IFilterRule rule, ParameterExpression parameter, Type valueType)
        {
            object rightValue = rule.value;

            // typeof(IEnumerable).IsAssignableFrom(valueType)
            if (valueType.IsGenericType && typeof(IEnumerable<>).IsAssignableFrom(valueType.GetGenericTypeDefinition()))
            {
                // constant List
                object value = null;
                if (rule.value != null)
                {
                    //value = Vit.Core.Module.Serialization.Json.Deserialize(Vit.Core.Module.Serialization.Json.Serialize(rule.value), valueType);
                    var leftFieldType = valueType.GetGenericArguments()[0];
                    value = ConvertToList(rule.value, rule, leftFieldType);
                }
                rightValue = value;
            }
            else
            {
                // constant value
                object value = GetRulePrimitiveValue(rule.value, rule, valueType);
                if (value != null)
                {
                    Type type = Nullable.GetUnderlyingType(valueType) ?? valueType;
                    value = Convert.ChangeType(value, type);
                }
                rightValue = value;
            }

            Expression<Func<object>> valueLamba = () => rightValue;
            return Expression.Convert(valueLamba.Body, valueType);
        }

        #region ConvertToList
        internal object ConvertToList(object value, IFilterRule rule, Type fieldType)
        {
            if (value is string str)
            {
                var itemValue = GetRulePrimitiveValue(str, rule, fieldType);
                if (itemValue is IEnumerable)
                    return itemValue;
            }
            else if (value is IEnumerable values)
            {
                var methodInfo = typeof(FilterService).GetMethod("ConvertToListByType", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic).MakeGenericMethod(fieldType);
                return methodInfo.Invoke(this, new object[] { values, rule });
            }
            return null;
        }
        internal List<T> ConvertToListByType<T>(IEnumerable values, IFilterRule rule)
        {
            Type fieldType = typeof(T);
            var list = new List<T>();
            foreach (var item in values)
            {
                var itemValue = GetRulePrimitiveValue(item, rule, fieldType);
                T value = (T)Convert.ChangeType(itemValue, fieldType);
                list.Add(value);
            }
            return list;
        }
        #endregion

        #endregion


        Expression ConvertToExpression(IFilterRule rule, ParameterExpression parameter)
        {
            if (rule == null) return null;

            // #1 nested filter rules
            if (rule.rules?.Any() == true)
            {
                return ConvertToExpression(rule.rules, parameter, GetCondition(rule));
            }

            // #2 simple rule
            if (string.IsNullOrWhiteSpace(rule.@operator))
            {
                return null;
            }

            Expression leftValueExpression = GetLeftValueExpression(rule, parameter);
            Type leftFieldType = leftValueExpression.Type;

            #region Get Expression

            var Operator = GetOperator(rule);
            var cmpType = operatorIsIgnoreCase ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal;


            {
                #region ##1  null
                if (FilterRuleOperator.IsNull.Equals(Operator, cmpType))
                {
                    return IsNull();
                }
                if (FilterRuleOperator.IsNotNull.Equals(Operator, cmpType))
                {
                    return Expression.Not(IsNull());
                }
                #endregion


                #region ##2 number
                if (FilterRuleOperator.Equal.Equals(Operator, cmpType))
                {
                    return Expression.Equal(leftValueExpression, GetRightValueExpression());
                }
                if (FilterRuleOperator.NotEqual.Equals(Operator, cmpType))
                {
                    return Expression.NotEqual(leftValueExpression, GetRightValueExpression());
                }

                if (FilterRuleOperator.GreaterThan.Equals(Operator, cmpType))
                {
                    return Expression.GreaterThan(leftValueExpression, GetRightValueExpression());
                }
                if (FilterRuleOperator.GreaterThanOrEqual.Equals(Operator, cmpType))
                {
                    return Expression.GreaterThanOrEqual(leftValueExpression, GetRightValueExpression());
                }
                if (FilterRuleOperator.LessThan.Equals(Operator, cmpType))
                {
                    return Expression.LessThan(leftValueExpression, GetRightValueExpression());

                }
                if (FilterRuleOperator.LessThanOrEqual.Equals(Operator, cmpType))
                {
                    return Expression.LessThanOrEqual(leftValueExpression, GetRightValueExpression());
                }
                #endregion


                #region ##3 array
                if (FilterRuleOperator.In.Equals(Operator, cmpType))
                {
                    return In();
                }
                if (FilterRuleOperator.NotIn.Equals(Operator, cmpType))
                {
                    return Expression.Not(In());
                }
                #endregion


                #region ##4 string
                if (FilterRuleOperator.Contains.Equals(Operator, cmpType))
                {
                    var nullCheck = Expression.Call(typeof(string), "IsNullOrEmpty", null, leftValueExpression);
                    var contains = Expression.Call(leftValueExpression, "Contains", null, GetRightValueExpression());

                    return Expression.AndAlso(Expression.Not(nullCheck), contains);

                }
                if (FilterRuleOperator.NotContains.Equals(Operator, cmpType))
                {
                    var nullCheck = Expression.Call(typeof(string), "IsNullOrEmpty", null, leftValueExpression);
                    var contains = Expression.Call(leftValueExpression, "Contains", null, GetRightValueExpression());

                    return Expression.OrElse(nullCheck, Expression.Not(contains));
                }
                if (FilterRuleOperator.StartsWith.Equals(Operator, cmpType))
                {
                    var nullCheck = Expression.Not(Expression.Call(typeof(string), "IsNullOrEmpty", null, leftValueExpression));
                    var startsWith = Expression.Call(leftValueExpression, "StartsWith", null, GetRightValueExpression());

                    return Expression.AndAlso(nullCheck, startsWith);
                }

                if (FilterRuleOperator.EndsWith.Equals(Operator, cmpType))
                {
                    var nullCheck = Expression.Not(Expression.Call(typeof(string), "IsNullOrEmpty", null, leftValueExpression));
                    var endsWith = Expression.Call(leftValueExpression, "EndsWith", null, GetRightValueExpression());
                    return Expression.AndAlso(nullCheck, endsWith);
                }
                if (FilterRuleOperator.IsNullOrEmpty.Equals(Operator, cmpType))
                {
                    return Expression.Call(typeof(string), "IsNullOrEmpty", null, leftValueExpression);
                }
                if (FilterRuleOperator.IsNotNullOrEmpty.Equals(Operator, cmpType))
                {
                    return Expression.Not(Expression.Call(typeof(string), "IsNullOrEmpty", null, leftValueExpression));
                }
                #endregion

            }
            #endregion

            if (!ignoreError) throw new Exception("unrecognized operator : " + rule.@operator);
            return null;


            #region inner Method
            Expression GetRightValueExpression()
            {
                return this.GetRightValueExpression(rule, parameter, leftFieldType);
            }

            Expression IsNull()
            {
                var isNullable = !leftFieldType.IsValueType || Nullable.GetUnderlyingType(leftFieldType) != null;

                if (isNullable)
                {
                    var nullValue = Expression.Constant(null, leftFieldType);
                    return Expression.Equal(leftValueExpression, nullValue);
                }
                return Expression.Constant(false, typeof(bool));
            }

            Expression In()
            {
                Expression arrayExp = null;
                Type valueType = typeof(IEnumerable<>).MakeGenericType(leftFieldType);
                arrayExp = this.GetRightValueExpression(rule, parameter, valueType);

                var inCheck = Expression.Call(typeof(System.Linq.Enumerable), "Contains", new[] { leftFieldType }, arrayExp, leftValueExpression);
                return inCheck;
            }
            #endregion
        }


        Expression ConvertToExpression(IEnumerable<IFilterRule> rules, ParameterExpression parameter, ECondition condition = ECondition.and)
        {
            if (rules?.Any() != true)
            {
                return null;
            }

            Expression expression = null;

            foreach (var rule in rules)
            {
                var curExp = ConvertToExpression(rule, parameter);
                if (curExp != null)
                    expression = Append(expression, curExp);
            }

            return expression;


            #region Method Append
            Expression Append(Expression exp1, Expression exp2)
            {
                if (exp1 == null)
                {
                    return exp2;
                }

                if (exp2 == null)
                {
                    return exp1;
                }
                return condition == ECondition.or ? Expression.OrElse(exp1, exp2) : Expression.AndAlso(exp1, exp2);
            }
            #endregion

        }


    }
}
