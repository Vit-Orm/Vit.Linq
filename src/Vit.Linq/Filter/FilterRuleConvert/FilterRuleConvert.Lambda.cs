using System.Linq.Expressions;

using Vit.Linq.Filter.ComponentModel;

namespace Vit.Linq.Filter
{
    public partial class FilterRuleConvert
    {
        protected virtual FilterRule ConvertToData_Lambda(QueryAction queryAction, LambdaExpression lambda)
        {
            return ConvertToFilterRule(queryAction, lambda.Body);
        }

    }
}
