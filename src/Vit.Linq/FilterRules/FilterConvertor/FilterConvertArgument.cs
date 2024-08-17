using System;
using System.Linq.Expressions;

namespace Vit.Linq.FilterRules.FilterConvertor
{
    public class FilterConvertArgument
    {
        public FilterConvertArgument(FilterService filterService, ParameterExpression parameter)
        {
            this.filterService = filterService;
            this.parameter = parameter;
            comparison = filterService.operatorIsIgnoreCase ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal;
        }
        public FilterService filterService { get; set; }

        public ParameterExpression parameter { get; set; }

        public StringComparison comparison { get; set; }
    }
}
