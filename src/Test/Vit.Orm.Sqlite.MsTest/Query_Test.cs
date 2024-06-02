
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

            if(1==2)
            {
                var users = dbContext.Query<User>();
                {
                    var userList = users.ToList();

                    Assert.AreEqual(6, userList.Count);
                    Assert.AreEqual(1, userList.First().id);
                    Assert.AreEqual(6, userList.Last().id);
                }
            }

            {
                var users = dbContext.Query<User>();
                {
                    var userList = users.Where(u => u.id > 2).Where(m => m.id < 4).ToList();
                    Assert.AreEqual(1, userList.Count);
                    Assert.AreEqual(3, userList.First().id);
                }
                {
                    var userList = users.Where(u => u.id + 1 == 4).Where(m => m.fatherId == 5).ToList();
                    Assert.AreEqual(3, userList.First().id);
                }
                {
                    var userList = users.Where(u => 4 == u.id + 1).Where(m => m.fatherId == 5).ToList();
                    Assert.AreEqual(3, userList.First().id);
                }


                {
                    var userList = users.Where(u => u.birth == new DateTime(2021, 01, 01, 03, 00, 00)).ToList();
                    Assert.AreEqual(1, userList.Count);
                    Assert.AreEqual(3, userList.First().id);
                }
                {
                    var userList = users.Where(u => u.birth == DateTime.Parse("2021-01-01 01:00:00").AddHours(2)).ToList();
                    Assert.AreEqual(1, userList.Count);
                    Assert.AreEqual(3, userList.First().id);
                }
            }

        }

    }
}
