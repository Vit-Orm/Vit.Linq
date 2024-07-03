using System.Collections.Generic;
using System.Linq.Expressions;

namespace Vit.Linq.Filter.ComponentModel
{
    /// <summary>
    /// This interface is used to define a hierarchical filter for a given collection.
    /// </summary>
    public interface IFilterRule
    {
        /// <summary>
        ///  condition - acceptable values are "And" , "Or" , "Not" , "NotAnd" , "NotOr" .
        /// </summary>
        string condition { get; set; }

        /// <summary>
        /// could be nested, example: b1.name
        /// </summary>
        string field { get; set; }


        /// <summary>
        ///  Supported value :
        ///
        ///     "IsNull", "IsNotNull" ,
        ///     "=", "!=", "&gt;", "&lt;" , "&gt;=", "&lt;=", 
        ///     "In" , "NotIn" ,
        ///     "Contains", "NotContains", "StartsWith", "EndsWith" , "IsNullOrEmpty", "IsNotNullOrEmpty"
        ///
        ///    //TODO [array]   "is empty", "is not empty"
        /// </summary>
        string @operator { get; set; }

        /// <summary>
        /// nested filter rules
        /// </summary>
        IEnumerable<IFilterRule> rules { get; }



        /// <summary>
        /// value of the filter. Supported value types are "integer", "double", "string", "date", "datetime", and "boolean".
        /// </summary>
        object value { get; set; }

        Expression GetLeftValueExpression(Expression valueExpression);
    }
}
