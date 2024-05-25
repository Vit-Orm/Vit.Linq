using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

using Vit.Linq.ExpressionTree.ComponentModel;

namespace Vit.Linq.ExpressionTree
{
    public class CodeConvertArgument
    {
        private CodeConvertArgument()
        {
        }

        public ExpressionConvertService convertService { get; set; }
        public List<ParameterExpression> parameters;

        public Func<ExpressionNode_New, Type> getResultTypeForNewNode;

        public static CodeConvertArgument WithParams(ExpressionConvertService convertService, List<ParameterExpression> parameters = null, Func<ExpressionNode_New, Type> getResultTypeForNewNode = null)
        {
            return new CodeConvertArgument { convertService = convertService, parameters = parameters, getResultTypeForNewNode = getResultTypeForNewNode };
        }

        public CodeConvertArgument WithParams(params ParameterExpression[] newParams)
        {
            var arg = new CodeConvertArgument { convertService = convertService, getResultTypeForNewNode = getResultTypeForNewNode };

            if (parameters?.Any() == true)
            {
                arg.parameters = parameters.ToList();
            }
            else
            {
                arg.parameters = new List<ParameterExpression>();
            }

            if (newParams.Any())
            {
                arg.parameters.AddRange(newParams);
            }
            return arg;
        }
    }
}
