using System.Linq.Expressions;

using Vit.Linq.Filter.ComponentModel;
using Vit.Linq.Filter.MethodCalls;

namespace Vit.Linq.Filter.FilterGenerator
{
    public class Lambda : IFilterGenerator
    {
        public virtual int priority { get; set; } = 400;
        public FilterRule ConvertToData(FilterGenerateArgument arg, Expression expression)
        {
            if (expression is not LambdaExpression lambda) return null;

            return arg.convertService.ConvertToData(arg, lambda.Body);
        }

    }
}
