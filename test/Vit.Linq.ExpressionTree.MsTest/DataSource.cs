using System;
using System.Collections.Generic;
using System.Linq;

namespace Vit.Linq.ExpressionTree.MsTest
{



    public class DataSource
    {
        public static List<Person> BuildDataSource(int count = 1000)
        {
            var Now = DateTime.Now;
            var list = new List<Person>(count);
            for (int i = 0; i < count; i++)
            {
                list.Add(new Person
                {
                    id = i,
                    departmentId = i / 10,
                    name = "name" + i,
                    addTime = Now.AddSeconds(i),
                    ext = "ext" + i,
                    isEven = i % 2 == 0
                }.PopulateJob());

            }
            return list;
        }

        public static IQueryable GetIQueryable() => GetQueryable();
        public static IQueryable<Person> GetQueryable() => BuildDataSource().AsQueryable();
    }
}
