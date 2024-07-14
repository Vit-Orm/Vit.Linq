using System;
using System.Linq.Expressions;

using Vit.Linq.Filter.ComponentModel;

using Vit.Linq.Filter.MethodCalls;

namespace Vit.Linq.Filter.FilterGenerator.MethodCalls.String_Methods
{
    /// <summary>
    /// String.Contains
    /// </summary>
    public class Contains : MethodConvertor_Common
    {
        public override Type methodType { get; } = typeof(string);
        public override FilterRule ToData(FilterGenerateArgument arg, MethodCallExpression call)
        {
            var left = arg.convertService.GetMemberName(call.Object);
            var right = arg.convertService.GetValue(call.Arguments[0]);
            return new FilterRule { field = left?.ToString(), @operator = "Contains", value = right };
        }
    }
}
