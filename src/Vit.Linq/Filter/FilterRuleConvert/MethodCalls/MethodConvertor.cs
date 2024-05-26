using System;
using System.Linq.Expressions;

using Vit.Linq.Filter.ComponentModel;

namespace Vit.Linq.Filter.MethodCalls
{
    public class MethodConverter : IMethodConvertor
    {
        public Func<DataConvertArgument, MethodCallExpression, bool> predicate;
        public Func<DataConvertArgument, MethodCallExpression, FilterRule> converter;

        /// <summary>
        /// 
        /// </summary>
        public int priority { get; set; }

        public MethodConverter(
            Func<DataConvertArgument, MethodCallExpression, bool> predicate,
            Func<DataConvertArgument, MethodCallExpression, FilterRule> converter,
            int priority = 100)
        {
            this.predicate = predicate;
            this.converter = converter;
            this.priority = priority;
        }



        public bool PredicateToData(DataConvertArgument arg, MethodCallExpression call) => predicate(arg, call);
        public FilterRule ToData(DataConvertArgument arg, MethodCallExpression call) => converter(arg, call);
    }
}
