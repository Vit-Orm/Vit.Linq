using System.Linq.Expressions;

using Vit.Linq.Filter.ComponentModel;
using Vit.Linq.Filter.MethodCalls;

namespace Vit.Linq.Filter.FilterGenerator
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
