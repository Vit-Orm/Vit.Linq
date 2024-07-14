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
        public override FilterRule ToData(FilterGenerateArgument arg, MethodCallExpression call)
        {
            string left = arg.convertService.GetMemberName(call.Arguments[0]);
            return new FilterRule { field = left, @operator = "IsNull" };
        }
    }



}
