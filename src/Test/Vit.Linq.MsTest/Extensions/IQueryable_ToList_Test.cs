using Vit.Extensions.Linq_Extensions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace Vit.Linq.MsTest.Extensions
{
    [TestClass]
    public class IQueryable_ToList_Test
    {

        [TestMethod]
        public void Test_ToList()
        {
            var query = DataSource.GetIQueryable();

            #region Count ToList ToArray
            {

                int count = query.IQueryable_Count();
                Assert.AreEqual(1000, count);


                var list1 = query.IQueryable_ToList<ModelA>();
                Assert.AreEqual(1000, list1.Count);

                var list2 = query.IQueryable_ToList() as List<ModelA>;
                Assert.AreEqual(1000, list2.Count);


                var array1 = query.IQueryable_ToArray<ModelA>();
                Assert.AreEqual(1000, array1.Length);

                var array2 = query.IQueryable_ToArray() as ModelA[];
                Assert.AreEqual(1000, array2.Length);
            }
            #endregion
        }


    }
}
