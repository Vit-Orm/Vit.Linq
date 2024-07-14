using System;
using System.Collections.Generic;
using System.Linq;

using Vit.Linq.ExpressionTree.ComponentModel;

namespace Vit.Linq.ExpressionTree.Query
{
    public partial class QueryAction
    {
        class ConvertArgument
        {
            public QueryAction collectionQuery;
            public string parameterName;
            public bool gettedOrder = false;

            public ExpressionNodeCloner cloner;
        }

        public static void LoadFromNode(QueryAction collectionQuery, ExpressionNode_Lambda lambda)
        {
            var arg = new ConvertArgument { collectionQuery = collectionQuery, parameterName = "m" };

            var cloner = new ExpressionNodeCloner();
            cloner.clone = (node) => Clone(arg, node);
            arg.cloner = cloner;

            var filter = ConvertFilter(arg, lambda.body);
            collectionQuery.filter = ExpressionNode.Lambda(parameterNames: new[] { arg.parameterName }, body: filter);
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
                            return (true, ConvertFilter(arg, lambda.body));
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
                        return (true, ExpressionNode.Member(objectValue: objectValue, memberName: member.memberName, parameterName: parameterName));
                    }
                case NodeType.MethodCall:
                    {
                        ExpressionNode_MethodCall call = node;
                        switch (call.methodName)
                        {
                            case nameof(Queryable.Take):
                                {
                                    arg.collectionQuery.take = (call.arguments[1] as ExpressionNode_Constant)?.value as int?;
                                    return (true, ConvertFilter(arg, call.arguments[0]));
                                }
                            case nameof(Queryable.Skip):
                                {
                                    arg.collectionQuery.skip = (call.arguments[1] as ExpressionNode_Constant)?.value as int?;
                                    return (true, ConvertFilter(arg, call.arguments[0]));
                                }
                            case nameof(Queryable.OrderBy) or nameof(Queryable.OrderByDescending) or nameof(Queryable.ThenBy) or nameof(Queryable.ThenByDescending):
                                {
                                    if (!arg.gettedOrder)
                                    {
                                        var methodName = call.methodName;

                                        var memberField = call.arguments[1];

                                        var orderField = new ExpressionNodeOrderField { member = memberField, asc = !methodName.EndsWith("Descending") };

                                        if (methodName.StartsWith("Order"))
                                        {
                                            arg.gettedOrder = true;
                                        }

                                        arg.collectionQuery.orders ??= new List<ExpressionNodeOrderField>();

                                        arg.collectionQuery.orders.Insert(0, orderField);

                                    }
                                    return (true, ConvertFilter(arg, call.arguments[0]));
                                }
                            case nameof(Queryable.Where):
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
                            case nameof(Queryable.FirstOrDefault) or nameof(Queryable.First) or nameof(Queryable.LastOrDefault) or nameof(Queryable.Last):
                            case nameof(Queryable.Count):
                            case nameof(Queryable_Extensions.ToListAndTotalCount) or nameof(Queryable_Extensions.TotalCount):
                                {
                                    if (!string.IsNullOrWhiteSpace(arg.collectionQuery.method)) throw new Exception("can not process multiple Method call");

                                    arg.collectionQuery.method = call.methodName;
                                    return (true, ConvertFilter(arg, call.arguments[0]));
                                }

                            case nameof(String.IsNullOrEmpty):
                            case nameof(Enumerable.ElementAt):
                            case nameof(Enumerable.Contains):
                            case nameof(Enumerable.Any):
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
