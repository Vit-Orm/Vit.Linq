using System;
using System.Linq.Expressions;
using System.Linq;
using Vit.Linq.ExpressionTree.ComponentModel;

namespace Vit.Linq.ExpressionTree.ExpressionConvertor.MethodCalls
{
    public abstract class MethodConvertor_Base : IMethodConvertor
    {
        public virtual int priority { get; set; } = 100;
        public abstract bool PredicateToCode(CodeConvertArgument arg, ExpressionNode_MethodCall call);
        public abstract Expression ToCode(CodeConvertArgument arg, ExpressionNode_MethodCall call);

        public abstract bool PredicateToData(DataConvertArgument arg, MethodCallExpression call);
        public virtual ExpressionNode ToData(DataConvertArgument arg, MethodCallExpression call)
        {
            var method = call.Method;

            // #1 typeName
            var typeName = method.DeclaringType.Name;

            // #2 Object
            ExpressionNode @object = call.Object == null ? null : arg.convertService.ConvertToData(arg, call.Object);


            // #3 methodName
            var methodName = method.Name;

            // #4 typeArguments
            // var typeArguments = method.GetGenericArguments();

            // #5 Arguments
            var arguments = call.Arguments?.Select(param => arg.convertService.ConvertToData(arg, param)).ToArray();
            if (arguments?.Any() == true)
            {
                var paramArray = method.GetParameters();
                for (int i = Math.Min(arguments.Length, paramArray.Length) - 1; i >= 0; i--)
                {
                    if (arguments[i]?.nodeType == NodeType.Constant)
                    {
                        ExpressionNode_Constant constant = arguments[i];
                        constant.valueType = ComponentModel.ValueType.FromType(paramArray[i].ParameterType);
                    }
                }
            }


            // if all arguments is constant, directly evaluate constant value
            //var nodeTypes = new List<string> { instance?.nodeType };
            //if (methodArguments != null) nodeTypes.AddRange(methodArguments.Select(m => m?.nodeType));
            //if (nodeTypes.Where(nodeTypes => nodeTypes != null).All(nodeType => nodeType == ExpressionNodeType.Constant))
            //{
            //    var value = GetValue(call);
            //    return ExpressionNode_Constant.FromValue(value);
            //}

            return ExpressionNode.MethodCall(typeName: typeName, methodName: methodName, @object: @object, arguments: arguments);
        }



    }
}
