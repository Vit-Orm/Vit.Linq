using System;
using System.Collections.Generic;
using System.Text;



namespace Vit.Linq.ExpressionTree.ComponentModel
{

    public interface ExpressionNode_Lambda : IExpressionNode
    {
        public string[] parameterNames { get; }

        public ExpressionNode body { get; }

        Type[] Lambda_GetParamTypes();
        ExpressionNode Lambda_SetParamTypes(Type[] paramTypes);
    }

    public partial class ExpressionNode : ExpressionNode_Lambda
    {
        public string[] parameterNames { get; set; }

        public ExpressionNode body { get; set; }

        public static ExpressionNode Lambda(string[] parameterNames, ExpressionNode body)
        {
            return new ExpressionNode
            {
                nodeType = NodeType.Lambda,
                parameterNames = parameterNames,
                body = body,
            };
        }
        public Type[] Lambda_GetParamTypes()
        {
            return GetCodeArg("Lambda_ParamTypes") as Type[];
        }

        public ExpressionNode Lambda_SetParamTypes(Type[] paramTypes)
        {
            SetCodeArg("Lambda_ParamTypes", paramTypes);
            return this;
        }


    }
}
