using Dapper.Contrib.Extensions;

using Vit.Orm.Sqlite.Extensions;

namespace Vit.Orm.Sqlite.MsTest
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


    public class TestData
    {

        public static DbContext BuildInitedDatabase(string dbName)
        {
            //"data source=T:\\sample\\sers\\sqlite.db"
            var filePath = Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, $"{dbName}.db");

            if (File.Exists(filePath)) File.Delete(filePath);
            File.WriteAllBytes(filePath, new byte[0]);

            var dbContext = new DbContext();
            dbContext.UseSqlite($"data source={filePath}");

            var dbSet = dbContext.DbSet<User>();
            dbSet.Create();

            var users = new List<User> {
                    new User {id=1,name="u1",  fatherId=5,motherId=6 },
                    new User {id=2,name="u2",fatherId=5,motherId=6 },
                    new User {id=3,name="u3",fatherId=5,motherId=6 },
                    new User {id=4,name="u4" },
                    new User {id=5,name="u5" },
                    new User {id=6,name="u6"},

                };
            users.ForEach(user => { user.birth = DateTime.Parse("2021-01-01 00:00:00").AddHours(user.id); });

            users.ForEach(user => dbSet.Insert(user));

            return dbContext;
        }

    }
}
