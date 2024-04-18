using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

using Vit.Linq.ExpressionTree.ComponentModel;

namespace Vit.Linq.ExpressionTree
{
    public partial class ExpressionConvertService
    {
        public interface IMethodConvertor
        {
            int priority { get; }
        }


        #region Data -> Code
        protected virtual Expression ConvertMethodToExpression(CodeConvertArgument arg, ExpressionNode_Call call)
        {
            throw new NotSupportedException($"Unsupported  ExpressionNode call : {call.methodName}");
        }
        #endregion

        #region Code -> Data
        ExpressionNode ConvertExpression(DataConvertArgument arg, MethodCallExpression call)
        {
            throw new NotSupportedException($"Unsupported method call : {call.Method.Name}");
        }
        #endregion
    }
}
