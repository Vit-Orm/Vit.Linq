using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

using Vit.Extensions.Linq_Extensions;
using Vit.Linq.ComponentModel;
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
