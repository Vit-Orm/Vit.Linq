using System.Collections.Generic;
using System.Linq;

namespace Vit.Extensions.Linq_Extensions
{

    public static partial class IEnumerable_Extensions
    {
        /// <summary>
        /// except null items:  source?.Where(x => x != null)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <returns></returns>
        public static IEnumerable<T> NotNull<T>(this IEnumerable<T> source) => source?.Where(x => x != null);
    }
}