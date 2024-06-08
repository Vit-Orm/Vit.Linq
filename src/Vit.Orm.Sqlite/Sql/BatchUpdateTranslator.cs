﻿using System;
using System.Collections.Generic;
using System.Linq;

using Vit.Linq.ExpressionTree.CollectionsQuery;
using Vit.Linq.ExpressionTree.ComponentModel;
using Vit.Orm.Entity;

namespace Vit.Orm.Sqlite.Sql
{
    public class BatchUpdateTranslator : BaseQueryTranslator
    {
        /*

-- multiple
WITH tmp AS (
    select   ('u' || u.id || '_' || COALESCE(father.id,'') ) as `_name` , u.id 
    from `User` u
    left join `User` father on u.fatherId = father.id 
    where u.id > 0
)
UPDATE `User`  
  SET `name` =  ( SELECT `_name` FROM tmp WHERE tmp.id =`User`.id )
where id in ( SELECT id FROM tmp );


--- single
UPDATE `User` SET `name` = 'u'||id  where id > 0;
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
            sql += $"UPDATE `{tableName}` ";

            var sqlToUpdateCols = columnsToUpdate.Select(m => m.name).Select(name => $"{NewLine}  SET `{name}` =  ( SELECT `_{name}` FROM tmp WHERE tmp.`{keyName}` =`{tableName}`.`{keyName}` )");
            sql += string.Join(",", sqlToUpdateCols);

            sql += $"{NewLine}where `{keyName}` in ( SELECT `{keyName}` FROM tmp ); {NewLine}";

            return sql;
        }


        List<MemberBind> columnsToUpdate;
        IEntityDescriptor entityDescriptor;

        public BatchUpdateTranslator(SqlTranslator sqlTranslator) : base(sqlTranslator)
        {
        }

        protected override string ReadSelect(CombinedStream stream)
        {
            var fieldsToUpdate = (stream as StreamToUpdate)?.fieldsToUpdate;

            columnsToUpdate = (fieldsToUpdate?.constructorArgs ?? new()).AsQueryable().Concat(fieldsToUpdate?.memberArgs ?? new()).ToList();
            if (columnsToUpdate?.Any() != true) throw new ArgumentException("can not get columns to update");


            var entityType = fieldsToUpdate.New_GetType();
            entityDescriptor = sqlTranslator.GetEntityDescriptor(entityType);
            if (entityDescriptor == null) throw new ArgumentException("Entity can not be updated");


            var sqlFields = new List<string>();

            foreach (var column in columnsToUpdate)
            {
                sqlFields.Add($"({ReadEval(column.value)}) as `_{column.name}`");
            }

            // primary key
            sqlFields.Add($"{sqlTranslator.GetSqlField(stream.source.alias, entityDescriptor.keyName)} as `{entityDescriptor.keyName}`");
            return String.Join(",", sqlFields);
        }



    }
}
