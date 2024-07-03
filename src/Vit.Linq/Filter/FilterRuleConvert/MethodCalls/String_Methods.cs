using System;
using System.Linq.Expressions;

using Vit.Linq.Filter.ComponentModel;

namespace Vit.Linq.Filter.MethodCalls.String_Methods
{

    /// <summary>
    /// String.IsNullOrEmpty
    /// </summary>
    public class IsNullOrEmpty : MethodConvertor_Common
    {
        public override Type methodType { get; } = typeof(string);
        public override FilterRule ToData(DataConvertArgument arg, MethodCallExpression call)
        {
            string left = arg.convertService.GetMemberName(call.Arguments[0]);
            return new FilterRule { field = left, @operator = "IsNull" };
        }
    }

    /// <summary>
    /// String.Contains
    /// </summary>
    public class Contains : MethodConvertor_Common
    {
        public override Type methodType { get; } = typeof(string);
        public override FilterRule ToData(DataConvertArgument arg, MethodCallExpression call)
        {
            var left = arg.convertService.GetMemberName(call.Object);
            var right = arg.convertService.GetValue(call.Arguments[0]);
            return new FilterRule { field = left?.ToString(), @operator = "Contains", value = right };
        }
    }

}
