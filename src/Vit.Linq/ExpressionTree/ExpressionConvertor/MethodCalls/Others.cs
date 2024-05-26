using System;
using System.Linq.Expressions;
using System.Linq;
using Vit.Linq.ExpressionTree.ComponentModel;
using System.Collections.Generic;
using System.Reflection;
using Vit.Extensions.Linq_Extensions;

namespace Vit.Linq.ExpressionTree.ExpressionConvertor.MethodCalls
{


    #region #10 Contain  ElementAt

    public class Contains : MethodConvertor_Common
    {
        public override Expression ToCode(CodeConvertArgument arg, ExpressionNode_MethodCall call)
        {
            var instance = arg.convertService.ToExpression(arg, call.@object);
            var methodArguments = call.arguments?.Select(node => arg.convertService.ToExpression(arg, node)).ToArray();

            // #1 instance Method
            if (instance != null)
            {
                // ##1 string.Contains
                // ##2 List<>.Contains
                // # ...
                return Expression.Call(instance, "Contains", null, methodArguments);
            }


            // #2 static Method
            MethodInfo method;
            var argType = methodArguments[0].Type;
            var modelType = argType.GetGenericArguments()[0];
            if (typeof(IQueryable).IsAssignableFrom(argType))
            {
                // Queryable.Contains                 
                method = (new Func<IQueryable<string>, string, bool>(Queryable.Contains<string>))
                            .Method.GetGenericMethodDefinition().MakeGenericMethod(modelType);
            }
            else
            {
                // Enumerable.Contains 
                method = (new Func<IQueryable<string>, string, bool>(Enumerable.Contains<string>))
                           .Method.GetGenericMethodDefinition().MakeGenericMethod(modelType);
            }
            return Expression.Call(method, methodArguments);
        }

    }


    public class ElementAt : MethodConvertor_Common
    {
        public override Type methodType { get; } = typeof(Enumerable);
        public override Expression ToCode(CodeConvertArgument arg, ExpressionNode_MethodCall call)
        {
            //var instance = convertService.ToExpression(arg, call.instance);
            var methodArguments = call.arguments?.Select(node => arg.convertService.ToExpression(arg, node)).ToArray();


            // Enumerable.ElementAt
            var type = methodArguments[0].Type.GetGenericArguments()[0];
            var method = (new Func<IEnumerable<string>, int, string>(Enumerable.ElementAt<string>))
                            .Method.GetGenericMethodDefinition().MakeGenericMethod(type);
            return Expression.Call(method, methodArguments);
        }
    }
    #endregion



}
