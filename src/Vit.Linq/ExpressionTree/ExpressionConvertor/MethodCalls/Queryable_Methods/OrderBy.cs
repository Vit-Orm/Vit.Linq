using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

using Vit.Linq.ExpressionTree.ComponentModel;

namespace Vit.Linq.ExpressionTree.ExpressionConvertor.MethodCalls.Queryable_Methods
{

    /// <summary>
    ///  Queryable.OrderBy OrderByDescending ThenBy ThenByDescending
    /// </summary>
    public class OrderBy : MethodConvertor_Base
    {

        public Type methodType { get; } = typeof(Queryable);

        static readonly List<string> methodNames = new List<string> { "OrderBy", "OrderByDescending", "ThenBy", "ThenByDescending" };

        // public static IOrderedQueryable<TSource> OrderBy<TSource, TKey>(this IQueryable<TSource> source, Expression<Func<TSource, TKey>> keySelector);
        // public static IOrderedQueryable<TSource> OrderByDescending<TSource, TKey>(this IQueryable<TSource> source, Expression<Func<TSource, TKey>> keySelector);
        // public static IOrderedQueryable<TSource> ThenBy<TSource, TKey>(this IOrderedQueryable<TSource> source, Expression<Func<TSource, TKey>> keySelector); 
        // public static IOrderedQueryable<TSource> ThenByDescending<TSource, TKey>(this IOrderedQueryable<TSource> source, Expression<Func<TSource, TKey>> keySelector); 

        public override bool PredicateToData(DataConvertArgument arg, MethodCallExpression call)
        {
            return methodType == call.Method.DeclaringType == methodNames.Contains(call.Method.Name);
        }

        public override bool PredicateToCode(CodeConvertArgument arg, ExpressionNode_MethodCall call)
        {
            return methodNames.Contains(call.methodName) && methodType.Name == call.methodCall_typeName;
        }

        public override Expression ToCode(CodeConvertArgument arg, ExpressionNode_MethodCall call)
        {
            var expSource = arg.convertService.ToExpression(arg, call.arguments[0]);
            var elementType = expSource.Type.GetGenericArguments()[0];

            var lambda = call.arguments[1] as ExpressionNode_Lambda;
            var expKeySelector = arg.convertService.ToLambdaExpression(arg, lambda, elementType);
            var keyType = expKeySelector.ReturnType;

            MethodInfo method = null;

            switch (call.methodName)
            {
                case "OrderBy": // public static IOrderedQueryable<TSource> OrderBy<TSource, TKey>(this IQueryable<TSource> source, Expression<Func<TSource, TKey>> keySelector); 
                    method = (new Func<IQueryable<object>, Expression<Func<object, string>>, IOrderedQueryable<object>>(Queryable.OrderBy<object, string>))
                        .Method.GetGenericMethodDefinition().MakeGenericMethod(elementType, keyType);
                    break;
                case "OrderByDescending": // public static IOrderedQueryable<TSource> OrderByDescending<TSource, TKey>(this IQueryable<TSource> source, Expression<Func<TSource, TKey>> keySelector); 
                    method = (new Func<IQueryable<object>, Expression<Func<object, string>>, IOrderedQueryable<object>>(Queryable.OrderByDescending<object, string>))
                        .Method.GetGenericMethodDefinition().MakeGenericMethod(elementType, keyType);
                    break;
                case "ThenBy": // public static IOrderedQueryable<TSource> ThenBy<TSource, TKey>(this IOrderedQueryable<TSource> source, Expression<Func<TSource, TKey>> keySelector); 
                    method = (new Func<IOrderedQueryable<object>, Expression<Func<object, string>>, IOrderedQueryable<object>>(Queryable.ThenBy<object, string>))
                        .Method.GetGenericMethodDefinition().MakeGenericMethod(elementType, keyType);
                    break;
                case "ThenByDescending": // public static IOrderedQueryable<TSource> ThenByDescending<TSource, TKey>(this IOrderedQueryable<TSource> source, Expression<Func<TSource, TKey>> keySelector); 
                    method = (new Func<IOrderedQueryable<object>, Expression<Func<object, string>>, IOrderedQueryable<object>>(Queryable.ThenByDescending<object, string>))
                        .Method.GetGenericMethodDefinition().MakeGenericMethod(elementType, keyType);
                    break;
            }

            var methodArguments = new[] { expSource, expKeySelector };
            return Expression.Call(method, methodArguments);
        }
    }


}
