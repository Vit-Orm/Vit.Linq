﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

using Vit.Linq.ExpressionNodes.ComponentModel;

namespace Vit.Linq.ExpressionNodes.ExpressionConvertor.MethodCalls.Enumerable_Methods
{

    /// <summary>
    /// Enumerable.ElementAt
    /// </summary>
    public class ElementAt : MethodConvertor_Common
    {
        public override Type methodType { get; } = typeof(Enumerable);
        public override Expression ToCode(ToCodeArgument arg, ExpressionNode_MethodCall call)
        {
            //var instance = convertService.ToExpression(arg, call.instance);
            var methodArguments = call.arguments?.Select(node => arg.convertService.ConvertToCode(arg, node)).ToArray();


            // Enumerable.ElementAt
            var type = methodArguments[0].Type.GetGenericArguments()[0];
            var method = (new Func<IEnumerable<string>, int, string>(Enumerable.ElementAt<string>))
                            .Method.GetGenericMethodDefinition().MakeGenericMethod(type);
            return Expression.Call(method, methodArguments);
        }
    }

}
