using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

using Vit.Linq.ExpressionTree.ComponentModel;

namespace Vit.Linq.ExpressionTree.ExpressionConvertor.MethodCalls.Other_Methods
{

    /// <summary>
    /// #1 instance Method
    ///   ##1 string.Contains
    ///   ##2 List.Contains
    ///   ...
    /// #2 static Method
    ///   ##1 Queryable.Contains
    ///   ##2 Enumerable.Contains 
    /// 
    /// </summary>
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
                // ##1 Queryable.Contains
                method = (new Func<IQueryable<string>, string, bool>(Queryable.Contains<string>))
                            .Method.GetGenericMethodDefinition().MakeGenericMethod(modelType);
            }
            else
            {
                // ##2 Enumerable.Contains 
                method = (new Func<IQueryable<string>, string, bool>(Enumerable.Contains<string>))
                           .Method.GetGenericMethodDefinition().MakeGenericMethod(modelType);
            }
            return Expression.Call(method, methodArguments);
        }

    }





}
