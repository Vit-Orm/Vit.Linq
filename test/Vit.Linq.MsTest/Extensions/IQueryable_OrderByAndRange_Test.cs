using Vit.Extensions.Linq_Extensions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Vit.Linq.ComponentModel;

namespace Vit.Linq.MsTest.Extensions
{
    [TestClass]
    public class IQueryable_OrderByAndRange_Test
    {

        #region Test
        [TestMethod]
        public void Test()
        {
            var query = DataSource.GetIQueryable();

            #region #1 OrderBy Range
            {
                var result = query
                    .IQueryable_OrderBy(new[] {
                        new OrderField { field = "job.departmentId", asc = false },
                        new OrderField { field = "id", asc = true }
                    })
                    .IQueryable_Range(new PageInfo { pageSize = 10, pageIndex = 1 })
                    .IQueryable_ToList<Person>();

                Assert.AreEqual(10, result.Count);
                Assert.AreEqual(990, result[0].id);
            }
            #endregion


            #region #2 OrderBy Range
            {
                var result = query
                    .IQueryable_OrderBy("id", false)
                    .IQueryable_Range(skip: 10, take: 10)
                    .IQueryable_ToList<Person>();

                Assert.AreEqual(10, result.Count);
                Assert.AreEqual(989, result[0].id);
            }
            #endregion


            #region #3 ToRangeData
            {
                var result = query
                    .IQueryable_OrderBy(new[] {
                        new OrderField { field = "job.departmentId", asc = false },
                        new OrderField { field = "id", asc = true }
                    })
                    .IQueryable_ToRangeData<Person>(RangeInfo.FromPage(pageSize: 10, pageIndex: 1));

                Assert.AreEqual(1000, result.totalCount);
                Assert.AreEqual(10, result.items.Count);
                Assert.AreEqual(990, result.items[0].id);
            }
            #endregion


        }
        #endregion

    }
}
