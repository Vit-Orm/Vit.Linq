
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Vit.Extensions.Linq_Extensions;
using System.Data;
using Vit.Orm;
using Dapper.Contrib.Extensions;
using Vit.Orm.Sqlite.Extensions;

namespace Vit.Linq.MsTest.Converter
{






    [Table("User")]
    public class User
    {
        [Key]
        //[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }
        public string name { get; set; }
        public DateTime? birth { get; set; }

        public int? fatherId { get; set; }
        public int? motherId { get; set; }
    }



    [TestClass]
    public class Queryable_Test
    {
        static DbContext GetDbContext()
        {
            var dbContext = new DbContext();
            dbContext.UseSqlite("data source=T:\\sample\\sers\\sqlite.db");

            return dbContext;
        }


        [TestMethod]
        public void Test_Insert()
        {
            var dbContext = GetDbContext();

            #region ## Insert
            if (1 == 1)
            {
                var user = new User { id = 7, name = "testUser", birth = DateTime.Now, fatherId = 1, motherId = 2 };

                dbContext.Insert(user);

                Assert.IsTrue(user.id > 0);
            }
            #endregion

        }

        [TestMethod]
        public void Test_Update()
        {
            var dbContext = GetDbContext();

            #region ## Test_Update
            if (1 == 1)
            {
                var user = new User { id = 7, name = "testUser2", birth = DateTime.Now, fatherId = 1, motherId = 2 };

                var rowCount=dbContext.Update(user);

                Assert.AreEqual(1, rowCount);
            }
            #endregion
        }

        [TestMethod]
        public void Test_Delete()
        {
            var dbContext = GetDbContext();

            if (1 == 1)
            {
                var user = new User { id = 7, name = "testUser2", birth = DateTime.Now, fatherId = 1, motherId = 2 };

                var rowCount = dbContext.Delete(user);

                Assert.AreEqual(1, rowCount);
            }
            if (1 == 1)
            {
                var rowCount = dbContext.DeleteByKey<User>(7);
                Assert.AreEqual(0, rowCount);
            }
        
        }



        [TestMethod]
        public void Test_Query()
        {

            var dbContext = GetDbContext();


            #region ## Count
            if (1 == 2)
            {
                var users = dbContext.Query<User>();

                var count = (from user in users
                             from father in users.Where(father => user.fatherId == father.id).DefaultIfEmpty()
                             where user.id > 2 && father == null
                             select new
                             {
                                 father
                             }).Count();

                Assert.AreEqual(3, count);
            }
            #endregion


            #region ##  OrderBy Skip/Take ToSql
            if (1 == 1)
            {
                var users = dbContext.Query<User>();

                var query = (from user in users
                             from father in users.Where(father => user.fatherId == father.id).DefaultIfEmpty()
                             where user.id > 2
                             orderby father.id, user.id descending
                             select new
                             {
                                 user,
                                 father
                             })
                            .Skip(1).Take(2);

                var sql = query.ToSql();
                var list = query.ToList();

                Assert.AreEqual(2, list.Count);
                Assert.AreEqual(5, list[0].user.id);
                Assert.AreEqual(4, list[1].user.id);
            }
            #endregion
           

            #region #1 Join
            // worked
            if (1 == 1)
            {
                var users = dbContext.Query<User>();

                var userList = (from user in users
                                from father in users.Where(father => user.fatherId == father.id).DefaultIfEmpty()
                                where user.id > 2 && father == null
                                select new
                                {
                                    userId = user.id + 100,
                                    user,
                                    father,
                                    hasFather = father != null,
                                    fatherName = father.name,
                                }).ToList();
            }
            if (1 == 2)
            {
                var users = dbContext.Query<User>();
                var userList = (from user in users
                                from father in users.Where(father => user.fatherId == father.id).DefaultIfEmpty()
                                from mother in users.Where(mother => user.motherId == mother.id).DefaultIfEmpty()
                                select new
                                {
                                    userId = user.id + 100,
                                    hasFather = user.fatherId != null ? true : false,
                                    fatherName = father.name,
                                    motherName = mother.name,
                                }).ToList();

            }
            if (1 == 2)
            {
                var users = dbContext.Query<User>();
                var userList = (from user in users
                                    from father in users.Where(father => user.fatherId == father.id).DefaultIfEmpty()
                                    from mother in users.Where(mother => user.motherId == mother.id).DefaultIfEmpty()
                                select new
                                {
                                    uniqueId = user.id + "_" + father.id + "_" + mother.id,
                                    uniqueId1 = user.id + "_" + user.fatherId + "_" + user.motherId,
                                    user,
                                    user2 = user,
                                    user3 = user,
                                    father,
                                    hasFather = user.fatherId != null ? true : false,
                                    fatherName = father.name,
                                    mother
                                }).ToList();
            }

            if (1 == 2)
            {
                var users = dbContext.Query<User>();
                var userList = (from user in users
                                select new
                                {
                                    uniqueId1 = user.id + "_" + user.fatherId + "_" + user.motherId,
                                    uniqueId2 = $"{ user.id }_{ user.fatherId }_{ user.motherId }"
                                }).ToList();
            }

            #endregion

           



        }

        [TestMethod]
        public void Test_ExecuteUpdate()
        {

            var dbContext = GetDbContext();


            if (1 == 1)
            {
                var users = dbContext.Query<User>();

                var count = users.ExecuteUpdate(row => new User
                {
                    name = "u_" + row.id + "_" + (row.fatherId.ToString() ?? "") + "_" + (row.motherId.ToString() ?? "")
                });

                Assert.AreEqual(6, count);
            }

            if (1 == 3)
            {
                var users = dbContext.Query<User>();

                var query = (from user in users
                             from father in users.Where(father => user.fatherId == father.id).DefaultIfEmpty()
                             select new
                             {
                                 user,
                                 father
                             });

                var count = query.ExecuteUpdate(row => new User
                {
                    name = "u_" + row.user.id + "_" + (row.father.id.ToString() ?? "") + "|" + (row.user.name ?? "")
                });

                Assert.AreEqual(3, count);
            }



            if (1 == 2)
            {
                var users = dbContext.Query<User>();

                var query = (from user in users
                             from father in users.Where(father => user.fatherId == father.id).DefaultIfEmpty()
                             where user.id <= 5 && father != null
                             select new
                             {
                                 user,
                                 father
                             });

                var count = query.ExecuteUpdate(row => new User { name = row.father.name });

                Assert.AreEqual(3, count);
            }


        }
    }
}
