using Vit.Core.Module.Serialization;
using Vit.Linq;
using Vit.Linq.Filter.ComponentModel;

namespace App
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var users = new[] { new { id = 1, name = "name1" }, new { id = 2, name = "name2" } };

            var strRule = "{\"field\":\"id\",  \"operator\": \"=\",  \"value\": 1 }";
            var rule = Json.Deserialize<FilterRule>(strRule);

            var result = users.AsQueryable().Where(rule).ToList();

            var count = result.Count;
        }
    }
}
