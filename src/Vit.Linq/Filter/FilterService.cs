using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

using Vit.Linq.ComponentModel;


namespace Vit.Linq.Filter
{
    public class OperatorBuilderArgs
    {
        public IFilterRule rule { get; set; }
        public ParameterExpression parameter { get; set; }
        public Expression leftValue { get; set; }
        public string Operator { get; set; }

        /// <summary>
        /// Type rightValueType
        /// </summary>
        public Func<Type, Expression> GetRightValueExpression { get; set; }
    }



    public class FilterService
    {
        public static FilterService Instance = new FilterService();

        /// <summary>
        /// operatorName -> operatorType(in class FilterRuleOperator)
        /// </summary>
        protected Dictionary<string, string> operatorMap = new Dictionary<string, string>();
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



        public virtual ECondition? GetCondition(IFilterRule filter)
        {
            var condition = filter.condition;
            if (string.IsNullOrEmpty(condition)) return default;

            if (FilterRuleCondition.And.Equals(condition, StringComparison.OrdinalIgnoreCase))
            {
                return ECondition.And;
            }
            if (FilterRuleCondition.Or.Equals(condition, StringComparison.OrdinalIgnoreCase))
            {
                return ECondition.Or;
            }
            if (FilterRuleCondition.Not.Equals(condition, StringComparison.OrdinalIgnoreCase))
            {
                return ECondition.Not;
            }
            if (FilterRuleCondition.NotAnd.Equals(condition, StringComparison.OrdinalIgnoreCase))
            {
                return ECondition.NotAnd;
            }
            if (FilterRuleCondition.NotOr.Equals(condition, StringComparison.OrdinalIgnoreCase))
            {
                return ECondition.NotOr;
            }
            return default;
        }


        #region GetPrimitiveValue
        /// <summary>
        /// (bool success, object value)GetPrimitiveValue(object valueInRule, IFilterRule rule, Type valueType)
        ///
        ///  <para>   var GetPrimitiveValue = (object valueInRule, IFilterRule rule, Type valueType) =>           </para>
        ///  <para>   {                                                                                           </para>
        ///  <para>       if (valueInRule == null) return (true, null);                                           </para>
        ///  <para>       if (valueType.IsAssignableFrom(valueInRule.GetType())) return (true, valueInRule);      </para>
        ///  <para>       return (true, Json.Deserialize(Json.Serialize(valueInRule), valueType));                </para>
        ///  <para>   };                                                                                          </para>        /// 
        /// 
        /// </summary>
        public Func<object, IFilterRule, Type, (bool success, object value)> GetPrimitiveValue { get; set; }
        protected virtual object GetRulePrimitiveValue(object valueInRule, IFilterRule rule, Type valueType)
        {
            if (GetPrimitiveValue != null)
            {
                var result = GetPrimitiveValue(valueInRule, rule, valueType);
                if (result.success) return result.value;
            }

            //if (valueInRule == null) return null;
            //if (valueType.IsAssignableFrom(valueInRule.GetType())) return valueInRule;
            //return  Vit.Core.Module.Serialization.Json.Deserialize(Vit.Core.Module.Serialization.Json.Serialize(valueInRule), valueType);

            object value = null;

            //  List
            if (valueType.IsGenericType && (
                typeof(IEnumerable<>).IsAssignableFrom(valueType.GetGenericTypeDefinition())
                || typeof(List<>).IsAssignableFrom(valueType.GetGenericTypeDefinition())
                ))
            {
                //if (valueInRule != null)
                {
                    value = ConvertToList(valueInRule, rule, valueType);
                }
            }
            else
            {
                //  value
                if (valueInRule != null)
                {
                    Type type = Nullable.GetUnderlyingType(valueType) ?? valueType;
                    value = Convert.ChangeType(valueInRule, type);
                }
            }
            return value;
        }


