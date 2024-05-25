using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;



namespace Vit.Linq.ExpressionTree.ComponentModel.CollectionQuery
{
    public partial class QueryAction
    {
        class ConvertArgument
        {
            public QueryAction queryAction;
            public string parameterName;
            public bool _gettedOrder = false;

            public ExpressionNodeCloner cloner;
        }

        public static void LoadFromNode(QueryAction queryAction, ExpressionNode_Lambda lambda)
        {
            var arg = new ConvertArgument { queryAction = queryAction, parameterName = "m" };

            var cloner = new ExpressionNodeCloner();
            cloner.clone = (node) => (true, ConvertFilter(arg, node));
            arg.cloner = cloner;

            var filter = ConvertFilter(arg, lambda.body);
            queryAction.filter = ExpressionNode.Lambda(parameterNames: new[] { arg.parameterName }, body: filter);
        }

        static ExpressionNode ConvertFilter(ConvertArgument arg, ExpressionNode node)
        {
            var dest = ConvertFilter(arg, node, out var success);

            if (success) return dest;

            return arg.cloner.Clone(node);
        }

        static ExpressionNode ConvertFilter(ConvertArgument arg, ExpressionNode node, out bool success)
        {
            success = true;
            if (node == null) return null;

            if (node.nodeType == NodeType.Lambda)
            {
                ExpressionNode_Lambda lambda = node;
                if (lambda.parameterNames?.Length == 1)
                    return ConvertFilter(arg, lambda.body);
            }

            if (node.nodeType == NodeType.Member)
            {
                ExpressionNode_Member member = node;

                var parameterName = string.IsNullOrWhiteSpace(member.parameterName) ? null : arg.parameterName;
                var objectValue = member.objectValue;
                if (objectValue != null)
                {
                    objectValue = ConvertFilter(arg, objectValue);
                }
                return ExpressionNode.Member(objectValue: objectValue, memberName: member.memberName, parameterName: parameterName);
            }

            #region ExpressionNode_Call
            if (node.nodeType == NodeType.MethodCall)
            {
                ExpressionNode_MethodCall call = node;
                switch (call.methodName)
                {
                    case "Take":
                        {
                            arg.queryAction.take = (call.arguments[1] as ExpressionNode_Constant)?.value as int?;
                            return ConvertFilter(arg, call.arguments[0]);
                        }

                    case "Skip":
                        {
                            arg.queryAction.skip = (call.arguments[1] as ExpressionNode_Constant)?.value as int?;
                            return ConvertFilter(arg, call.arguments[0]);
                        }

                    case "OrderBy":
                    case "OrderByDescending":
                    case "ThenBy":
                    case "ThenByDescending":
                        {
                            if (!arg._gettedOrder)
                            {
                                var methodName = call.methodName;

                                var memberField = call.arguments[1];

                                var orderParam = new SortField { member = memberField, asc = !methodName.EndsWith("Descending") };

                                if (methodName.StartsWith("Order"))
                                {
                                    arg._gettedOrder = true;
                                }

                                if (arg.queryAction.orders == null)
                                    arg.queryAction.orders = new List<SortField> { orderParam };
                                else
                                    arg.queryAction.orders.Insert(0, orderParam);

                            }
                            return ConvertFilter(arg, call.arguments[0]);
                        }
                    case "Where":
                        {
                            var source = call.arguments[0];
                            var predicate = call.arguments[1];
                            var right = ConvertFilter(arg, predicate);
                            if (right.nodeType == NodeType.Lambda)
                            {
                                ExpressionNode_Lambda lambda = right;
                                if (lambda.parameterNames?.Length == 1)
                                {
                                    right = lambda.body;
                                }
                                else
                                {
                                    throw new Exception("not supported Where filter with index");
                                }
                            }

                            if (source == null || (source.nodeType == NodeType.Member && source.memberName == null && source.parameterName == null))
                            {
                                return right;
                            }

                            var left = ConvertFilter(arg, source);
                            if (left == null || left.nodeType == NodeType.Member) return right;
                            if (left.nodeType == NodeType.Lambda)
                            {
                                ExpressionNode_Lambda lambda2 = left;
                                if (lambda2.parameterNames?.Length == 1)
                                {
                                    left = lambda2.body;
                                }
                                else
                                {
                                    throw new Exception("not supported Where filter with index");
                                }
                            }
                            return ExpressionNode.And(left: left, right: right);
                        }
                    case "TotalCount":
                    case "First":
                    case "FirstOrDefault":
                    case "Last":
                    case "LastOrDefault":
                    case "Count":
                        {
                            if (!string.IsNullOrWhiteSpace(arg.queryAction.method)) throw new Exception("can not process multiple Method call");

                            arg.queryAction.method = call.methodName;
                            return ConvertFilter(arg, call.arguments[0]);
                        }

                    case "IsNullOrEmpty":
                    case "ElementAt":

                    case "Contains":
                    case "Any":
                        {
                            return node;
                        }
                }
                throw new Exception("not supported method call : " + call.methodName);
            }
            #endregion

            success = false;
            return null;
        }



    }
}
