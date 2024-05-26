using System;
using System.Data;
using System.Collections.Generic;

namespace Vit.Orm.Sql.DataReader
{

    class SqlFieldReader
    {
        public int sqlFieldIndex { get; set; }
        protected Type valueType { get; set; }
        protected Type underlyingType;


        public SqlFieldReader(List<string> sqlFields, Type valueType, string sqlFieldName)
        {
            this.valueType = valueType;
            underlyingType = GetUnderlyingType(valueType);

            sqlFieldIndex = sqlFields.IndexOf(sqlFieldName);
            if (sqlFieldIndex < 0)
            {
                sqlFieldIndex = sqlFields.Count;
                sqlFields.Add(sqlFieldName);
            }
        }



        public object Read(IDataReader reader)
        {
            var value = reader.GetValue(sqlFieldIndex);
            if (value == null || value == DBNull.Value) return null;

            if (!underlyingType.IsInstanceOfType(value))
                value = Convert.ChangeType(value, underlyingType);
            return value;
        }

        static Type GetUnderlyingType(Type type)
        {
            if (type.IsGenericType && typeof(Nullable<>) == type.GetGenericTypeDefinition())
            {
                return type.GetGenericArguments()[0];
            }
            return type;
        }
    }



}
