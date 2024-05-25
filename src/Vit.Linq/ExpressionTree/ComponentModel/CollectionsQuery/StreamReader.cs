using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace Vit.Linq.ExpressionTree.ComponentModel.CollectionsQuery
{

    class ExpressionNode_RenameableMember : ExpressionNode
    {
        private IStream stream;
        public override string parameterName
        {
            get => stream?.alias;
            set => throw new NotSupportedException();
        }
        public static ExpressionNode Member(IStream stream, string memberName, ExpressionNode_Member member)
        {
            var node = new ExpressionNode_RenameableMember
            {
                nodeType = NodeType.Member,
                stream = stream,
                memberName = memberName,
            };
            node.Member_SetType(member.Member_GetType());
            return node;
        }
    }

    class Argument
    {
        public Dictionary<string, IStream> streamAliasMap;

        public Argument WithAlias(IStream stream, string alias)
        {
            var arg = new Argument();

            var map = streamAliasMap ?? new Dictionary<string, IStream>();
            arg.streamAliasMap = map.ToDictionary(kv => kv.Key, kv => kv.Value);
            arg.streamAliasMap[alias] = stream;
            return arg;
        }
    }

    public class StreamReader
    {

        /// <summary>
        /// lambda:
        ///     (query,query2) => query.SelectMany(query2).Where().OrderBy().Skip().Take().Select().ToList();
        /// </summary>
        /// <param name="lambda"> </param>
        /// <returns> </returns>
        public static IStream ReadNode(ExpressionNode_Lambda lambda)
        {
            return new StreamReader().ReadFromNode(lambda);
        }


        /// <summary>
        /// lambda:
        ///     (query,query2) => query.SelectMany(query2).Where().OrderBy().Skip().Take().Select().ToList();
        /// </summary>
        /// <param name="lambda"> </param>
        /// <returns> </returns>
        public IStream ReadFromNode(ExpressionNode_Lambda lambda)
        {
            var arg = new Argument();
            return ReadStream(arg, lambda.body);
        }
        int aliasNameCount = 0;
        string NewAliasName()
        {
            return "t" + (aliasNameCount++);
        }

        // query.SelectMany(query2).Where().OrderBy().Skip().Take().Select()
        IStream ReadStream(Argument arg, ExpressionNode node)
        {
            switch (node.nodeType)
            {
                case NodeType.Member:
                    {
                        ExpressionNode_Member member = node;
                        var oriValue = member.Member_GetOriValue();
                        if (oriValue != null)
                            return new SourceStream(oriValue, NewAliasName());
                        break;
                    }
                case NodeType.Constant:
                    {
                        ExpressionNode_Constant constant = node;
                        var oriValue = constant.value;
                        return new SourceStream(oriValue, NewAliasName());
                    }
                case NodeType.MethodCall:
                    {
                        ExpressionNode_MethodCall call = node;
                        switch (call.methodName)
                        {
                            case "SelectMany":
                                {
                                    var left = ReadStream(arg, call.arguments[0]);
                                    ExpressionNode_Lambda rightSelector = call.arguments[1];
                                    ExpressionNode_Lambda resultSelector = call.arguments[2];
                                    return SelectMany(arg, left, rightSelector, resultSelector);
                                }
                            case "Where":
                                {
                                    var source = ReadStream(arg, call.arguments[0]);
                                    var predicateLambda = call.arguments[1] as ExpressionNode_Lambda;
                                    ExpressionNode where;

                                    if (source is JoinedStream joinedStream)
                                    {
                                        if (  joinedStream.orders == null
                                            && joinedStream.skip == null && joinedStream.take == null
                                            && joinedStream.select?.existCalculatedField != true)
                                        {
                                            where = ReadWhere(arg, joinedStream, predicateLambda);
                                            joinedStream.where = joinedStream.where == null ? where : ExpressionNode.And(left: joinedStream.where, right: where);
                                            return joinedStream;
                                        }
                                    }

                                    where = ReadWhere(arg, source, predicateLambda);
                                    joinedStream = new JoinedStream(NewAliasName()) { left = source };
                                    joinedStream.where = where;
                                    return joinedStream;
                                }
                            case "Select":
                                {
                                    var source = ReadStream(arg, call.arguments[0]);
                                    ExpressionNode_Lambda resultSelector = call.arguments[1];

                                    var select = ReadFieldSelect(resultSelector, source);

                                    if (source is JoinedStream joinedStream && joinedStream.select == null)
                                    {
                                        joinedStream.select = select;
                                    }
                                    else
                                    {
                                        source = new JoinedStream(NewAliasName()) { left = source, select = select };
                                    }
                                    return source;
                                }
                            case "ExecuteUpdate":
                                {
                                    var source = ReadStream(arg, call.arguments[0]);
                                    ExpressionNode_Lambda resultSelector = call.arguments[1];

                                    var select = ReadFieldSelect(resultSelector, source);

                                    return new StreamToUpdate(source) { fieldsToUpdate = select.fields };
                                }
                            case "Take":
                                {
                                    var source = ReadStream(arg, call.arguments[0]);
                                    JoinedStream joinedStream = source as JoinedStream;
                                    if (joinedStream == null)
                                    {
                                        joinedStream = new JoinedStream(NewAliasName()) { left = source };
                                    }

                                    joinedStream.take = (call.arguments[1] as ExpressionNode_Constant)?.value as int?;
                                    return joinedStream;
                                }
                            case "Skip":
                                {
                                    var source = ReadStream(arg, call.arguments[0]);
                                    JoinedStream joinedStream = source as JoinedStream;
                                    if (joinedStream == null)
                                    {
                                        joinedStream = new JoinedStream(NewAliasName()) { left = source };
                                    }

                                    joinedStream.skip = (call.arguments[1] as ExpressionNode_Constant)?.value as int?;
                                    return joinedStream;
                                }

                            case "OrderBy":
                            case "OrderByDescending":
                            case "ThenBy":
                            case "ThenByDescending":
                                {
                                    var source = ReadStream(arg, call.arguments[0]);
                                    JoinedStream joinedStream = source as JoinedStream;
                                    if (joinedStream == null)
                                    {
                                        joinedStream = new JoinedStream(NewAliasName()) { left = source };
                                    }


                                    var methodName = call.methodName;

                                    var memberField = ReadSortField(call.arguments[1], source);

                                    var orderParam = new SortField { member = memberField, asc = !methodName.EndsWith("Descending") };

                                    if (methodName.StartsWith("OrderBy"))
                                    {
                                        joinedStream.orders = new List<SortField>();
                                    }

                                    joinedStream.orders ??= new List<SortField>();

                                    joinedStream.orders.Add(orderParam);

                                    return joinedStream;
                                }
                            case "Count":
                            case "ToSql":
                                {
                                    if (call.arguments?.Length != 1) break;

                                    var source = ReadStream(arg, call.arguments[0]);
                                    JoinedStream joinedStream = source as JoinedStream;
                                    if (joinedStream == null)
                                    {
                                        joinedStream = new JoinedStream(NewAliasName()) { left = source };
                                    }

                                    joinedStream.method = call.methodName;
                                    return joinedStream;
                                }
                        }
                        throw new Exception("[CollectionStream] unexpected method call : " + call.methodName);
                    }
            }
            throw new NotSupportedException($"[CollectionStream] unexpected expression : {node.nodeType}");
        }


        // users.SelectMany(
        //      user => users.Where(father => (father.id == user.fatherId)).DefaultIfEmpty(),
        //      (user, father) => new <>f__AnonymousType4`2(user = user, father = father)
        //  )
        IStream SelectMany(Argument arg, IStream left, ExpressionNode_Lambda rightSelector, ExpressionNode_Lambda resultSelector)
        {
            // #1 right stream
            var right = ReadRightStream(arg, left, rightSelector);

            // #2 select
            // (user, father) => new <>f__AnonymousType4`2(user = user, father = father)
            var select = ReadFieldSelect(resultSelector, left, right.right);


            // #3 merge multiple join
            if (left is JoinedStream joinedStream)
            {
                if ( joinedStream.where == null && joinedStream.orders == null
                    && joinedStream.skip == null && joinedStream.take == null
                    && joinedStream.select?.existCalculatedField != true)
                {
                    joinedStream.joins ??= new List<StreamToJoin>();
                    joinedStream.joins.Add(right);
                    joinedStream.select = select;
                    return joinedStream;
                }
                throw new NotSupportedException($"[CollectionStream] not support inner select in join sentence");
            }

            var stream = new JoinedStream(NewAliasName());
            stream.left = left;
            stream.joins = new List<StreamToJoin> { right };
            stream.select = select;
            return stream;


            #region method for SelectMany

            // rightSelector:
            //      user => users.Where(father => (father.id == user.fatherId)).DefaultIfEmpty(),
            StreamToJoin ReadRightStream(Argument arg, IStream left, ExpressionNode_Lambda rightSelector)
            {
                string joinType = "innerJoin";
                IStream right = null;
                ExpressionNode on = null;

                ReadNode(arg.WithAlias(left, rightSelector.parameterNames[0]), rightSelector.body);
                var rightStream = new StreamToJoin();
                rightStream.joinType = joinType;
                rightStream.right = right;
                rightStream.on = on;

                return rightStream;

                void ReadNode(Argument arg, ExpressionNode node)
                {
                    if (node.nodeType != NodeType.MethodCall)
                        throw new NotSupportedException($"[CollectionStream] unexpected expression : {node.nodeType}");

                    ExpressionNode_MethodCall call = node;
                    switch (call.methodName)
                    {
                        case "Where":
                            {
                                if (on != null)
                                    throw new Exception("[CollectionStream] unexpected multiple where in join");

                                var source = ReadStream(arg, call.arguments[0]);
                                var predicateLambda = call.arguments[1] as ExpressionNode_Lambda;

                                right = source;

                                on = ReadWhere(arg, right, predicateLambda);
                                return;
                            }
                        case "DefaultIfEmpty":
                            {
                                joinType = "leftJoin";
                                var source = call.arguments[0];
                                ReadNode(arg, source);
                                return;
                            }
                    }
                    throw new Exception("[CollectionStream] unexpected method call : " + call.methodName);
                }

            }
            #endregion
        }


        // predicateLambda:          father => (father.id == user.fatherId)
        ExpressionNode ReadWhere(Argument arg, IStream source, ExpressionNode_Lambda predicateLambda)
        {
            arg = arg.WithAlias(source, predicateLambda.parameterNames[0]);
            ExpressionNode predicate = predicateLambda.body;
            var cloner = new ExpressionNodeCloner();
            cloner.clone = (node) =>
            {
                if (node?.nodeType == NodeType.Member)
                {
                    ExpressionNode_Member member = node;

                    if (!string.IsNullOrWhiteSpace(member.parameterName) && arg.streamAliasMap?.TryGetValue(member.parameterName, out var parameterValue) == true)
                    {
                        if (parameterValue is JoinedStream stream && stream.select?.TryGetField(node.memberName, out var sourceStream) == true)
                        {
                            return (true, (ExpressionNode)sourceStream);
                        }
                        return (true, ExpressionNode_RenameableMember.Member(parameterValue, node.memberName, node));
                    }
                }
                return default;
            };

            return cloner.Clone(predicate);
        }


        SelectedFields ReadFieldSelect(ExpressionNode_Lambda resultSelector, params IStream[] args)
        {
            ExpressionNode node = resultSelector.body;
            if (node?.nodeType != NodeType.New)
                throw new NotSupportedException($"[CollectionStream] unexpected expression : {node.nodeType}");

            #region #1 get args
            var parameters = new Dictionary<string, IStream>();
            for (var i = 0; i < args.Length; i++)
            {
                parameters[resultSelector.parameterNames[i]] = args[i];
            }
            #endregion

            // #2 cloner
            var cloner = new ExpressionNodeCloner();
            cloner.clone = (node) =>
            {
                if (node?.nodeType == NodeType.Member)
                {
                    ExpressionNode_Member member = node;

                    // {"nodeType":"Member", "parameterName":"a0", "memberName":"id"}
                    if (!string.IsNullOrWhiteSpace(member.parameterName) && parameters.TryGetValue(member.parameterName, out var parameterValue))
                    {
                        if (parameterValue is JoinedStream stream && stream.select?.TryGetField(node.memberName, out var sourceStream) == true)
                        {
                            return (true, sourceStream);
                        }
                        return (true, ExpressionNode_RenameableMember.Member(parameterValue, node.memberName, node));
                    }

                    // reduce level:  {"nodeType":"Member","objectValue":{"parameterName":"a0","nodeType":"Member"},"memberName":"id"}
                    if (member.objectValue?.nodeType == NodeType.Member && member.objectValue.memberName == null)
                    {
                        var objectValue = cloner.Clone(member.objectValue);

                        objectValue.memberName = member.memberName;
                        objectValue.Member_SetType(member.Member_GetType());
                        return (true, objectValue);
                    }
                }
                return default;
            };


            // #3
            bool? existCalculatedField = null;

            // root value of ExpressionNode_Member is IStream

            var fields = cloner.Clone(node) as ExpressionNode_New;

            if (existCalculatedField != true)
                existCalculatedField = fields.constructorArgs?.Exists(m => m?.value?.nodeType != NodeType.Member && m?.value?.nodeType != NodeType.New);

            if (existCalculatedField != true)
                existCalculatedField = fields.memberArgs?.Exists(m => m?.value?.nodeType != NodeType.Member && m?.value?.nodeType != NodeType.New);

            return new() { fields = fields, existCalculatedField = existCalculatedField };
        }



        ExpressionNode ReadSortField(ExpressionNode_Lambda resultSelector, params IStream[] args)
        {
            ExpressionNode node = resultSelector.body;
            if (node?.nodeType != NodeType.Member)
                throw new NotSupportedException($"[CollectionStream] unexpected expression : {node.nodeType}");

            #region #1 get args
            var parameters = new Dictionary<string, IStream>();
            for (var i = 0; i < args.Length; i++)
            {
                parameters[resultSelector.parameterNames[i]] = args[i];
            }
            #endregion

            // #2 cloner
            var cloner = new ExpressionNodeCloner();
            cloner.clone = (node) =>
            {
                if (node?.nodeType == NodeType.Member)
                {
                    ExpressionNode_Member member = node;

                    // {"nodeType":"Member", "parameterName":"a0", "memberName":"id"}
                    if (!string.IsNullOrWhiteSpace(member.parameterName) && parameters.TryGetValue(member.parameterName, out var parameterValue))
                    {
                        if (parameterValue is JoinedStream stream && stream.select?.TryGetField(node.memberName, out var sourceStream) == true)
                        {
                            return (true, sourceStream);
                        }
                        return (true, ExpressionNode_RenameableMember.Member(parameterValue, node.memberName, node));
                    }

                    // reduce level:  {"nodeType":"Member","objectValue":{"parameterName":"a0","nodeType":"Member"},"memberName":"id"}
                    if (member.objectValue?.nodeType == NodeType.Member && member.objectValue.memberName == null)
                    {
                        var objectValue = cloner.Clone(member.objectValue);

                        objectValue.memberName = member.memberName;
                        objectValue.Member_SetType(member.Member_GetType());
                        return (true, objectValue);
                    }
                }
                return default;
            };


            // #3
            var member = cloner.Clone(node);
            return member;
        }

    }
}
