using Vit.Linq;

namespace Vit.Linq.ExpressionTree.ExpressionTreeTest
{
    public abstract partial class ExpressionTester
    {
        public class User
        {
            public User() { }
            public User(string name) { this.name = name; }
            public User(int id) { this.id = id; }
            public int id { get; set; }
            public string name { get; set; }
            public DateTime? birth { get; set; }

            public int? fatherId { get; set; }
            public int? motherId { get; set; }
            public User[] childrenArray { get; set; }

            public int GetId() => id;

            public object this[string propertyName]
                => propertyName switch
                {
                    "id" => id,
                    "name" => name,
                    _ => default,
                };

            public object this[string propertyName, object defaultValue]
               => propertyName switch
               {
                   "id" => id,
                   "name" => name,
                   _ => defaultValue,
               };
        }


        public static List<User> GetSourceData()
        {
            int count = 1000;

            var Now = DateTime.Now;
            var list = new List<User>(count);
            for (int i = 1; i <= count; i++)
            {
                list.Add(new User
                {
                    id = i,
                    name = "name" + i,
                    birth = Now.AddSeconds(i),
                    fatherId = i >= 2 ? i >> 1 : null,
                    motherId = i >= 2 ? (i >> 1) + 1 : null,
                });
            }

            list.ForEach(user =>
            {
                var array = list.Where(child => child.fatherId == user.id || child.motherId == user.id).ToArray();
                if (array.Length != 0) user.childrenArray = array;
            });

            return list;
        }


    }
}
