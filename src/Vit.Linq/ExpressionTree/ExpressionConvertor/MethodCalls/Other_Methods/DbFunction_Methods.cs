using System;
using System.Linq.Expressions;
using System.Linq;
using Vit.Linq.ExpressionTree.ComponentModel;
using System.Collections.Generic;
using Vitorm;

namespace Vit.Linq.ExpressionTree.ExpressionConvertor.MethodCalls.Other_Methods
{

    public class Db_Methods : MethodConvertor_Base
    {

        public Type methodType { get; } = typeof(DbFunction);

        static readonly List<string> methodNames = typeof(DbFunction).GetMethods().Select(m => m.Name).ToList();
        public override int priority => 10000;

        public override bool PredicateToData(DataConvertArgument arg, MethodCallExpression call)
        {
            // is method from Queryable
            return methodType == call.Method.DeclaringType;
        }


        public override bool PredicateToCode(CodeConvertArgument arg, ExpressionNode_MethodCall call)
        {
            return methodType.Name == call.methodCall_typeName && methodNames.Contains(call.methodName);
        }

        public override Expression ToCode(CodeConvertArgument arg, ExpressionNode_MethodCall call)
        {
            throw new NotSupportedException($"Unsupported method typeName: {call.methodCall_typeName}, methodName: {call.methodName}");
        }
    }



}
