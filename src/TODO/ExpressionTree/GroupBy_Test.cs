using System.Linq.Expressions;

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Vit.Core.Module.Serialization;
using Vit.Linq.Converter;
using Vit.Linq.Filter;
using Vit.Extensions.Linq_Extensions;
using System.Data;
using System.Linq;
using System;

namespace Vit.Linq.MsTest.Converter
{
    [TestClass]
    public class GroupBy_Test
    {
        [TestMethod]
        public void Test()
        {
            {
                Func<Expression, Expression, Type, object> QueryExecutor = (expression,rootExpression,type) =>
                {
                    var convertService = ExpressionConvertService.Instance;
                    var node = convertService.ConvertToData(expression, rootExpression);

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

                var groups = query.GroupBy(p => p.departmentId).ToList();


                var list = groups.ToList();

                var count = groups.Count();
                var totalCount = groups.AsQueryable().TotalCount();


                //Assert.AreEqual(5, list.Count); 
            }


        }




    }
}
