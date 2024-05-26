using System;
using System.Collections.Concurrent;
using System.Linq;

using Vit.Linq.ExpressionTree;
using Vit.Orm.Entity;

namespace Vit.Orm
{
    public class DbContext
    {
        public DbContext() { }

        public virtual ExpressionConvertService convertService => ExpressionConvertService.Instance;

        public Func<Type, IDbSet> DbSetCreator { set; protected get; }

        ConcurrentDictionary<Type, IDbSet> dbSetMap = new();

        public virtual IDbSet DbSet(Type entityType)
        {
            return dbSetMap.GetOrAdd(entityType, DbSetCreator);
        }
        public virtual DbSet<Entity> DbSet<Entity>()
        {
            return DbSet(typeof(Entity)) as DbSet<Entity>;
        }


        public virtual IEntityDescriptor GetEntityDescriptor(Type entityType) => DbSet(entityType)?.entityDescriptor;



        public virtual Entity Insert<Entity>(Entity entity) => DbSet<Entity>().Insert(entity);
        public virtual int Update<Entity>(Entity entity) => DbSet<Entity>().Update(entity);
        public virtual int Delete<Entity>(Entity entity) => DbSet<Entity>().Delete(entity);
        public virtual int DeleteByKey<Entity>(object keyValue) => DbSet<Entity>().DeleteByKey(keyValue);
        public virtual IQueryable<Entity> Query<Entity>() => DbSet<Entity>().Query();
    }
}
