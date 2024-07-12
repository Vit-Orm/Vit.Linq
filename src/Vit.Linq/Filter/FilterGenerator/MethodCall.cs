using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

using Vit.Linq.Filter.ComponentModel;
using Vit.Linq.Filter.MethodCalls;

namespace Vit.Linq.Filter.FilterGenerator
{
    public class MethodCall : IFilterGenerator
    {
        public virtual int priority { get; set; } = 500;

        protected List<IMethodConvertor> methodConvertors = new List<IMethodConvertor>();


        public MethodCall()
        {
            // populate MethodConvertor
            {
                var types = GetType().Assembly.GetTypes().Where(type => type.IsClass
                        && !type.IsAbstract
                        && typeof(IMethodConvertor).IsAssignableFrom(type)
                        && type.GetConstructor(Type.EmptyTypes) != null
                ).ToList();

                types.ForEach(type => RegisterMethodConvertor(Activator.CreateInstance(type) as IMethodConvertor));
            }
        }

        public virtual void RegisterMethodConvertor(IMethodConvertor convertor)
        {
            methodConvertors.Add(convertor);

            methodConvertors.Sort((a, b) => a.priority - b.priority);
        }



        public FilterRule ConvertToData(FilterGenerateArgument arg, Expression expression)
        {
            if (expression is not MethodCallExpression call) return null;

            var convertor = methodConvertors.FirstOrDefault(m => m.PredicateToData(arg, call));

            return convertor?.ToData(arg, call) ?? throw new NotSupportedException($"Unsupported method call: {call.Method.Name}");
        }


    }
}
