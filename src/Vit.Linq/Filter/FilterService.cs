using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

using Vit.Linq.Filter.ComponentModel;


namespace Vit.Linq.Filter
{
    public partial class FilterService
    {
        public static FilterService Instance = new FilterService();


        public bool operatorIsIgnoreCase = true;


        #region OperatorMap
        /// <summary>
        /// operatorName -> operatorType(in class FilterRuleOperator)
        /// </summary>
        protected Dictionary<string, string> operatorMap = new Dictionary<string, string>();


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
        #endregion


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
            ParameterExpression parameter = LinqHelp.CreateParameter(targetType, "lambdaParam");
            var expression = ConvertToExpression(rule, parameter);
            if (expression == null)
            {
                return null;
            }
            return Expression.Lambda(expression, parameter);
        }



        public virtual string GetCondition(IFilterRule filter)
        {
            return filter?.condition;
        }



        public Func<object, object> getPrimitiveValue { get; set; }
        protected virtual object GetPrimitiveValue(object value)
        {
            if (getPrimitiveValue != null) value = getPrimitiveValue(value);
            return value;
        }



        #region  GetRightValue of FileRule
        /// <summary>
        /// (bool success, object value)getRightValue(IFilterRule rule, Type valueType)
        ///
        ///  <para>   var getRightValue = (IFilterRule rule, Type valueType) =>                                   </para>
        ///  <para>   {                                                                                           </para>
        ///  <para>       var valueInRule = rule.value;                                                           </para>
        ///  <para>       if (valueInRule == null) return (true, null);                                           </para>
        ///  <para>       if (valueType.IsAssignableFrom(valueInRule.GetType())) return (true, valueInRule);      </para>
        ///  <para>       return (true, Json.Deserialize(Json.Serialize(valueInRule), valueType));                </para>
        ///  <para>   };                                                                                          </para>
        /// 
        /// </summary>
        public Func<IFilterRule, Type, (bool success, object value)> getRightValue { get; set; }
        protected virtual object GetRightValue(IFilterRule rule, Type valueType)
        {
            if (getRightValue != null)
            {
                var result = getRightValue(rule, valueType);
                if (result.success) return result.value;
            }

            object value = GetPrimitiveValue(rule.value);
            {
                var result = ConvertValue(value, valueType);
                if (result.success) return result.value;
            }

            throw new InvalidOperationException("value in rule is not valid");
            //return value;
        }




        #region static ConvertValue
        static (bool success, object value) ConvertValue(object value, Type valueType)
        {
            //  List
            if (valueType.IsGenericType && (
                typeof(IEnumerable<>).IsAssignableFrom(valueType.GetGenericTypeDefinition())
                || typeof(List<>).IsAssignableFrom(valueType.GetGenericTypeDefinition())
                ))
            {
                var elementType = valueType.GetGenericArguments()[0];
                var list = value as IEnumerable<object>;
                if (list == null) return (false, null);
                return (true, ToList(list, elementType));
            }
            else
            {
                return (true, ConvertElementValue(value, valueType));
            }
        }

        static object ToList(IEnumerable<object> list, Type elementType)
        {
            var methodInfo = new Func<IEnumerable<object>, List<string>>(ToList<string>)
              .Method.GetGenericMethodDefinition().MakeGenericMethod(elementType);

            return methodInfo.Invoke(null, new object[] { list });
        }

        static List<ElementType> ToList<ElementType>(IEnumerable<object> list)
        {
            return list?.Select(elem => (ElementType)ConvertElementValue(elem, typeof(ElementType))).ToList();
        }

        static object ConvertElementValue(object value, Type valueType)
        {
            if (value == null) return default;

            if (valueType.IsAssignableFrom(value.GetType()))
            {
                return value;
            }
            valueType = Nullable.GetUnderlyingType(valueType) ?? valueType;
            return Convert.ChangeType(value, valueType);
        }
        #endregion

        #endregion



        protected virtual Expression GetLeftValueExpression(IFilterRule rule, ParameterExpression valueExpression)
        {
            return rule.GetLeftValueExpression(valueExpression);
        }


        public virtual string GetOperator(IFilterRule filter)
        {
            var Operator = filter.@operator ?? "";
            if (operatorIsIgnoreCase) Operator = Operator.ToLower();
            if (operatorMap.TryGetValue(Operator, out var op2)) return operatorIsIgnoreCase ? op2?.ToLower() : op2;
            return Operator;
        }

