
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Vit.Extensions.Linq_Extensions;
using System.Data;
using Vit.Orm;
using Vit.Orm.Sqlite.MsTest;

namespace Vit.Linq.MsTest.Converter
{

    [TestClass]
    public class Query_Test
    {
        DbContext GetDbContext() => TestData.BuildInitedDatabase(GetType().Name);

        [TestMethod]
        public void Test_SimpleQuery()
        {
            var dbContext = GetDbContext();

            if (1 == 1)
            {
                var users = dbContext.Query<User>();
                {
                    var userList = users.ToList();
                    Assert.AreEqual(6, userList.Count);
                }
            }

            if (1 == 1)
            {
                var users = dbContext.Query<User>();
                {
                    var userList = users.Where(u => u.id > 2).Where(m => m.fatherId == 5).ToList();
                    Assert.AreEqual(3, userList[0].id);
                }
                {
                    var userList = users.Where(u => u.id + 1 == 4).Where(m => m.fatherId == 5).ToList();
                    Assert.AreEqual(3, userList[0].id);
                }
                {
                    var userList = users.Where(u => 4 == u.id + 1).Where(m => m.fatherId == 5).ToList();
                    Assert.AreEqual(3, userList[0].id);
                }

                {
                    var userList = users.Where(u => u.birth >= new DateTime(2021, 01, 03)).ToList();
                    Assert.AreEqual(3, userList.Count);
                }
            }

        }
 
    }
}
