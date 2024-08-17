using System.Linq.Expressions;

using Vit.Linq.FilterRules.ComponentModel;
using Vit.Linq.FilterRules.MethodCalls;

namespace Vit.Linq.FilterRules.FilterGenerator
{

    public class Constant : IFilterGenerator
    {
        public virtual int priority { get; set; } = 100;

        public FilterRule ConvertToData(FilterGenerateArgument arg, Expression expression)
        {
            if (expression is ConstantExpression constant && constant.Value is bool v)
            {
                return new FilterRule { @operator = v ? "true" : "false" };
            }

            return null;
        }


    }
}
