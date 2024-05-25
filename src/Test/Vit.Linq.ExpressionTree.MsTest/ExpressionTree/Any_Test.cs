using System.Linq.Expressions;

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Vit.Core.Module.Serialization;
using Vit.Extensions.Linq_Extensions;
using System.Data;
using System.Linq;
using System;
using Vit.Linq.ExpressionTree;
using Vit.Linq.ExpressionTree.ComponentModel.CollectionQuery;

namespace Vit.Linq.MsTest.Converter
{
    [TestClass]
    public class Any_Test
    {
        [TestMethod]
        public void Test()
        {
            {
                Func<Expression, Type, object> QueryExecutor = (expression,type) =>
                {
                    var convertService = ExpressionConvertService.Instance;
                    var node = convertService.ConvertToData(expression);

                    var strNode = Json.Serialize(node);
                    // ToLambdaExpression       
                    var exp3 = (Expression<Func<IQueryable<Person>, IQueryable<Person>>>)convertService.ToLambdaExpression(node, typeof(IQueryable<Person>));
                    var predicate_ = exp3.Compile();

                    // As Filter
                    var queryAction = new QueryAction(node);
                    var strQuery = Json.Serialize(queryAction);
                    var predicate = convertService.ToPredicateExpression<Person>(queryAction.filter, "person");
                    //var lambdaExp = convertService.ToLambdaExpression(queryAction.filter,typeof(Person), "person");


                    //var query2 = new ExpressionConverter().ConvertToQueryAction(exp2);
                    //var strQuery2 = Json.Serialize(query2);

                    var list = DataSource.GetQueryable().Where(predicate);


                    list = list.OrderBy(queryAction.orders);

                    var methodName = queryAction.method;

                    if (methodName == "TotalCount") return list.Count();

                    if (queryAction.skip.HasValue)
                        list = list.Skip(queryAction.skip.Value);
                    if (queryAction.take.HasValue)
                        list = list.Take(queryAction.take.Value);

                    switch (methodName)
                    {
                        case "First": return list.First();
                        case "FirstOrDefault": return list.FirstOrDefault();
                        case "Last": return list.Last();
                        case "LastOrDefault": return list.LastOrDefault();
                        case "Count": return list.Count();
                    }

                    // ToList
                    return list;
                };

                var query = QueryableBuilder.Build<Person>(QueryExecutor);

                var persons = new List<Person> { new Person { id = 2 }, new Person { id = 3 } }.AsQueryable();
                var pids = persons.Select(m => m.id).ToList();


                var jobs = new List<Job> { new Job { departmentId = 1 }, new Job { departmentId = 2 } };
                //var joins = query.Join(jobs, outer => outer.departmentId, inner => inner.departmentId, (left, right) => new { person = left, job = right });
                //var result = joins.ToList();

                //var joins = query.Join(jobs, outer => outer.departmentId, inner => inner.departmentId, (left, right) => new Person{departmentId = right.departmentId,name=left.name });
                //var result = joins.ToList();


                query = query
                        .Where(m => persons.Any(p => p.id == m.id)) // MethodCall Any
                        ;

                var list = query.ToList();

                var count = query.Count();
                var totalCount = query.TotalCount();
            

                Assert.AreEqual(5, list.Count);
                Assert.AreEqual(17, list[0].id);
                Assert.AreEqual(15, list[1].id);
            }


        }




    }
}
