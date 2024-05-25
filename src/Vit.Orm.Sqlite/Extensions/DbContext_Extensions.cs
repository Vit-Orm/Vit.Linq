using System;
using System.Data;

using Vit.Orm.Entity;
using Vit.Orm.Entity.Dapper;
using Vit.Orm.Sql;
using Vit.Orm.Sqlite.Sql;

namespace Vit.Orm.Sqlite.Extensions
{
    public static class DbContext_Extensions
    {
        public static DbContext UseSqlite(this DbContext dbContext, string ConnectionString)
        {
            ISqlTranslator sqlTranslator = new SqlTranslator(dbContext);

            Func<IDbConnection> CreateDbConnection = () => new Microsoft.Data.Sqlite.SqliteConnection(ConnectionString);

            Func<Type, IEntityDescriptor> GetEntityDescriptor = (type) => EntityDescriptor.GetEntityDescriptor(type);

            Func<Type, IDbSet> DbSetCreator =
                (type) => new SqlDbSetConstructor
                {
                    entityType = type,
                    dbContext = dbContext,
                    sqlTranslator = sqlTranslator,
                    CreateDbConnection = CreateDbConnection,
                    GetEntityDescriptor = GetEntityDescriptor
                }
                .CreateDbSet();

            dbContext.DbSetCreator = DbSetCreator;

            return dbContext;
        }

        

    }
}
