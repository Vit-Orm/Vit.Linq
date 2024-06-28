using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Vit.Extensions.Linq_Extensions
{
    /// <summary>
    ///  
    /// </summary>
    public static partial class IEnumerable_Extensions
    {

        public static void ForEach<T>(this IEnumerable<T> data, Action<T> action)
        {
            if (data != null)
                foreach (var item in data)
                {
                    action(item);
                }
        }

        public static async Task ForEachAsync<T>(this IEnumerable<T> data, Func<T, Task> action)
        {
            if (data != null)
                foreach (var item in data)
                {
                    await action(item);
                }
        }



        public static List<T> IEnumerator_ToList<T>(this IEnumerator data)
        {
            var list = new List<T>();
            data.Reset();
            while (data.MoveNext())
            {
                list.Add((T)data.Current);
            }
            return list;
        }


        public static List<T> IEnumerator_ToList<T>(this IEnumerator<T> data)
        {
            var list = new List<T>();
            data.Reset();
            while (data.MoveNext())
            {
                list.Add(data.Current);
            }
            return list;
        }


        public static List<T> IEnumerable_ToList<T>(this IEnumerable data) //ICollection data
        {
            return data?.GetEnumerator().IEnumerator_ToList<T>();
        }

        public static List<T> IEnumerable_ToList<T>(this IEnumerable<T> data) //ICollection data
        {
            return data?.GetEnumerator().IEnumerator_ToList();
        }


    }
}
