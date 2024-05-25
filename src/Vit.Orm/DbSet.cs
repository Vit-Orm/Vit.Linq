using System.Linq;

using Vit.Orm.Entity;

namespace Vit.Orm
{
    public interface IDbSet
    {
        IEntityDescriptor entityDescriptor { get; }
    }

    public abstract class DbSet<Entity>: IDbSet
    {
        public abstract IEntityDescriptor entityDescriptor { get; }

        public abstract void Create();
        public abstract Entity Insert(Entity entity);
        public abstract int Update(Entity entity);

        public abstract int Delete(Entity entity);
        public abstract int DeleteByKey(object keyValue);
        public abstract IQueryable<Entity> Query();

    }
}
