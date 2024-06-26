using System;
using System.Collections.Generic;

using Vit.Linq.ExpressionTree.ComponentModel;

namespace Vit.Linq.ExpressionTree.CollectionQuery
{
    public partial class QueryAction
    {
        class ConvertArgument
        {
            public QueryAction queryAction;
            public string parameterName;
            public bool gettedOrder = false;

            public ExpressionNodeCloner cloner;
        }

        public static void LoadFromNode(QueryAction queryAction, ExpressionNode_Lambda lambda)
        {
            var arg = new ConvertArgument { queryAction = queryAction, parameterName = "m" };

            var cloner = new ExpressionNodeCloner();
            cloner.clone = (node) => Clone(arg, node);
            arg.cloner = cloner;

            var filter = ConvertFilter(arg, lambda.body);
            queryAction.filter = ExpressionNode.Lambda(parameterNames: new[] { arg.parameterName }, body: filter);
        }

        static ExpressionNode ConvertFilter(ConvertArgument arg, ExpressionNode node)
        {
            return arg.cloner.Clone(node);
        }

        static (bool success, ExpressionNode dest) Clone(ConvertArgument arg, ExpressionNode node)
        { 
            if (node == null) return default;

            switch (node.nodeType) 
            {
                case NodeType.Lambda:
                    {
                        ExpressionNode_Lambda lambda = node;
                        if (lambda.parameterNames?.Length == 1)
                            return (true,ConvertFilter(arg, lambda.body));
                        break;
                    }
                case NodeType.Member:
                    {
                        ExpressionNode_Member member = node;

                        var parameterName = string.IsNullOrWhiteSpace(member.parameterName) ? null : arg.parameterName;
                        var objectValue = member.objectValue;
                        if (objectValue != null)
                        {
                            objectValue = ConvertFilter(arg, objectValue);
                        }
                        return (true,ExpressionNode.Member(objectValue: objectValue, memberName: member.memberName, parameterName: parameterName));
                    }
                case NodeType.MethodCall:
                    {
                        ExpressionNode_MethodCall call = node;
                        switch (call.methodName)
                        {
                            case "Take":
                                {
                                    arg.queryAction.take = (call.arguments[1] as ExpressionNode_Constant)?.value as int?;
                                    return (true, ConvertFilter(arg, call.arguments[0]));
                                }

                            case "Skip":
                                {
                                    arg.queryAction.skip = (call.arguments[1] as ExpressionNode_Constant)?.value as int?;
                                    return (true, ConvertFilter(arg, call.arguments[0]));
                                }

                            case "OrderBy" or "OrderByDescending" or "ThenBy" or "ThenByDescending":
                                {
                                    if (!arg.gettedOrder)
                                    {
                                        var methodName = call.methodName;

                                        var memberField = call.arguments[1];

                                        var orderField = new OrderField { member = memberField, asc = !methodName.EndsWith("Descending") };

                                        if (methodName.StartsWith("Order"))
                                        {
                                            arg.gettedOrder = true;
                                        }

                                        if (arg.queryAction.orders == null)
                                            arg.queryAction.orders = new List<OrderField>();

                                        arg.queryAction.orders.Insert(0, orderField);

                                    }
                                    return (true, ConvertFilter(arg, call.arguments[0]));
                                }
                            case "Where":
                                {
                                    var source = call.arguments[0];
                                    var predicate = call.arguments[1];
                                    var predicateFilter = ConvertFilter(arg, predicate);
                                    if (predicateFilter.nodeType == NodeType.Lambda)
                                    {
                                        ExpressionNode_Lambda lambda = predicateFilter;
                                        if (lambda.parameterNames?.Length == 1)
                                        {
                                            predicateFilter = lambda.body;
                                        }
                                        else
                                        {
                                            throw new Exception("not supported Where filter with index");
                                        }
                                    }

                                    if (source == null || (source.nodeType == NodeType.Member && source.memberName == null && source.parameterName == null))
                                    {
                                        return (true, predicateFilter);
                                    }

                                    var left = ConvertFilter(arg, source);
                                    if (left == null || left.nodeType == NodeType.Member) return (true, predicateFilter);
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
                                    return (true, ExpressionNode.AndAlso(left: left, right: predicateFilter));
                                }
                            case "FirstOrDefault" or "First" or "LastOrDefault" or "Last":
                            case "Count":
                            case "TotalCount":
                                {
                                    if (!string.IsNullOrWhiteSpace(arg.queryAction.method)) throw new Exception("can not process multiple Method call");

                                    arg.queryAction.method = call.methodName;
                                    return (true, ConvertFilter(arg, call.arguments[0]));
                                }

                            case "IsNullOrEmpty":
                            case "ElementAt":
                            case "Contains":
                            case "Any":
                                {
                                    return (true, node);
                                }
                        }
                        throw new Exception("not supported method call : " + call.methodName);
                    }
            }

            return default;
        }



    }
}
