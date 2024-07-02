using Vit.Linq;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Vit.Linq.ComponentModel;

namespace Vit.Linq.MsTest.Extensions
{
    [TestClass]
    public class Queryable_OrderBy_Reflection_Test
    {

        [TestMethod]
        public void Test()
        {
            var query = DataSource.GetQueryable();

            #region #1 OrderBy_Reflection
            {
                var result = query
                    .OrderByReflection(new[] {
                        new OrderField { field = "job.departmentId", asc = false },
                        new OrderField { field = "id", asc = true }
                    })
                    .Page(new PageInfo { pageSize = 10, pageIndex = 1 })
                    .ToList();

                Assert.AreEqual(10,result.Count);
                Assert.AreEqual(990, result[0].id);
            }
            #endregion


            #region #2 OrderBy_Reflection
            {
                var result = query
                    .OrderByReflection("id", false)
                    .Page(pageSize: 10, pageIndex: 2)
                    .ToList();

                Assert.AreEqual(10, result.Count);
                Assert.AreEqual(989, result[0].id);
            }
            #endregion


            #region #3 ToPageData
            {
                var result = query
                    .OrderByReflection(new[] {
                        new OrderField { field = "job.departmentId", asc = false },
                        new OrderField { field = "id", asc = true }
                    })
                    .ToPageData(new PageInfo { pageSize = 10, pageIndex = 1 });

                Assert.AreEqual(1000, result.totalCount);
                Assert.AreEqual(10, result.items.Count);
                Assert.AreEqual(990, result.items[0].id);
            }
            #endregion


        }

    }
}
