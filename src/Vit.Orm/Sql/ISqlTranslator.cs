using System;
using System.Collections.Generic;

using Vit.Linq.ExpressionTree.ComponentModel;
using Vit.Linq.ExpressionTree.ComponentModel.CollectionsQuery;
using Vit.Orm.Entity;

namespace Vit.Orm.Sql
{
    public interface ISqlTranslator
    {
        (string sql, Dictionary<string, object> sqlParam, IDbDataReader dataReader) Query(JoinedStream joinedStream, Type entityType);
        (string sql, Dictionary<string, object> sqlParam) ExecuteUpdate(JoinedStream joinedStream, Type entityType);

        string Create(IEntityDescriptor entityDescriptor);

        (string sql, Dictionary<string, object> sqlParam) Insert<Entity>(DbSet<Entity> dbSet, Entity entity);
        (string sql, Dictionary<string, object> sqlParam) Update<Entity>(DbSet<Entity> dbSet, Entity entity);

        (string sql, Dictionary<string, object> sqlParam) Delete<Entity>(DbSet<Entity> dbSet, Entity entity);

        (string sql, Dictionary<string, object> sqlParam) DeleteByKey<Entity>(DbSet<Entity> dbSet, object keyValue);


        string GetTableName(Type entityType);
        string GetSqlField(string tableName, string columnName);
        string GetSqlField(ExpressionNode_Member member);

        IEntityDescriptor GetEntityDescriptor(Type entityType);
    }
}
