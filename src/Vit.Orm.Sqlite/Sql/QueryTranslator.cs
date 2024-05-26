using System;
using System.Collections.Generic;
using System.Linq;
using Vit.Linq.ExpressionTree.ComponentModel;
using Vit.Orm.DataReader;
using Vit.Orm.Sql;
using Vit.Orm.Sql.DataReader;
using System.Linq.Expressions;
using Vit.Linq.ExpressionTree.ComponentModel.CollectionsQuery;

namespace Vit.Orm.Sqlite.Sql
{
    public class QueryTranslator
    {
        public SqlTranslator sqlTranslator { get; private set; }
        public Type entityType { get; private set; }
        public QueryTranslator(SqlTranslator sqlTranslator, Type entityType)
        {
            this.sqlTranslator = sqlTranslator;
            this.entityType = entityType;
        }



        public IDbDataReader dataReader;
        public Dictionary<string, object> sqlParam { get; private set; } = new Dictionary<string, object>();

        int paramIndex = 0;
        string NewParamName() => "param" + (paramIndex++);


        protected virtual string ReadSelect(JoinedStream stream)
        {
            if (stream.method == "Count")
            {
                var reader = new NumScalarReader();
                if (this.dataReader == null) this.dataReader = reader;
                return "count(*)";
            }
            else if (string.IsNullOrWhiteSpace(stream.method) || stream.method == "ToList")
            {
                var reader = new EntityReader();

                ExpressionNode selectedFields = stream.select?.fields as ExpressionNode;
                if (selectedFields == null)
                {
                    selectedFields = ExpressionNode.Member(parameterName: stream.left.alias, memberName: null).Member_SetType(entityType);
                }

                var sqlFields = reader.BuildSelect(entityType, sqlTranslator, sqlTranslator.dbContext.convertService, selectedFields);
                if (this.dataReader == null) this.dataReader = reader;
                return sqlFields;
            }
            else
            {
                throw new NotSupportedException("not supported method: " + stream.method);
            }
        }


        public virtual string BuildQuery(JoinedStream stream)
        {
            /* //sql
            select u.id, u.name, u.birth ,u.fatherId ,u.motherId,    father.name,  mother.name
            from `User` u
            inner join `User` father on u.fatherId = father.id 
            left join `User` mother on u.motherId = mother.id
            where u.id > 1
            limit 1,5;
             */

            string sql = "";

            // #0  select
            sql += "select " + ReadSelect(stream);


            #region #1 source
            // from `User` u
            sql += "\r\n from " + ReadInnerTable(stream.left);
            #endregion

            #region #2 join
            stream.joins?.ForEach(joinedStream =>
            {
                sql += "\r\n " + (joinedStream.joinType == "leftJoin" ? "left join" : "inner join");
                sql += " " + ReadInnerTable(joinedStream.right);

                var where = ReadWhere(joinedStream.on);
                if (!string.IsNullOrWhiteSpace(where)) sql += " on " + where;
            });

            #endregion

            // #3 where 1=1
            if (stream.where != null)
            {
                var where = ReadWhere(stream.where);
                if (!string.IsNullOrWhiteSpace(where)) sql += "\r\n where " + where;
            }

            // #4 OrderBy
            if (stream.orders?.Any() == true)
            {
                var fields = stream.orders.Select(field => (sqlTranslator.GetSqlField(field.member) + " " + (field.asc ? "asc" : "desc"))).ToList();
                sql += "\r\n order by " + String.Join(",", fields);
            }

            // #5 limit 1000,10       limit {skip},{take}   |     limit {take}
            if (stream.take != null || stream.skip != null)
            {
                if (stream.skip == null)
                {
                    sql += "\r\n limit " + stream.take;
                }
                else
                {
                    sql += "\r\n limit " + stream.skip + "," + (stream.take ?? 100000000);
                }
            }

            return sql;
        }

        string ReadInnerTable(IStream stream)
        {
            if (stream is SourceStream sourceStream)
            {
                IQueryable query = sourceStream.GetSource() as IQueryable;
                var tableName = sqlTranslator.GetTableName(query.ElementType);
                return $"`{tableName}` as " + stream.alias;
            }
            if (stream is JoinedStream combinedStream)
            {
                var innerQuery = BuildQuery(combinedStream);
                return $"({innerQuery}) as " + stream.alias;
            }
            throw new NotSupportedException();
        }


