using System;
using System.Collections.Generic;
using System.Linq;

using Vit.Linq.ExpressionNodes.ExpressionConvertor;
using Vit.Linq.ExpressionNodes.ExpressionConvertor.MethodCalls;

namespace Vit.Linq.ExpressionNodes
{
    public partial class ExpressionConvertService
    {
        public static ExpressionConvertService Instance = new ExpressionConvertService();

        protected List<IExpressionConvertor> expressionConvertors = new List<IExpressionConvertor>();

        public ExpressionConvertService()
        {
            // populate ExpressionConvertor
            {
                var types = GetType().Assembly.GetTypes().Where(type => type.IsClass
                        && !type.IsAbstract
                        && typeof(IExpressionConvertor).IsAssignableFrom(type)
                        && type.GetConstructor(Type.EmptyTypes) != null
                ).ToList();

                types.ForEach(type => AddExpresssionConvertor(Activator.CreateInstance(type) as IExpressionConvertor));
            }
        }


        public virtual void AddExpresssionConvertor(IExpressionConvertor convertor)
        {
            expressionConvertors.Add(convertor);

            expressionConvertors.Sort((a, b) => a.priority - b.priority);
        }





        public virtual bool RegisterMethodConvertor(IMethodConvertor convertor)
        {
            var methodCallConvertor = expressionConvertors.FirstOrDefault(m => m is MethodCall) as MethodCall;

            if (methodCallConvertor == null) return false;

            methodCallConvertor?.RegisterMethodConvertor(convertor);

            return true;
        }


    }
}
