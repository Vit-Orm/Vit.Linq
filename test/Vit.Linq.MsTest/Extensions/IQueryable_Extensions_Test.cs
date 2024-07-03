using Vit.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace Vit.Linq.MsTest.Extensions
{
    [TestClass]
    public class IQueryable_Extensions_Test
    {

        [TestMethod]
        public void Test_ToList()
        {
            var query = DataSource.GetIQueryable();

            // IQueryable_Count
            {
                int count = query.IQueryable_Count();
                Assert.AreEqual(1000, count);
            }

            // IQueryable_Skip
            {
                int count = query.IQueryable_Skip(10).IQueryable_Count();
                Assert.AreEqual(1000 - 10, count);
            }

            // IQueryable_Take
            {
                int count = query.IQueryable_Take(10).IQueryable_Count();
                Assert.AreEqual(10, count);
            }

            // IQueryable_ToList
            {

                var list1 = query.IQueryable_ToList<Person>();
                Assert.AreEqual(1000, list1.Count);

                var list2 = query.IQueryable_ToList() as List<Person>;
                Assert.AreEqual(1000, list2.Count);
            }

            // IQueryable_ToArray
            {
                var array1 = query.IQueryable_ToArray<Person>();
                Assert.AreEqual(1000, array1.Length);

                var array2 = query.IQueryable_ToArray() as Person[];
                Assert.AreEqual(1000, array2.Length);
            }

            // IQueryable_FirstOrDefault
            {
                var person = query.IQueryable_FirstOrDefault<Person>();
                Assert.AreEqual(0, person.id);

                person = query.IQueryable_FirstOrDefault() as Person;
                Assert.AreEqual(0, person.id);
            }

            // IQueryable_First
            {
                var person = query.IQueryable_First<Person>();
                Assert.AreEqual(0, person.id);

                person = query.IQueryable_First() as Person;
                Assert.AreEqual(0, person.id);
            }

        }


    }
}
