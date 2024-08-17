using System;
using System.Linq;
using System.Linq.Expressions;

using Vit.Linq.ExpressionNodes.ComponentModel;

namespace Vit.Linq.ExpressionNodes.ExpressionConvertor
{
    public partial class MethodCall
    {
        public static ExpressionNode ConvertToData(ToDataArgument arg, MethodCallExpression call)
        {
            var method = call.Method;

            // #1 Object
            ExpressionNode @object = call.Object == null ? null : arg.convertService.ConvertToData(arg, call.Object);

            // #2 typeArguments
            // var typeArguments = method.GetGenericArguments();

            // #3 Arguments
            var arguments = call.Arguments?.Select(param => arg.convertService.ConvertToData(arg, param)).ToArray();
            if (arguments?.Any() == true)
            {
                var paramArray = method.GetParameters();
                for (int i = Math.Min(arguments.Length, paramArray.Length) - 1; i >= 0; i--)
                {
                    if (arguments[i]?.nodeType == NodeType.Constant)
                    {
                        ExpressionNode_Constant constant = arguments[i];
                        constant.valueType = ComponentModel.NodeValueType.FromType(paramArray[i].ParameterType);
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

            return ExpressionNode.MethodCall(method: method, @object: @object, arguments: arguments);
        }

    }
}
