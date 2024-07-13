using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

using Vit.Linq.Filter.ComponentModel;
using Vit.Linq.Filter.FilterConvertor;
using Vit.Linq.Filter.FilterConvertor.ConditionConvertor;
using Vit.Linq.Filter.FilterConvertor.OperatorConvert;


namespace Vit.Linq.Filter
{
    public partial class FilterService
    {
        public static FilterService Instance = new FilterService();


        public bool operatorIsIgnoreCase = true;

        public virtual Expression ConvertToCode(ParameterExpression parameter, IFilterRule rule)
        {
            var result = ConvertToCode(new FilterConvertArgument(filterService: this, parameter: parameter), rule);
            if (result.success) return result.expression;

            throw new ArgumentException("Unrecognized filter");
        }


        public virtual (bool success, Expression expression) ConvertToCode(FilterConvertArgument arg, IFilterRule filter)
        {
            var condition = GetCondition(filter);
            foreach (var convertor in conditionConvertors)
            {
                var result = convertor.ConvertToCode(arg, filter, condition);
                if (result.success) return result;
            }
            return default;
        }



        public virtual Func<T, bool> ConvertToCode_Predicate<T>(IFilterRule rule)
        {
            return ConvertToCode_PredicateExpression<T>(rule)?.Compile();
        }

        public virtual string ConvertToCode_PredicateExpressionString<T>(IFilterRule rule)
        {
            return ConvertToCode_PredicateExpression<T>(rule)?.ToString();
        }


        public virtual Expression<Func<T, bool>> ConvertToCode_PredicateExpression<T>(IFilterRule rule)
        {
            var exp = ConvertToCode_LambdaExpression(rule, typeof(T));
            return (Expression<Func<T, bool>>)exp;
        }


        public virtual LambdaExpression ConvertToCode_LambdaExpression(IFilterRule rule, Type targetType)
        {
            ParameterExpression parameter = LinqHelp.CreateParameter(targetType, "lambdaParam");
            var expression = ConvertToCode(parameter, rule);
            if (expression == null)
            {
                return null;
            }
            return Expression.Lambda(expression, parameter);
        }





        #region Convertors

        public static List<Type> defaultConvertorTypes;

        public FilterService()
        {
            // populate
            defaultConvertorTypes ??=
                typeof(FilterService).Assembly.GetTypes()
                .Where(type => type.IsClass
                    && !type.IsAbstract
                    && typeof(IConditionConvertor).IsAssignableFrom(type)
                    && type.GetConstructor(Type.EmptyTypes) != null
                ).ToList();

            defaultConvertorTypes.ForEach(type => RegisterConditionConvertor(Activator.CreateInstance(type) as IConditionConvertor));
        }

        protected List<IConditionConvertor> conditionConvertors = new();
        public virtual void RegisterConditionConvertor(IConditionConvertor convertor)
        {
            conditionConvertors.Add(convertor);
            conditionConvertors.Sort((a, b) => a.priority - b.priority);
        }

        public virtual void RegisterConditionConvertor(string condition, Func<FilterConvertArgument, IFilterRule, string, (bool success, Expression expression)> convertor, int priority = 10)
        {
            RegisterConditionConvertor(new CustomConditionConvertor(priority: priority, condition: condition, convertor: convertor));
        }
        public virtual void RegisterConditionConvertor(string condition, Func<FilterConvertArgument, IFilterRule, string, Expression> convertor, int priority = 10)
        {
            Func<FilterConvertArgument, IFilterRule, string, (bool success, Expression expression)> convertorWithFlag = (arg, filter, condition) => (true, convertor(arg, filter, condition));
            RegisterConditionConvertor(new CustomConditionConvertor(priority: priority, condition: condition, convertor: convertorWithFlag));
        }

        public virtual bool RegisterOperatorConvertor(IOperatorConvertor convertor)
        {
            var operatorConvertor = conditionConvertors.FirstOrDefault(m => m is OperatorConvertor) as OperatorConvertor;
            if (operatorConvertor == null) return false;
            operatorConvertor.RegisterOperatorConvertor(convertor);
            return true;
        }

        public virtual bool RegisterOperatorConvertor(string Operator, Func<OperatorConvertArgument, (bool success, Expression expression)> convertor, int priority = 100)
        {
            return RegisterOperatorConvertor(new CustomOperatorConvertor(priority: priority, Operator: Operator, convertor: convertor));
        }

        public virtual bool RegisterOperatorConvertor(string Operator, Func<OperatorConvertArgument, Expression> convertor, int priority = 100)
        {
            Func<OperatorConvertArgument, (bool success, Expression expression)> convertorWithFlag = (arg) => (true, convertor(arg));
            return RegisterOperatorConvertor(new CustomOperatorConvertor(priority: priority, Operator: Operator, convertor: convertorWithFlag));
        }

        #endregion




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





        public virtual Expression GetLeftValueExpression(IFilterRule rule, ParameterExpression valueExpression)
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

        #region OperatorMap
        /// <summary>
        /// operatorName -> operatorType(in class FilterRuleOperator)
        /// </summary>
        protected Dictionary<string, string> operatorMap = new Dictionary<string, string>();


        public virtual FilterService AddOperatorMap(string operatorName, string operatorType)
        {
            if (operatorIsIgnoreCase) operatorName = operatorName?.ToLower();
            operatorMap[operatorName] = operatorType;
            return this;
        }

        public virtual FilterService AddOperatorMaps(IEnumerable<(string operatorName, string operatorType)> maps)
        {
            foreach (var (operatorName, operatorType) in maps)
                AddOperatorMap(operatorName, operatorType);
            return this;
        }
        #endregion

        public virtual Expression GetRightValueExpression(IFilterRule rule, ParameterExpression parameter, Type valueType)
        {
            var rightValue = GetRightValue(rule, valueType);
            return Expression.Constant(rightValue, valueType);

            //Expression<Func<object>> valueLamba = () => rightValue;
            //return Expression.Convert(valueLamba.Body, valueType);
        }



        #region  GetRightValue
        /// <summary>
        /// (bool success, object value)getRightValue(IFilterRule filter, Type valueType)
        ///
        ///  <para>   var getRightValue = (IFilterRule filter, Type valueType) =>                                 </para>
        ///  <para>   {                                                                                           </para>
        ///  <para>       var valueInRule = filter.value;                                                         </para>
        ///  <para>       if (valueInRule == null) return (true, null);                                           </para>
        ///  <para>       if (valueType.IsAssignableFrom(valueInRule.GetType())) return (true, valueInRule);      </para>
        ///  <para>       return (true, Json.Deserialize(Json.Serialize(valueInRule), valueType));                </para>
        ///  <para>   };                                                                                          </para>
        /// 
        /// </summary>
        public Func<IFilterRule, Type, (bool success, object value)> getRightValue { get; set; }
        protected virtual object GetRightValue(IFilterRule filter, Type valueType)
        {
            if (getRightValue != null)
            {
                var result = getRightValue(filter, valueType);
                if (result.success) return result.value;
            }

            object value = GetPrimitiveValue(filter.value);
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
                if (value is not IEnumerable<object> list) return (false, null);
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







    }
}