        protected virtual Expression GetRightValueExpression(IFilterRule rule, ParameterExpression valueExpression, Type valueType)
        {
            var rightValue = GetRightValue(rule, valueType);
            return Expression.Constant(rightValue, valueType);

            //Expression<Func<object>> valueLamba = () => rightValue;
            //return Expression.Convert(valueLamba.Body, valueType);
        }






   


        #region ConvertToExpression

        Expression ConvertToExpression(IFilterRule rule, ParameterExpression parameter)
        {
            if (rule == null) return null;

            return ConvertToExpression(rule, parameter, GetCondition(rule));
        }

        Expression ConvertToExpression(IFilterRule rule, ParameterExpression parameter, string condition)
        {
            if (condition == null)
                return ConvertToExpressionNonNested(rule, parameter);

            //  nested filter rules
            if (RuleCondition.And.Equals(condition, StringComparison.OrdinalIgnoreCase))
            {
                if (rule.rules == null)
                    return ConvertToExpressionNonNested(rule, parameter);
                else
                    return ConvertToExpression(rule.rules, parameter, isAnd: true);
            }
            else if (RuleCondition.Or.Equals(condition, StringComparison.OrdinalIgnoreCase))
            {
                if (rule.rules == null)
                    return ConvertToExpressionNonNested(rule, parameter);
                else
                    return ConvertToExpression(rule.rules, parameter, isAnd: false);
            }
            else if (RuleCondition.Not.Equals(condition, StringComparison.OrdinalIgnoreCase))
            {
                return Expression.Not(ConvertToExpression(rule, parameter, null));
            }
            else if (RuleCondition.NotAnd.Equals(condition, StringComparison.OrdinalIgnoreCase))
            {
                return Expression.Not(ConvertToExpression(rule, parameter, RuleCondition.And));
            }
            else if (RuleCondition.NotOr.Equals(condition, StringComparison.OrdinalIgnoreCase))
            {
                return Expression.Not(ConvertToExpression(rule, parameter, RuleCondition.Or));
            }

            throw new Exception("unrecognized condition : " + rule.condition);
        }



