using System.Linq;

namespace Vit.Extensions.Linq_Extensions
{

    public static partial class Queryable_Extensions
    {
        /// <summary>
        /// except null items:  source?.Where(x => x != null)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <returns></returns>
        public static IQueryable<T> NotNull<T>(this IQueryable<T> source) => source?.Where(x => x != null);
    }
}