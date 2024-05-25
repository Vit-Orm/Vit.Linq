using System;
using System.Collections.Generic;

using Vit.Linq.ExpressionTree.ComponentModel;
using Vit.Linq.ExpressionTree.ComponentModel.CollectionsQuery;
using Vit.Orm.Entity;
using Vit.Orm.Sql;

namespace Vit.Orm.Sqlite.Sql
{
    public class SqlTranslator : ISqlTranslator
    {

        public DbContext dbContext { get; private set; }

        public SqlTranslator(DbContext dbContext)
        {
            this.dbContext = dbContext;
        }


        public string Create(IEntityDescriptor entityDescriptor)
        {
            /* //sql
CREATE TABLE `user` (
  `id` int NOT NULL PRIMARY KEY,
  `name` varchar(100) DEFAULT NULL,
  `birth` date DEFAULT NULL,
  `fatherId` int DEFAULT NULL,
  `motherId` int DEFAULT NULL
) ;
              */
            List<string> sqlFields = new();

            // #1 primary key
            sqlFields.Add(GetColumnSql(entityDescriptor.key) + " PRIMARY KEY");

            // #2 columns
            if (entityDescriptor.columns != null)
            {
                foreach (var column in entityDescriptor.columns)
                {
                    sqlFields.Add(GetColumnSql(column));
                }
            }

            return $@"
CREATE TABLE `{entityDescriptor.tableName}` (
{string.Join(",\r\n", sqlFields)}
)";


            #region GetColumnSql
            string GetColumnSql(IColumnDescriptor column)
            {
                bool nullable = false;

                var type = column.type;
                if (type.IsGenericType)
                {
                    nullable = true;
                    type = type.GetGenericArguments()[0];
                }
                // `name` varchar(100) DEFAULT NULL
                return $"  `{column.name}` {GetDbType(type)} {(nullable ? "DEFAULT NULL" : "NOT NULL")}";
            }
            string GetDbType(Type type)
            {
                if (type == typeof(DateTime))
                    return "DATETIME";

                if (type == typeof(string))
                    return "TEXT";

                if (type == typeof(float) || type == typeof(double) || type == typeof(decimal))
                    return "REAL";

                if (type == typeof(bool) || type.Name.ToLower().Contains("int")) return "INTEGER";

                throw new NotSupportedException("unsupported column type:" + type.Name);
            }
            #endregion

        }


        public (string sql, Dictionary<string, object> sqlParam, IDbDataReader dataReader) Query(JoinedStream joinedStream, Type entityType)
        {
            var query = new QueryTranslator(this, entityType: entityType);
            string sql = query.BuildQuery(joinedStream);
            return (sql, query.sqlParam, query.dataReader);
        }

        public (string sql, Dictionary<string, object> sqlParam) ExecuteUpdate(JoinedStream joinedStream, Type entityType)
        {
            var query = new BatchUpdateTranslator(this, entityType: entityType);
            string sql = query.BuildQuery(joinedStream);
            return (sql, query.sqlParam);
        }

        public (string sql, Dictionary<string, object> sqlParam) Insert<Entity>(DbSet<Entity> dbSet, Entity entity)
        {
            /* //sql
             insert into user(name,birth,fatherId,motherId) values('','','');
             select seq from sqlite_sequence where name='user';
              */
            var entityDescriptor = dbSet.entityDescriptor;
            var sqlParam = new Dictionary<string, object>();

            #region columns 
            List<string> columnNames = new List<string>();
            List<string> valueParams = new List<string>();
            string columnName;
            object value;

            foreach (var column in entityDescriptor.allColumns)
            {
                columnName = column.name;
                value = column.Get(entity);
                //value ??= DBNull.Value;

                columnNames.Add($"`{columnName}`");
                valueParams.Add($"@{columnName}");
                sqlParam[columnName] = value;
            }
            #endregion

            // #2 build sql
            string sql = $@"insert into `{entityDescriptor.tableName}`({string.Join(",", columnNames)}) values({string.Join(",", valueParams)});";
            //sql+=$"select seq from sqlite_sequence where name = '{tableName}'; ";

            return (sql, sqlParam);
        }

        public (string sql, Dictionary<string, object> sqlParam) Update<Entity>(DbSet<Entity> dbSet, Entity entity)
        {
            /* //sql
                update user set name='' where id=7;
            */

            var entityDescriptor = dbSet.entityDescriptor;
            var sqlParam = new Dictionary<string, object>();

            // #1 columns
            List<string> columnsToUpdate = new List<string>();
            string columnName; object value;
            foreach (var column in entityDescriptor.columns)
            {
                columnName = column.name;
                value = column.Get(entity) ?? DBNull.Value;

                columnsToUpdate.Add($"`{columnName}`=@{columnName}");
                sqlParam[columnName] = value;
            }


            // #2 build sql
            string sql = $@"update `{entityDescriptor.tableName}` set {string.Join(",", columnsToUpdate)} where `{entityDescriptor.keyName}`=@{entityDescriptor.keyName};";
            sqlParam[entityDescriptor.keyName] = entityDescriptor.key.Get(entity);

            return (sql, sqlParam);
        }


        public (string sql, Dictionary<string, object> sqlParam) Delete<Entity>(DbSet<Entity> dbSet, Entity entity)
        {
            /* //sql
            delete from user where id=7;
            */
            var entityDescriptor = dbSet.entityDescriptor;
            var sqlParam = new Dictionary<string, object>();

            // #2 build sql
            string sql = $@"delete from `{entityDescriptor.tableName}` where `{entityDescriptor.keyName}`=@{entityDescriptor.keyName};";
            sqlParam[entityDescriptor.keyName] = entityDescriptor.key.Get(entity);

            return (sql, sqlParam);
        }
        public (string sql, Dictionary<string, object> sqlParam) DeleteByKey<Entity>(DbSet<Entity> dbSet, object keyValue)
        {
            /* //sql
            delete from user where id=7;
            */
            var entityDescriptor = dbSet.entityDescriptor;
            var sqlParam = new Dictionary<string, object>();

            // #2 build sql
            string sql = $@"delete from `{entityDescriptor.tableName}` where `{entityDescriptor.keyName}`=@{entityDescriptor.keyName};";
            sqlParam[entityDescriptor.keyName] = keyValue;

            return (sql, sqlParam);
        }

        public string GetTableName(Type entityType)
        {
            return dbContext.GetEntityDescriptor(entityType)?.tableName;
        }

        public string GetSqlField(string tableName, string columnName)
        {
            return $"`{tableName}`.`{columnName}`";
        }

        public string GetSqlField(ExpressionNode_Member member)
        {
            var memberName = member.memberName;
            if (string.IsNullOrWhiteSpace(memberName))
            {
                memberName = dbContext.GetEntityDescriptor(member.Member_GetType())?.keyName;
            }

            // 1: {"nodeType":"Member","parameterName":"a0","memberName":"id"}
            // 2: {"nodeType":"Member","objectValue":{"parameterName":"a0","nodeType":"Member"},"memberName":"id"}
            return GetSqlField(member.objectValue?.parameterName ?? member.parameterName, memberName);
        }

        public IEntityDescriptor GetEntityDescriptor(Type entityType) => dbContext.GetEntityDescriptor(entityType);

    }
}