        Expression ConvertToExpressionNonNested(IFilterRule rule, ParameterExpression parameter)
        {
            // non-nested rule

            Expression leftValueExpression = GetLeftValueExpression(rule, parameter);
            Type leftValueType = leftValueExpression.Type;

            var Operator = GetOperator(rule);
            if (string.IsNullOrWhiteSpace(Operator))
                return null;

            #region CustomOperator
            var operatorBuilderArgs = new OperatorBuilderArgs
            {
                rule = rule,
                parameter = parameter,
                leftValue = leftValueExpression,
                Operator = Operator,
                GetRightValueExpression = (Type rightValueType) => this.GetRightValueExpression(rule, parameter, rightValueType)
            };
            var operatorExpression = CustomOperator_ToExpression(operatorBuilderArgs);
            if (operatorExpression != default) return operatorExpression;
            #endregion


            #region Get System Expression

            var cmpType = operatorIsIgnoreCase ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal;


            {
                #region ##1  null
                if (RuleOperator.IsNull.Equals(Operator, cmpType))
                {
                    return IsNull();
                }
                if (RuleOperator.IsNotNull.Equals(Operator, cmpType))
                {
                    return Expression.Not(IsNull());
                }
                #endregion


                #region ##2 number
                if (RuleOperator.Equal.Equals(Operator, cmpType))
                {
                    return Expression.Equal(leftValueExpression, GetRightValueExpression());
                }
                if (RuleOperator.NotEqual.Equals(Operator, cmpType))
                {
                    return Expression.NotEqual(leftValueExpression, GetRightValueExpression());
                }

                if (RuleOperator.GreaterThan.Equals(Operator, cmpType))
                {
                    return Expression.GreaterThan(leftValueExpression, GetRightValueExpression());
                }
                if (RuleOperator.GreaterThanOrEqual.Equals(Operator, cmpType))
                {
                    return Expression.GreaterThanOrEqual(leftValueExpression, GetRightValueExpression());
                }
                if (RuleOperator.LessThan.Equals(Operator, cmpType))
                {
                    return Expression.LessThan(leftValueExpression, GetRightValueExpression());

                }
                if (RuleOperator.LessThanOrEqual.Equals(Operator, cmpType))
                {
                    return Expression.LessThanOrEqual(leftValueExpression, GetRightValueExpression());
                }
                #endregion


                #region ##3 array
                if (RuleOperator.In.Equals(Operator, cmpType))
                {
                    return In();
                }
                if (RuleOperator.NotIn.Equals(Operator, cmpType))
                {
                    return Expression.Not(In());
                }
                #endregion


                #region ##4 string
                if (RuleOperator.Contains.Equals(Operator, cmpType))
                {
                    var nullCheck = Expression.Call(typeof(string), "IsNullOrEmpty", null, leftValueExpression);
                    var contains = Expression.Call(leftValueExpression, "Contains", null, GetRightValueExpression());

                    return Expression.AndAlso(Expression.Not(nullCheck), contains);

                }
                if (RuleOperator.NotContains.Equals(Operator, cmpType))
                {
                    var nullCheck = Expression.Call(typeof(string), "IsNullOrEmpty", null, leftValueExpression);
                    var contains = Expression.Call(leftValueExpression, "Contains", null, GetRightValueExpression());

                    return Expression.OrElse(nullCheck, Expression.Not(contains));
                }
                if (RuleOperator.StartsWith.Equals(Operator, cmpType))
                {
                    var nullCheck = Expression.Not(Expression.Call(typeof(string), "IsNullOrEmpty", null, leftValueExpression));
                    var startsWith = Expression.Call(leftValueExpression, "StartsWith", null, GetRightValueExpression());

                    return Expression.AndAlso(nullCheck, startsWith);
                }

                if (RuleOperator.EndsWith.Equals(Operator, cmpType))
                {
                    var nullCheck = Expression.Not(Expression.Call(typeof(string), "IsNullOrEmpty", null, leftValueExpression));
                    var endsWith = Expression.Call(leftValueExpression, "EndsWith", null, GetRightValueExpression());
                    return Expression.AndAlso(nullCheck, endsWith);
                }
                if (RuleOperator.IsNullOrEmpty.Equals(Operator, cmpType))
                {
                    return Expression.Call(typeof(string), "IsNullOrEmpty", null, leftValueExpression);
                }
                if (RuleOperator.IsNotNullOrEmpty.Equals(Operator, cmpType))
                {
                    return Expression.Not(Expression.Call(typeof(string), "IsNullOrEmpty", null, leftValueExpression));
                }
                #endregion

            }
            #endregion



            throw new Exception("unrecognized operator : " + rule.@operator);
            //return null;


            #region inner Method
            Expression GetRightValueExpression()
            {
                return this.GetRightValueExpression(rule, parameter, leftValueType);
            }

            Expression IsNull()
            {
                var isNullable = !leftValueType.IsValueType || Nullable.GetUnderlyingType(leftValueType) != null;

                if (isNullable)
                {
                    var nullValue = Expression.Constant(null, leftValueType);
                    return Expression.Equal(leftValueExpression, nullValue);
                }
                return Expression.Constant(false, typeof(bool));
            }

            Expression In()
            {
                // #1 using Enumerable<>.Contains
                //Type valueType = typeof(IEnumerable<>).MakeGenericType(leftValueType);
                //var rightValueExpression = this.GetRightValueExpression(rule, parameter, valueType);

                //return Expression.Call(typeof(System.Linq.Enumerable), "Contains", new[] { leftValueType }, rightValueExpression, leftValueExpression);

                //-------------------------------------
                // #2 using List<>.Contains
                Type valueType = typeof(List<>).MakeGenericType(leftValueType);
                var rightValueExpression = this.GetRightValueExpression(rule, parameter, valueType);

                return Expression.Call(rightValueExpression, "Contains", null, leftValueExpression);
            }
            #endregion
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="rules"></param>
        /// <param name="parameter"></param>
        /// <param name="isAnd"> true : and     false: or </param>
        /// <returns></returns>
        Expression ConvertToExpression(IEnumerable<IFilterRule> rules, ParameterExpression parameter, bool isAnd = true)
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
                return isAnd ? Expression.AndAlso(exp1, exp2) : Expression.OrElse(exp1, exp2);
            }
            #endregion

        }

        #endregion
    }
}
