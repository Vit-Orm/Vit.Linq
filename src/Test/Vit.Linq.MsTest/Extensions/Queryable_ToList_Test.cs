using Vit.Extensions.Linq_Extensions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;

namespace Vit.Linq.MsTest.Extensions
{
    [TestClass]
    public class Queryable_ToList_Test
    {


        [TestMethod]
        public void Test_ToList()
        {
            var query = DataSource.GetQueryable();

            #region Count ToList ToArray
            {

                int count = query.Count();
                Assert.AreEqual(1000, count);


                var list1 = query.ToList<Person>();
                Assert.AreEqual(1000, list1.Count);

                var list2 = query.ToList();
                Assert.AreEqual(1000, list2.Count);


                var array1 = query.ToArray<Person>();
                Assert.AreEqual(1000, array1.Length);

                var array2 = query.ToArray();
                Assert.AreEqual(1000, array2.Length);
            }
            #endregion
        }


    }
}
