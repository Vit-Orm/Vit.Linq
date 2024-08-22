using System.Linq.Expressions;

using Vit.Linq.FilterRules.ComponentModel;

namespace Vit.Linq.FilterRules.FilterConvertor.OperatorConvert
{
    public class IsNotNull : IOperatorConvertor
    {
        public int priority => 1000;

        public (bool success, Expression expression) ConvertToCode(OperatorConvertArgument arg)
        {
            if (RuleOperator.IsNotNull.Equals(arg.Operator, arg.comparison))
            {
                return (true, Expression.Not(IsNull.ConvertIsNull(arg)));
            }

            return default;
        }



    }
}
