using System;
using System.Linq.Expressions;
using System.Linq;
using Vit.Linq.ExpressionTree.ComponentModel;
using System.Collections.Generic;
using System.Reflection;

namespace Vit.Linq.ExpressionTree.ExpressionConvertor.MethodCalls
{

    /// <summary>
    /// String.IsNullOrEmpty
    /// </summary>
    public class IsNullOrEmpty : MethodConvertor_Common
    {
        public override Type methodType { get; } = typeof(string);
        public override Expression ToCode(CodeConvertArgument arg, ExpressionNode_MethodCall call)
        {
            //var instance = convertService.ToExpression(arg, call.instance);
            var methodArguments = call.arguments?.Select(node => arg.convertService.ToExpression(arg, node)).ToArray();

            // string.IsNullOrEmpty
            return Expression.Call(typeof(string), "IsNullOrEmpty", null, methodArguments);
        }
    }


    /// <summary>
    /// String.Format
    /// </summary>
    public class Format : MethodConvertor_Common
    {
        public override Type methodType { get; } = typeof(string);
        public override Expression ToCode(CodeConvertArgument arg, ExpressionNode_MethodCall call)
        {

            //var instance = convertService.ToExpression(arg, call.instance);
            var methodArguments = call.arguments?.Select(node => arg.convertService.ToExpression(arg, node)).ToArray();

            // string.IsNullOrEmpty
            return Expression.Call(typeof(string), "Format", null, methodArguments);
        }
    }


    /// <summary>
    /// String.Concat
    /// </summary>
    public class Concat : MethodConvertor_Common
    {
        public override Type methodType { get; } = typeof(string);

        public override bool PredicateToData(DataConvertArgument arg, MethodCallExpression call)
        {
            return (call.Method.Name == "Add"|| call.Method.Name == "Concat") && methodType == call.Method.DeclaringType;
        }

        public override bool PredicateToCode(CodeConvertArgument arg, ExpressionNode_MethodCall call)
        {
            return (call.methodName == "Add"|| call.methodName == "Concat") && (methodType == null || methodType.Name == call.methodCall_typeName);
        }

        public override Expression ToCode(CodeConvertArgument arg, ExpressionNode_MethodCall call)
        {
            //var instance = convertService.ToExpression(arg, call.instance);
            var methodArguments = call.arguments?.Select(node => arg.convertService.ToExpression(arg, node)).ToArray();

            // string.IsNullOrEmpty
            return Expression.Call(typeof(string), "Concat", null, methodArguments);
        }
    }

}