        #region ConvertToList
        internal object ConvertToList(object value, IFilterRule rule, Type valueType)
        {
            if (!(value is string) && value is IEnumerable values)
            {
                Type fieldType = valueType.GetGenericArguments()[0];

                var methodInfo = new Func<IEnumerable, IFilterRule, List<object>>(ConvertToListByType<object>)
                    .Method.GetGenericMethodDefinition().MakeGenericMethod(fieldType);
                return methodInfo.Invoke(this, new object[] { values, rule });
            }
            return null;
        }
        internal List<T> ConvertToListByType<T>(IEnumerable values, IFilterRule rule)
        {
            Type fieldType = typeof(T);
            var list = new List<T>();
            foreach (var itemValue in values)
            {
                T value = (T)Convert.ChangeType(itemValue, fieldType);
                list.Add(value);
            }
            return list;
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
            var rightValue = GetRulePrimitiveValue(rule.value, rule, valueType);
            return Expression.Constant(rightValue, valueType);

            //Expression<Func<object>> valueLamba = () => rightValue;
            //return Expression.Convert(valueLamba.Body, valueType);
        }




        #region Custom Operator

        Dictionary<string, Func<OperatorBuilderArgs, Expression>> customOperator = new Dictionary<string, Func<OperatorBuilderArgs, Expression>>();

        Expression CustomOperator_GetExpression(OperatorBuilderArgs args)
        {
            if (customOperator.TryGetValue(args.Operator, out var operatorBuilder) && operatorBuilder != null)
            {
                return operatorBuilder(args);
            }
            return default;
        }

        public virtual void CustomOperator_Add(string Operator, Func<OperatorBuilderArgs, Expression> operatorBuilder)
        {
            if (operatorIsIgnoreCase) Operator = Operator.ToLower();
            customOperator[Operator] = operatorBuilder;
        }

        #endregion


        #region ConvertToExpression

        Expression ConvertToExpression(IFilterRule rule, ParameterExpression parameter)
        {
            if (rule == null) return null;

            return ConvertToExpression(rule, parameter, GetCondition(rule));
        }
        Expression ConvertToExpression(IFilterRule rule, ParameterExpression parameter, ECondition? condition)
        {
            if (condition == null)
                return ConvertToExpressionNonNested(rule, parameter);

            //  nested filter rules
            switch (condition.Value)
            {
                case ECondition.And:
                case ECondition.Or:
                    if (rule.rules?.Any() == true)
                        return ConvertToExpression(rule.rules, parameter, condition.Value);
                    else
                        return ConvertToExpressionNonNested(rule, parameter);
                case ECondition.Not:
                    return Expression.Not(ConvertToExpression(rule, parameter, null));
                case ECondition.NotAnd:
                    return Expression.Not(ConvertToExpression(rule, parameter, ECondition.And));
                case ECondition.NotOr:
                    return Expression.Not(ConvertToExpression(rule, parameter, ECondition.Or));
                default: throw new Exception("unrecognized condition : " + rule.condition);
            }
        }



        Expression ConvertToExpressionNonNested(IFilterRule rule, ParameterExpression parameter)
        {
            // non-nested rule

            Expression leftValueExpression = GetLeftValueExpression(rule, parameter);
            Type leftValueType = leftValueExpression.Type;

            var Operator = GetOperator(rule);

            #region CustomOperator
            var operatorBuilderArgs = new OperatorBuilderArgs
            {
                rule = rule,
                parameter = parameter,
                leftValue = leftValueExpression,
                Operator = Operator,
                GetRightValueExpression = (Type rightValueType) => this.GetRightValueExpression(rule, parameter, rightValueType)
            };
            var operatorExpression = CustomOperator_GetExpression(operatorBuilderArgs);
            if (operatorExpression != default) return operatorExpression;
            #endregion


            #region Get System Expression

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
        /// <param name="condition"> only could be And or Or</param>
        /// <returns></returns>
        Expression ConvertToExpression(IEnumerable<IFilterRule> rules, ParameterExpression parameter, ECondition condition = ECondition.And)
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
                return condition == ECondition.And ? Expression.AndAlso(exp1, exp2) : Expression.OrElse(exp1, exp2);
            }
            #endregion

        }

        #endregion
    }
}
