using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Vit.Orm.Entity.Dapper
{
    public class EntityDescriptor: IEntityDescriptor
    {
        static ConcurrentDictionary<Type, EntityDescriptor> descMap = new();
        static EntityDescriptor New(Type entityType) => new EntityDescriptor(entityType);
        public static EntityDescriptor GetEntityDescriptor(Type entityType)
        {
            if (entityType?.GetCustomAttribute<global::Dapper.Contrib.Extensions.TableAttribute>() == null) return null;
            //if (entityType == null) return null;
            return descMap.GetOrAdd(entityType, New);
        }

        public static EntityDescriptor GetEntityDescriptor<Entity>()
        {
            return GetEntityDescriptor(typeof(Entity));
        }

        EntityDescriptor(Type entityType)
        {
            tableName = entityType.GetCustomAttribute<global::Dapper.Contrib.Extensions.TableAttribute>()?.Name;

            var entityProperties = entityType?.GetProperties(BindingFlags.Public | BindingFlags.Instance);

            var keyProperty = entityProperties.FirstOrDefault(p => p.GetCustomAttribute<global::Dapper.Contrib.Extensions.KeyAttribute>() != null);
            this.key = new ColumnDescriptor(keyProperty, true);

            var properties = entityProperties.Where(p => p.GetCustomAttribute<global::Dapper.Contrib.Extensions.KeyAttribute>() == null);
            this.columns = properties.Select(p => new ColumnDescriptor(p, false)).ToArray();
        }

        public string tableName { get; private set; }

        /// <summary>
        /// primary key name
        /// </summary>
        public string keyName => key?.name;

        /// <summary>
        /// primary key
        /// </summary>
        public IColumnDescriptor key { get; private set; }

        /// <summary>
        /// not include primary key
        /// </summary>
        public IColumnDescriptor[] columns { get; private set; }


        public IEnumerable<IColumnDescriptor> allColumns => new[] { key }.Concat(columns);


    }
}
