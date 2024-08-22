using System;
using System.Linq;
using System.Reflection;

namespace Vit.Linq.ExpressionNodes.ComponentModel
{
    public interface ExpressionNode_MethodCall : IExpressionNode
    {
        /// <summary>
        ///     the System.Linq.Expressions.Expression that represents the instance for instance method calls or null for static method calls.
        /// </summary>
        public ExpressionNode @object { get; }

        public string methodCall_typeName { get; }
        public string methodName { get; }

        public ExpressionNode[] arguments { get; }

        Type[] MethodCall_GetParamTypes();
        Type MethodCall_GetReturnType();
        ExpressionNode MethodCall_SetParamTypes(Type[] paramTypes, Type returnType = null);

        ExpressionNode MethodCall_SetMethod(MethodInfo method);
        MethodInfo MethodCall_GetMethod();
    }

    public partial class ExpressionNode : ExpressionNode_MethodCall
    {

        /// <summary>
        ///     the System.Linq.Expressions.Expression that represents the instance for instance method calls or null for static method calls.
        /// </summary>
        public ExpressionNode @object { get; set; }

        public string methodCall_typeName { get; set; }
        public string methodName { get; set; }

        public ExpressionNode[] arguments { get; set; }

        public static ExpressionNode MethodCall(MethodInfo method, ExpressionNode @object = null, ExpressionNode[] arguments = null)
        {
            // #1 typeName
            var methodCall_typeName = method.DeclaringType.Name;

            // #2 methodName
            var methodName = method.Name;

            // #3 type
            var parameterTypes = method.GetParameters().Select(parameter => parameter.ParameterType).ToArray();
            var returnType = method.ReturnType;

            return new ExpressionNode
            {
                nodeType = NodeType.MethodCall,
                methodCall_typeName = methodCall_typeName,
                methodName = methodName,
                @object = @object,
                arguments = arguments,
            }
            .MethodCall_SetMethod(method)
            .MethodCall_SetParamTypes(parameterTypes: parameterTypes, returnType: returnType)
            ;
        }

        public Type[] MethodCall_GetParamTypes()
        {
            return GetCodeArg("MethodCall_ParameterTypes") as Type[];
        }
        public Type MethodCall_GetReturnType()
        {
            return GetCodeArg("MethodCall_ReturnType") as Type;
        }

        public ExpressionNode MethodCall_SetParamTypes(Type[] parameterTypes, Type returnType = null)
        {
            if (parameterTypes != null)
                SetCodeArg("MethodCall_ParameterTypes", parameterTypes);
            if (returnType != null)
                SetCodeArg("MethodCall_ReturnType", returnType);
            return this;
        }

        public ExpressionNode MethodCall_SetMethod(MethodInfo method)
        {
            if (method != null)
                SetCodeArg("MethodCall_Method", method);
            return this;
        }
        public MethodInfo MethodCall_GetMethod()
        {
            return GetCodeArg("MethodCall_Method") as MethodInfo;
        }


    }
}
