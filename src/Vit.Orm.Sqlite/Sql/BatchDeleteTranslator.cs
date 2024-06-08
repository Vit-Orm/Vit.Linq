using System;
using System.Collections.Generic;

using Vit.Linq.ExpressionTree.CollectionsQuery;
using Vit.Orm.Entity;

namespace Vit.Orm.Sqlite.Sql
{
    public class BatchDeleteTranslator : BaseQueryTranslator
    {
        /*
WITH tmp AS (
    select u.id 
    from `User` u
    left join `User` father on u.fatherId = father.id 
    where u.id > 0
)
delete from `User` where id in ( SELECT id FROM tmp );
         */
        public override string BuildQuery(CombinedStream stream)
        {
            var sqlInner = base.BuildQuery(stream);


            var NewLine = "\r\n";
            var keyName = entityDescriptor.keyName;
            var tableName = entityDescriptor.tableName;


            var sql = $"WITH tmp AS ( {NewLine}";
            sql += sqlInner;

            sql += $"{NewLine}){NewLine}";
            sql += $"delete from `{tableName}` ";

            sql += $"{NewLine}where `{keyName}` in ( SELECT `{keyName}` FROM tmp ); {NewLine}";

            return sql;
        }



        IEntityDescriptor entityDescriptor;

        public BatchDeleteTranslator(SqlTranslator sqlTranslator) : base(sqlTranslator)
        {
        }

        protected override string ReadSelect(CombinedStream stream)
        {
            var entityType = (stream.source as SourceStream)?.GetEntityType();
            entityDescriptor = sqlTranslator.GetEntityDescriptor(entityType);
            if (entityDescriptor == null) throw new ArgumentException("Entity can not be deleted");

            var sqlFields = new List<string>();

            // primary key
            sqlFields.Add($"{sqlTranslator.GetSqlField(stream.source.alias, entityDescriptor.keyName)} as `{entityDescriptor.keyName}`");
            return String.Join(",", sqlFields);
        }



    }
}
