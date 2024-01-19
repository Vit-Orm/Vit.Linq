using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using System.Linq;
using Vit.Linq.QueryBuilder;

namespace Vit.Linq.MoreFilter
{
    internal interface IMethods { }

    [ExcludeFromCodeCoverage]
    public class FilterRuleWithMethod : FilterRuleWithMethod<FilterRuleWithMethod>
    {
        public static bool SupportFieldMethod(IFilterRule filter)
        {
            return typeof(IMethods).IsAssignableFrom(filter.GetType());
        }
    }



    /// <summary>
    /// This class is used to define a hierarchical filter for a given collection. This type can be serialized/deserialized by JSON.NET without needing to modify the data structure from QueryBuilder.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class FilterRuleWithMethod<RuleType> : FilterRuleBase<RuleType>, IMethods
               where RuleType : IFilterRule
    {
        public class Field
        {
            public string field { get; set; }
            public string method { get; set; }

            public object[] methodParameters { get; set; }
        }

        public virtual List<Field> fields { get; set; }


        public virtual Expression GetValueExpression(Expression valueExpression, Field field)
        {
            if (!string.IsNullOrWhiteSpace(field.field))
            {
                valueExpression = LinqHelp.GetFieldMemberExpression(valueExpression, field.field);
            }

            if (!string.IsNullOrWhiteSpace(field.method))
            {
                var paramCount = field.methodParameters?.Length ?? 0;
                var valueType = valueExpression.Type;
                var Method = valueType.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance)
                    ?.FirstOrDefault(m => m.Name == field.method && m.GetParameters().Length == paramCount);
                if (Method != null)
                {
                    if (paramCount == 0)
                    {
                        valueExpression = Expression.Call(valueExpression, Method);
                    }
                    else
                    {
                        var argExpressions = Method.GetParameters().Select((paras, index) =>
                        {
                            var value = GetPrimitiveValue(field.methodParameters[index]);
                            value = Convert.ChangeType(value, paras.ParameterType);
                            return Expression.Constant(value);
                        }).ToList();

                        valueExpression = Expression.Call(valueExpression, Method, argExpressions);
                    }
                }
            }
            return valueExpression;
        }

        public override Expression GetLeftValueExpression(Expression valueExpression)
        {
            valueExpression = base.GetLeftValueExpression(valueExpression);

            fields?.ForEach(field => { valueExpression = GetValueExpression(valueExpression, field); });

            return valueExpression;
        }


        protected virtual object GetPrimitiveValue(Object value) => value;


        /// <summary>
        /// Gets or sets the value of the filter.
        /// </summary>
        /// <value>
        /// The value.
        /// </value>
        public override object value
        {
            get => GetPrimitiveValue(_value);
            set => _value = value;
        }

        private object _value;




    }
}
