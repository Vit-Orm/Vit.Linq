using System.Linq.Expressions;

using Vit.Linq.FilterRules.ComponentModel;

namespace Vit.Linq.FilterRules.FilterConvertor.OperatorConvert
{
    public class NotIn : IOperatorConvertor
    {
        public int priority => 3000;

        public (bool success, Expression expression) ConvertToCode(OperatorConvertArgument arg)
        {
            if (RuleOperator.NotIn.Equals(arg.Operator, arg.comparison))
            {
                return (true, Expression.Not(In.ConvertIn(arg)));
            }

            return default;
        }



    }
}
