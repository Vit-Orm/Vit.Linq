using System;
using System.Collections.Generic;
using System.Linq;

using Vit.Linq.ExpressionTree.ExpressionConvertor;
using Vit.Linq.ExpressionTree.ExpressionConvertor.MethodCalls;

namespace Vit.Linq.ExpressionTree
{
    public partial class ExpressionConvertService
    {
        public static ExpressionConvertService Instance = new ExpressionConvertService();
 
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

        protected List<IExpressionConvertor> expressionConvertors = new List<IExpressionConvertor>();
        public virtual void AddExpresssionConvertor(IExpressionConvertor convertor)
        {
            expressionConvertors.Add(convertor);
        }





        public virtual void RegisterMethodConvertor(IMethodConvertor convertor)
        {
            var methodCallConvertor = expressionConvertors.FirstOrDefault(m => m is MethodCall) as MethodCall;
            methodCallConvertor?.RegisterMethodConvertor(convertor);
        }


    }
}
