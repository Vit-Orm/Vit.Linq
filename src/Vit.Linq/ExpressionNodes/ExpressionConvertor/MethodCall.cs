using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

using Vit.Linq.ExpressionNodes.ComponentModel;
using Vit.Linq.ExpressionNodes.ExpressionConvertor.MethodCalls;

namespace Vit.Linq.ExpressionNodes.ExpressionConvertor
{


    /*
Supported methods:

#1. Contains
string.Contains
List<>.Contains
Queryable.Contains
Enumerable.Contains

#2. IsNullOrEmpty
string.IsNullOrEmpty

#3. ElementAt
Enumerable.ElementAt



#10 Contain IsNullOrEmpty ElementAt

#11. Take
#12. Skip



#80 Where
Queryable.Where

#81 Order
Queryable.OrderBy OrderByDescending ThenBy ThenByDescending

#82 Any
Queryable.Any



#99. InstanceMethod
String.StartsWith
String.EndsWith

     */


    public partial class MethodCall : IExpressionConvertor
    {
        public virtual int priority { get; set; } = 100;

        protected List<IMethodConvertor> methodConvertors = new List<IMethodConvertor>();

        public virtual void RegisterMethodConvertor(IMethodConvertor convertor)
        {
            methodConvertors.Add(convertor);

            methodConvertors.Sort((a, b) => a.priority - b.priority);
        }

        public MethodCall()
        {
            // populate MethodConvertor
            var types = GetType().Assembly.GetTypes().Where(type => type.IsClass
                    && !type.IsAbstract
                    && typeof(IMethodConvertor).IsAssignableFrom(type)
                    && !typeof(Attribute).IsAssignableFrom(type)
                    && type.GetConstructor(Type.EmptyTypes) != null
            ).ToList();

            types.ForEach(type => RegisterMethodConvertor(Activator.CreateInstance(type) as IMethodConvertor));
        }


        public ExpressionNode ConvertToData(ToDataArgument arg, Expression expression)
        {
            if (expression is MethodCallExpression methodCall)
            {
                foreach (var convertor in methodConvertors)
                {
                    var result = convertor.ToData(arg, methodCall);
                    if (result.success) return result.node;
                }

                throw new NotSupportedException($"Unsupported method call: {methodCall.Method.Name}");
            }

            return null;
        }

        public Expression ConvertToCode(ToCodeArgument arg, ExpressionNode data)
        {
            if (data.nodeType != NodeType.MethodCall) return null;

            ExpressionNode_MethodCall call = data;

            foreach (var convertor in methodConvertors)
            {
                var result = convertor.ToCode(arg, call);
                if (result.success) return result.expression;
            }

            throw new NotSupportedException($"Method not supported: {call.methodName}");
        }




    }
}
