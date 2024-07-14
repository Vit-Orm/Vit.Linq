using System;
using System.Linq.Expressions;
using Vit.Linq.Filter.ComponentModel;

namespace Vit.Linq.Filter.FilterConvertor.ConditionConvertor
{
    public class CustomConditionConvertor : IConditionConvertor
    {
        public CustomConditionConvertor(int priority, Func<FilterConvertArgument, IFilterRule, string, (bool success, Expression expression)> convertor)
        {
            this.priority = priority;
            this.convertor = convertor;
        }

        public CustomConditionConvertor(int priority, string condition, Func<FilterConvertArgument, IFilterRule, string, (bool success, Expression expression)> convertor)
        {
            this.priority = priority;
            this.condition = condition;
            this.convertor = convertor;
        }

        public int priority { get; set; }
        protected string condition { get; set; }
        protected Func<FilterConvertArgument, IFilterRule, string, (bool success, Expression expression)> convertor;

        public (bool success, Expression expression) ConvertToCode(FilterConvertArgument arg, IFilterRule filter, string condition)
        {
            if (this.condition?.Equals(filter.condition, arg.comparison) != false) return convertor(arg, filter, condition);

            return default;
        }



    }
}
