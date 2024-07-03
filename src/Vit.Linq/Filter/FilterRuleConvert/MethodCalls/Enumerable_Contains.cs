using System;
using System.Linq;
using System.Linq.Expressions;

using Vit.Linq.Filter.ComponentModel;

namespace Vit.Linq.Filter.MethodCalls.Enumerable_Contains
{

    /// <summary>
    /// Enumerable.Contains
    /// </summary>
    public class Contains : MethodConvertor_Common
    {
        public override Type methodType { get; } = typeof(Enumerable);
        public override FilterRule ToData(DataConvertArgument arg, MethodCallExpression call)
        {
            var left = arg.convertService.GetMemberName(call.Arguments[1]);
            var right = arg.convertService.GetValue(call.Arguments[0]);
            return new FilterRule { field = left, @operator = "In", value = right };
        }
    }


}
