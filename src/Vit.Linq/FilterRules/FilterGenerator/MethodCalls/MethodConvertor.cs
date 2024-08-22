using System;
using System.Linq.Expressions;

using Vit.Linq.FilterRules.ComponentModel;

namespace Vit.Linq.FilterRules.MethodCalls
{
    public class MethodConverter : IMethodConvertor
    {
        public Func<FilterGenerateArgument, MethodCallExpression, bool> predicate;
        public Func<FilterGenerateArgument, MethodCallExpression, FilterRule> converter;

        /// <summary>
        /// 
        /// </summary>
        public int priority { get; set; }

        public MethodConverter(
            Func<FilterGenerateArgument, MethodCallExpression, bool> predicate,
            Func<FilterGenerateArgument, MethodCallExpression, FilterRule> converter,
            int priority = 100)
        {
            this.predicate = predicate;
            this.converter = converter;
            this.priority = priority;
        }



        public bool PredicateToData(FilterGenerateArgument arg, MethodCallExpression call) => predicate(arg, call);
        public FilterRule ToData(FilterGenerateArgument arg, MethodCallExpression call) => converter(arg, call);
    }
}
