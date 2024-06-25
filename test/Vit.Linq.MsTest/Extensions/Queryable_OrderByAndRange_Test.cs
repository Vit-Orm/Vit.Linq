using Vit.Extensions.Linq_Extensions;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Vit.Linq.ComponentModel;

namespace Vit.Linq.MsTest.Extensions
{
    [TestClass]
    public class Queryable_OrderByAndRange_Test
    {

        [TestMethod]
        public void Test()
        {
            var query = DataSource.GetQueryable();

            #region #1 OrderBy Range
            {
                var result = query
                    .OrderBy(new[] {
                        new OrderField { field = "job.departmentId", asc = false },
                        new OrderField { field = "id", asc = true }
                    })
                    .Range(new PageInfo { pageSize = 10, pageIndex = 1 })
                    .ToList();

                Assert.AreEqual(10, result.Count);
                Assert.AreEqual(990, result[0].id);
            }
            #endregion


            #region #2 OrderBy Range
            {
                var result = query
                    .OrderBy("id", false)
                    .Range(skip: 10, take: 10)
                    .ToList();

                Assert.AreEqual(10, result.Count);
                Assert.AreEqual(989, result[0].id);
            }
            #endregion


            #region #3 ToRangeData
            {
                var result = query
                    .OrderBy(new[] {
                        new OrderField { field = "job.departmentId", asc = false },
                        new OrderField { field = "id", asc = true }
                    })
                    .ToRangeData(new PageInfo { pageSize = 10, pageIndex = 1 }.ToRange());

                Assert.AreEqual(1000, result.totalCount);
                Assert.AreEqual(10, result.items.Count);
                Assert.AreEqual(990, result.items[0].id);
            }
            #endregion


        }


    }
}
