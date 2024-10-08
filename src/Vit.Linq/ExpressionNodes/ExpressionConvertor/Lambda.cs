﻿using System;
using System.Linq;
using System.Linq.Expressions;

using Vit.Linq.ExpressionNodes.ComponentModel;

namespace Vit.Linq.ExpressionNodes.ExpressionConvertor
{

    public class Lambda : IExpressionConvertor
    {
        public virtual int priority { get; set; } = 100;
        public ExpressionNode ConvertToData(ToDataArgument arg, Expression expression)
        {
            if (expression is LambdaExpression lambda)
            {
                var parameterNames = lambda.Parameters.Select(parameter => parameter.Name).ToArray();
                arg.RegisterParameterNames(parameterNames);

                var body = arg.convertService.ConvertToData(arg, lambda.Body);

                var parameterTypes = lambda.Parameters.Select(parameter => parameter.Type).ToArray();
                var returnType = lambda.ReturnType;
                return ExpressionNode.Lambda(parameterNames: parameterNames, body: body).Lambda_SetParamTypes(parameterTypes, returnType).Lambda_SetLambdaExpression(lambda);
            }

            return null;
        }

        public Expression ConvertToCode(ToCodeArgument arg, ExpressionNode data)
        {
            if (data.nodeType != NodeType.Lambda) return null;

            ExpressionNode_Lambda lambda = data;

            string[] parameterNames = lambda.parameterNames ?? new string[0];

            Type[] paramTypes = lambda.Lambda_GetParamTypes();
            paramTypes ??= new Type[0];

            var parameters = parameterNames.Select((name, i) =>
            {
                var type = paramTypes.Length > i ? paramTypes[i] : typeof(object);

                if (string.IsNullOrWhiteSpace(name)) return LinqHelp.CreateParameter(type, "lambdaParam");
                return Expression.Parameter(type, name);
            }).ToArray();

            arg = arg.WithParams(parameters);
            var expression = arg.convertService.ConvertToCode(arg, lambda.body);
            if (expression == null)
            {
                return null;
            }
            return Expression.Lambda(expression, parameters);
        }

    }
}
