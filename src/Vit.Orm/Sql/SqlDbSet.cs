using System;
using System.Collections.Generic;
using System.Linq;
using Vit.Linq.ExpressionTree.ComponentModel.CollectionsQuery;

using Vit.Linq.ExpressionTree.ComponentModel;
using Dapper;
using System.Linq.Expressions;
using System.Data;
using Vit.Orm.Entity;
using System.Reflection;
using Vit.Core.Module.Serialization;
using Vit.Linq;

namespace Vit.Orm.Sql
{
    public class SqlDbSetConstructor
    {
        public Type entityType;
        public DbContext dbContext;
        public ISqlTranslator sqlTranslator;
        public Func<IDbConnection> CreateDbConnection;
        public Func<Type, IEntityDescriptor> GetEntityDescriptor;


        public static IDbSet CreateDbSet<Entity>(SqlDbSetConstructor arg)
        {
            return new SqlDbSet<Entity>(arg.dbContext, arg.CreateDbConnection, arg.sqlTranslator, arg.GetEntityDescriptor(arg.entityType));
        }
        static MethodInfo _CreateDbSet = new Func<SqlDbSetConstructor, IDbSet>(CreateDbSet<object>)
                   .Method.GetGenericMethodDefinition();

        public IDbSet CreateDbSet()
        {
            return _CreateDbSet.MakeGenericMethod(entityType)
                     .Invoke(null, new[] { this }) as IDbSet;
        }
    }

    public class SqlDbSet<Entity> : Vit.Orm.DbSet<Entity>
    {
        ISqlTranslator sqlTranslator;
        DbContext dbContext;
        protected Func<IDbConnection> CreateDbConnection;

        IEntityDescriptor _entityDescriptor;
        public override IEntityDescriptor entityDescriptor => _entityDescriptor;

        public SqlDbSet(DbContext dbContext, Func<IDbConnection> CreateDbConnection, ISqlTranslator sqlTranslator, IEntityDescriptor entityDescriptor)
        {
            this.dbContext = dbContext;
            this.CreateDbConnection = CreateDbConnection;
            this.sqlTranslator = sqlTranslator;
            this._entityDescriptor = entityDescriptor;
        }

        public override void Create()
        {
            string sql = sqlTranslator.Create(entityDescriptor);

            using var connection = CreateDbConnection();
            connection.Execute(sql: sql);
        }

        public override IQueryable<Entity> Query()
        {
            Func<Expression, Type, object> QueryExecutor = (expression, type) =>
            {
                // #1 convert to ExpressionNode
                // (query) => query.Where().OrderBy().Skip().Take().Select().ToList();
                // (users) => users.SelectMany(
                //      user => users.Where(father => (father.id == user.fatherId)).DefaultIfEmpty(),
                //      (user, father) => new <>f__AnonymousType4`2(user = user, father = father)
                //  ).Where().Select();
                var isArgument = QueryableBuilder.QueryTypeNameCompare("SqlDbSet");
                ExpressionNode node = dbContext.convertService.ConvertToData(expression, autoReduce: true, isArgument: isArgument);
                var strNode = Json.Serialize(node);


                // #2 convert to JoinedStream
                // {select,left,joins,where,order,skip,take}
                var stream = StreamReader.ReadNode(node);
                var strStream = Json.Serialize(stream);

     

                // #3.1 BatchUpdate
                if (stream is StreamToUpdate streamToUpdate)
                {
                    var entityType = expression.Type.GetGenericArguments()?.FirstOrDefault();
                    return BatchUpdate(streamToUpdate, entityType);
                }


                // #3.3 Query
                // #3.3.1
                var joinedStream = stream as JoinedStream;
                if (joinedStream == null) joinedStream = new JoinedStream("tmp") { left = stream };

                // #3.3.2 execute and read result
                switch (joinedStream.method)
                {
                    case "ToSql":
                        {
                            // ToSql
                            var entityType = expression.Type.GetGenericArguments()?.FirstOrDefault();
                            (string sql, Dictionary<string, object> sqlParam, IDbDataReader dataReader) = sqlTranslator.Query(joinedStream, entityType);
                            return sql;
                        }
                    case "Count":
                        {
                            // Count
                            (string sql, Dictionary<string, object> sqlParam, IDbDataReader dataReader) = sqlTranslator.Query(joinedStream, entityType: null);

                            using var connection = CreateDbConnection();
                            var count = connection.ExecuteScalar(sql: sql, param: (object)sqlParam);
                            return count;
                        }
                    case "ToList":
                    case "":
                    case null:
                        {
                            // ToList
                            var entityType = expression.Type.GetGenericArguments()?.FirstOrDefault();
                            (string sql, Dictionary<string, object> sqlParam, IDbDataReader dataReader) = sqlTranslator.Query(joinedStream, entityType);

                            using var connection = CreateDbConnection();
                            using var reader = connection.ExecuteReader(sql: sql, param: (object)sqlParam);
                            return dataReader.ReadData(reader);
                        }
                }
                throw new NotSupportedException("not supported query type: " + joinedStream.method);
            };
            return QueryableBuilder.Build<Entity>(QueryExecutor, "SqlDbSet");
        }

        int BatchUpdate(StreamToUpdate streamToUpdate, Type entityType)
        {
            // Update
            (string sql, Dictionary<string, object> sqlParam) = sqlTranslator.ExecuteUpdate(streamToUpdate, entityType);

            using var connection = CreateDbConnection();
            return connection.Execute(sql: sql, param: (object)sqlParam);
        }


        public override Entity Insert(Entity entity)
        {
            (string sql, Dictionary<string, object> sqlParam ) = sqlTranslator.Insert(this, entity);

            // # execute and get key value
            using var connection = CreateDbConnection();
            var affectedRowCount = connection.Execute(sql: sql, param: (object)sqlParam);

            return affectedRowCount == 1 ? entity : default;
        }
   
        public override int Update(Entity entity)
        {
            (string sql, Dictionary<string, object> sqlParam) = sqlTranslator.Update(this, entity);

            // #3 execute
            using var connection = CreateDbConnection();
            var affectedRowCount = connection.Execute(sql: sql, param: (object)sqlParam);

            return affectedRowCount;
        }
     
        public override int Delete(Entity entity)
        {
            (string sql, Dictionary<string, object> sqlParam) = sqlTranslator.Delete(this, entity);

            // #3 execute
            using var connection = CreateDbConnection();
            var affectedRowCount = connection.Execute(sql: sql, param: (object)sqlParam);

            return affectedRowCount;
        }
        public override int DeleteByKey(object keyValue)
        {
            (string sql, Dictionary<string, object> sqlParam) = sqlTranslator.DeleteByKey(this, keyValue);

            // #3 execute
            using var connection = CreateDbConnection();
            var affectedRowCount = connection.Execute(sql: sql, param: (object)sqlParam);

            return affectedRowCount;
        }

    }
}
