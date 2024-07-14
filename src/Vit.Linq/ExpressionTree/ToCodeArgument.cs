using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

using Vit.Linq.ExpressionTree.ComponentModel;

namespace Vit.Linq.ExpressionTree
{
    public class ToCodeArgument
    {
        private ToCodeArgument()
        {
        }

        public ExpressionConvertService convertService { get; set; }
        public List<ParameterExpression> parameters;

        public Func<ExpressionNode_New, Type> getResultTypeForNewNode;

        public static ToCodeArgument WithParams(ExpressionConvertService convertService, List<ParameterExpression> parameters = null, Func<ExpressionNode_New, Type> getResultTypeForNewNode = null)
        {
            return new ToCodeArgument { convertService = convertService, parameters = parameters, getResultTypeForNewNode = getResultTypeForNewNode };
        }

        public ToCodeArgument WithParams(params ParameterExpression[] newParams)
        {
            var arg = new ToCodeArgument { convertService = convertService, getResultTypeForNewNode = getResultTypeForNewNode };

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