       protected string ReadValue(ExpressionNode data)
       {
            switch (data.nodeType)
            {
                case NodeType.Member:
                    return sqlTranslator.GetSqlField(data);

                case NodeType.Constant:
                    ExpressionNode_Constant constant = data;
                    var paramName = NewParamName();
                    sqlParam[paramName] = constant.value;
                    return "@" + paramName;

                case NodeType.Convert:
                    {
                        // cast( 4.1 as signed)

                        ExpressionNode_Convert convert = data;

                        Type targetType = convert.valueType?.ToType();

                        if (targetType == typeof(object)) return ReadValue(convert.body);

                        // Nullable
                        if (targetType.IsGenericType) targetType = targetType.GetGenericArguments()[0];

                        string targetDbType = GetDbType(targetType);

                        var sourceType = convert.body.Member_GetType();
                        if (sourceType != null)
                        {
                            if (sourceType.IsGenericType) sourceType = sourceType.GetGenericArguments()[0];

                            if (targetDbType == GetDbType(sourceType)) return ReadValue(convert.body);
                        }

                        if (targetDbType == "datetime")
                        {
                            return $"DATETIME({ReadValue(convert.body)})";
                        }
                        return $"cast({ReadValue(convert.body)} as {targetDbType})";

                        #region GetDbType
                        string GetDbType(Type type)
                        {
                            if (type == typeof(DateTime))
                                return "datetime";

                            if (type == typeof(string))
                                return "text";

                            if (type == typeof(float) || type == typeof(double) || type == typeof(decimal))
                                return "numeric";

                            if (type == typeof(bool) || type.Name.ToLower().Contains("int")) return "integer";

                            throw new NotSupportedException("unsupported column type:" + type.Name);
                        }
                        #endregion
                    }
                case nameof(ExpressionType.Add):
                    {
                        ExpressionNode_Binary binary = data;

                        // ##1 String Add
                        if (data.valueType?.ToType() == typeof(string))
                        {
                            return $"{ReadValue(binary.left)} || {ReadValue(binary.right)}";
                        }

                        // ##2 Numberic Add
                        return $"{ReadValue(binary.left)} + {ReadValue(binary.right)}";
                    }
                case nameof(ExpressionType.Coalesce):
                    {
                        ExpressionNode_Binary binary = data;
                        return $"COALESCE({ReadValue(binary.left)},{ReadValue(binary.right)})";
                    }
                case NodeType.MethodCall:
                    {
                        ExpressionNode_MethodCall call = data;
                        switch (call.methodName)
                        {
                            case "ToString":
                                {
                                    return $"cast({ReadValue(call.@object)} as text)";
                                }
                        }
                        throw new NotSupportedException("unsupported method call: " + data.methodName);
                    }
            }
            throw new NotSupportedException(data.nodeType);
        }

        string ReadWhere(ExpressionNode data)
        {
            switch (data.nodeType)
            {
                //case NodeType.Member:
                case NodeType.Constant:
                case NodeType.Convert:
                    return ReadValue(data);

                case NodeType.And:
                    ExpressionNode_And and = data;
                    return $"({ReadWhere(and.left)}) and ({ReadWhere(and.right)})";

                case NodeType.Or:
                    ExpressionNode_Or or = data;
                    return $"({ReadWhere(or.left)}) or ({ReadWhere(or.right)})";

                case NodeType.Not:
                    ExpressionNode_Not not = data;
                    return $"not ({ReadWhere(not.body)})";

                case NodeType.ArrayIndex:
                    throw new NotSupportedException(data.nodeType);
                //ExpressionNode_ArrayIndex arrayIndex = data;
                //return Expression.ArrayIndex(ToExpression(arg, arrayIndex.left), ToExpression(arg, arrayIndex.right));
                case NodeType.Equal:
                case NodeType.NotEqual:
                    {
                        ExpressionNode_Binary binary = data;

                        //   "= null"  ->   "is null" ,    "!=null" -> "is not null"   
                        if (binary.right.nodeType == NodeType.Constant && binary.right.value == null)
                        {
                            var opera = data.nodeType == NodeType.Equal ? "is null" : "is not null";
                            return $"{ReadValue(binary.left)} " + opera;
                        }
                        else if (binary.left.nodeType == NodeType.Constant && binary.left.value == null)
                        {
                            var opera = data.nodeType == NodeType.Equal ? "is null" : "is not null";
                            return $"{ReadValue(binary.right)} " + opera;
                        }

                        var @operator = operatorMap[data.nodeType];
                        return $"{ReadValue(binary.left)} {@operator} {ReadValue(binary.right)}";
                    }
                case NodeType.LessThan:
                case NodeType.LessThanOrEqual:
                case NodeType.GreaterThan:
                case NodeType.GreaterThanOrEqual:
                    {
                        ExpressionNode_Binary binary = data;
                        var @operator = operatorMap[data.nodeType];
                        return $"{ReadValue(binary.left)} {@operator} {ReadValue(binary.right)}";
                    }
                case NodeType.MethodCall:
                    throw new NotSupportedException(data.nodeType);
                    //ExpressionNode_MethodCall call = data;
                    //return ConvertMethodToExpression(arg, call);
            }

            throw new NotSupportedException(data.nodeType);
        }





        readonly static Dictionary<string, string> operatorMap = new Dictionary<string, string>
        {
            [NodeType.Equal] = "=",
            [NodeType.NotEqual] = "!=",
            [NodeType.LessThan] = "<",
            [NodeType.LessThanOrEqual] = "<=",
            [NodeType.GreaterThan] = ">",
            [NodeType.GreaterThanOrEqual] = ">=",
        };

    }

}
