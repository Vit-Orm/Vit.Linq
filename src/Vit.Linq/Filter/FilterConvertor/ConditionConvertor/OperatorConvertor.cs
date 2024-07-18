using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

using Vit.Linq;
using Vit.Linq.Filter.ComponentModel;

namespace Vit.Linq.Filter.FilterConvertor.ConditionConvertor
{
    public class OperatorConvertor : IConditionConvertor
    {
        public int priority { get; set; } = 900000;


        protected List<IOperatorConvertor> operatorConvertors = new();


        public List<Type> defaultConvertorTypes;

        public OperatorConvertor()
        {
            // populate
            defaultConvertorTypes ??= typeof(OperatorConvertor).Assembly.GetTypes()
                .Where(type => type.IsClass
                      && !type.IsAbstract
                      && typeof(IOperatorConvertor).IsAssignableFrom(type)
                      && type.GetConstructor(Type.EmptyTypes) != null
                ).ToList();
            defaultConvertorTypes.ForEach(type => RegisterOperatorConvertor(Activator.CreateInstance(type) as IOperatorConvertor));
        }

        public virtual void RegisterOperatorConvertor(IOperatorConvertor convertor)
        {
            operatorConvertors.Add(convertor);
            operatorConvertors.Sort((a, b) => a.priority - b.priority);
        }





        public (bool success, Expression expression) ConvertToCode(FilterConvertArgument arg, IFilterRule filter, string condition)
        {
            //if (filter.rules != null)
            //    return default;

            var Operator = arg.filterService.GetOperator(filter);


            Expression leftValueExpression = arg.filterService.GetLeftValueExpression(filter, arg.parameter);
            Type leftValueType = leftValueExpression.Type;

            var operatorConvertArg = new OperatorConvertArgument
            {
                filterService = arg.filterService,
                parameter = arg.parameter,
                filter = filter,
                leftValueExpression = leftValueExpression,
                leftValueType = leftValueType,
                Operator = Operator,
                comparison = arg.comparison,
            };


            foreach (var operatorConvertor in operatorConvertors)
            {
                var result = operatorConvertor.ConvertToCode(operatorConvertArg);
                if (result.success) return result;
            }

            if (string.IsNullOrWhiteSpace(Operator))
                return (true, default);

            throw new Exception("unrecognized operator : " + Operator);

        }
    }
}
