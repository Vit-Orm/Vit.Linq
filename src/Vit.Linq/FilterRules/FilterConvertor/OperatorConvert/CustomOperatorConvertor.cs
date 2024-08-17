using System;
using System.Linq.Expressions;

namespace Vit.Linq.FilterRules.FilterConvertor.OperatorConvert
{
    public class CustomOperatorConvertor : IOperatorConvertor
    {
        public CustomOperatorConvertor(int priority, Func<OperatorConvertArgument, (bool success, Expression expression)> convertor)
        {
            this.priority = priority;
            this.convertor = convertor;
        }

        public CustomOperatorConvertor(int priority, string Operator, Func<OperatorConvertArgument, (bool success, Expression expression)> convertor)
        {
            this.priority = priority;
            this.Operator = Operator;
            this.convertor = convertor;
        }

        public int priority { get; set; }
        protected string Operator { get; set; }
        protected Func<OperatorConvertArgument, (bool success, Expression expression)> convertor;

        public (bool success, Expression expression) ConvertToCode(OperatorConvertArgument arg)
        {
            if (Operator?.Equals(arg.Operator, arg.comparison) != false) return convertor(arg);

            return default;
        }
    }
}
