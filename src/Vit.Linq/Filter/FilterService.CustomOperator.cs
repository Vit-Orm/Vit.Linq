using System;
using System.Collections.Generic;
using System.Linq.Expressions;

using Vit.Linq.Filter.ComponentModel;

namespace Vit.Linq.Filter
{
    public class OperatorBuilderArgs
    {
        public IFilterRule rule { get; set; }
        public ParameterExpression parameter { get; set; }
        public Expression leftValue { get; set; }
        public string Operator { get; set; }

        /// <summary>
        /// Type rightValueType
        /// </summary>
        public Func<Type, Expression> GetRightValueExpression { get; set; }
    }




    public partial class FilterService
    {
        #region Custom Operator

        Dictionary<string, Func<OperatorBuilderArgs, Expression>> customOperator = new Dictionary<string, Func<OperatorBuilderArgs, Expression>>();

        protected virtual Expression CustomOperator_ToExpression(OperatorBuilderArgs args)
        {
            if (customOperator.TryGetValue(args.Operator, out var operatorBuilder) && operatorBuilder != null)
            {
                return operatorBuilder(args);
            }
            return default;
        }

        public virtual void CustomOperator_Add(string Operator, Func<OperatorBuilderArgs, Expression> operatorBuilder)
        {
            if (operatorIsIgnoreCase) Operator = Operator.ToLower();
            customOperator[Operator] = operatorBuilder;
        }

        #endregion
    }
}
