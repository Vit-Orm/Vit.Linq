using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

using Vit.Extensions.Linq_Extensions;
using Vit.Linq.ComponentModel;
using Vit.Linq.Filter.ComponentModel;
using Vit.Linq.Filter.MethodCalls;

namespace Vit.Linq.Filter
{
    public partial class FilterRuleConvert
    {

        protected List<IMethodConvertor> methodConvertors = new List<IMethodConvertor>();

        public virtual void RegisterMethodConvertor(IMethodConvertor convertor)
        {
            methodConvertors.Add(convertor);

            methodConvertors.Sort((a, b) => a.priority - b.priority);
        }



        protected virtual FilterRule ConvertToData_MethodCall(QueryAction queryAction, MethodCallExpression call)
        {
            var arg = new DataConvertArgument { queryAction = queryAction, convertService = this };
            var convertor = methodConvertors.FirstOrDefault(m => m.PredicateToData(arg, call));

            return convertor?.ToData(arg, call) ?? throw new NotSupportedException($"Unsupported method call: {call.Method.Name}");
        }


    }
}
