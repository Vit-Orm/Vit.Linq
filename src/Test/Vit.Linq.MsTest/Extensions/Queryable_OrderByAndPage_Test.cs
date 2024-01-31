using Vit.Extensions.Linq_Extensions;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Vit.Linq.ComponentModel;

namespace Vit.Linq.MsTest.Extensions
{
    [TestClass]
    public class Queryable_OrderByAndPage_Test
    {

        #region Test
        [TestMethod]
        public void Test()
        {
            var query = DataSource.GetQueryable();

            #region #1
            {
                var result = query
                    .OrderBy(new[] {
                        new OrderField { field = "job.departmentId", asc = false },
                        new OrderField { field = "id", asc = true }
                    })
                    .Page(new PageInfo { pageIndex = 1, pageSize = 10 })
                    .ToList();
                Assert.AreEqual(result.Count, 10);
                Assert.AreEqual(result[0].id, 990);
            }
            #endregion


            #region #2
            {
                var result = query
                    .OrderBy("id", false)
                    .Page(2, 10)
                    .ToList();
                Assert.AreEqual(result.Count, 10);
                Assert.AreEqual(result[0].id, 989);
            }
            #endregion


            #region #3
            {
                var result = query
                    .OrderBy(new[] {
                        new OrderField { field = "job.departmentId", asc = false },
                        new OrderField { field = "id", asc = true }
                    })
                    .ToPageData(new PageInfo { pageIndex = 1, pageSize = 10 });

                Assert.AreEqual(result.totalCount, 1000);
                Assert.AreEqual(result.rows.Count, 10);
                Assert.AreEqual(result.rows[0].id, 990);
            }
            #endregion


        }
        #endregion

    }
}
