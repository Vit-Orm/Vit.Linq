using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using Vit.Core.Module.Serialization;

namespace Vit.Linq.ExpressionTree.MsTest
{
    [TestClass]
    public class ConstValue_Test
    {

        void Test(Expression<Func<Person, bool>> predicate)
        {
            var convertService = ExpressionConvertService.Instance;

            // #1 to ExpressionNode
            // () =>  (query)=> query.isEven
            var node = convertService.ConvertToData(predicate);
            var str = Json.Serialize(node);

            // #2 get convertedPredicate
            //var lambda = convertService.ToLambdaExpression(node);
            //var del = lambda.Compile();
            //var convertedPredicate = del.DynamicInvoke() as Func<Person, bool>;
            var convertedPredicate = convertService.ToPredicate<Person>(node.body);


            IQueryable<Person> queryable = DataSource.GetQueryable();

            var result1 = queryable.Where(predicate).ToList();
            var result2 = queryable.Where(convertedPredicate).ToList();


            Assert.AreEqual(result1.Count, result2.Count);
            for (var t = 0; t < result1.Count; t++)
            {
                Assert.AreEqual(result1[t].id, result2[t].id);
            }
        }


        [TestMethod]
        public void Test()
        {
            {
                Expression<Func<Person, bool>> predicate = x => x.isEven;

                Test(predicate);
            }

            {
                var person = new { isEven = true };
                Expression<Func<Person, bool>> predicate = x => person.isEven;

                Test(predicate);
            }

            {
                var person = new { names = new[] { "name2" } };
                Expression<Func<Person, bool>> predicate = x => x.name == person.names[0];

                Test(predicate);
            }

            {
                var person = new { names = new List<string> { "name2_job1" } };
                Expression<Func<Person, bool>> predicate = x => x.jobArray[0].name == person.names[0];

                Test(predicate);
            }

            {
                var person = new { id = 10, name = "name5" };
                Expression<Func<Person, bool>> predicate = x => x.id <= person.id && x.name == person.name;

                Test(predicate);
            }

        }



    }
}
