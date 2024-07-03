using System;



namespace Vit.Linq.ExpressionTree.ComponentModel
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

        public static ExpressionNode MethodCall(string methodCall_typeName = null, string methodName = null, ExpressionNode @object = null, ExpressionNode[] arguments = null)
        {
            return new ExpressionNode
            {
                nodeType = NodeType.MethodCall,
                methodCall_typeName = methodCall_typeName,
                methodName = methodName,
                @object = @object,
                arguments = arguments,
            };
        }

        public Type[] MethodCall_GetParamTypes()
        {
            return GetCodeArg("MethodCall_ParamTypes") as Type[];
        }
        public Type MethodCall_GetReturnType()
        {
            return GetCodeArg("MethodCall_ReturnType") as Type;
        }

        public ExpressionNode MethodCall_SetParamTypes(Type[] paramTypes, Type returnType = null)
        {
            if (paramTypes != null)
                SetCodeArg("MethodCall_ParamTypes", paramTypes);
            if (returnType != null)
                SetCodeArg("MethodCall_ReturnType", returnType);
            return this;
        }
    }
}
