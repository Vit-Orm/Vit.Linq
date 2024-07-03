using System;
using System.Linq;
using System.Linq.Expressions;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using Vit.Core.Module.Serialization;
using Vit.Linq.ExpressionTree.CollectionQuery;
using Vit.Linq.ExpressionTree.ComponentModel;
using Vit.Linq;
using Vit.Linq.ExpressionTree.ExpressionTreeTest;

namespace Vit.Linq.ExpressionTree.MsTest.ExpressionTreeTest
{
    [TestClass]
    public class QueryAction_Test
    {


        [TestMethod]
        public void TestQueryable()
        {
            ExpressionTester.TestQueryable(GetQuery());
        }




        static IQueryable<ExpressionTester.User> GetQuery()
        {
            var convertService = ExpressionConvertService.Instance;
            var queryTypeName = "TestQuery";
            var sourceData = ExpressionTester.GetSourceData().AsQueryable();

            Func<Expression, Type, object> QueryExecutor = (expression, type) =>
            {
                ExpressionNode node;

                // #1 Code to Data
                // query => query.Where().OrderBy().Skip().Take().Select().ToList();
                var isArgument = QueryableBuilder.CompareQueryByName(queryTypeName);
                node = convertService.ConvertToData(expression, autoReduce: true, isArgument: isArgument);
                var strNode = Json.Serialize(node);

                // #2 Filter by QueryAction
                var queryAction = new QueryAction(node);
                var strQuery = Json.Serialize(queryAction);
                var predicate = convertService.ToPredicateExpression<ExpressionTester.User>(queryAction.filter);
                //var lambdaExp = (Expression<Func<Person, bool>>)convertService.ToLambdaExpression(queryAction.filter, typeof(Person));

                var query = sourceData.Where(predicate);

                query = query.OrderBy(queryAction.orders);

                var methodName = queryAction.method;

                if (methodName == "TotalCount") return query.Count();

                if (queryAction.skip.HasValue)
                    query = query.Skip(queryAction.skip.Value);
                if (queryAction.take.HasValue)
                    query = query.Take(queryAction.take.Value);

                switch (methodName)
                {
                    case "First": return query.First();
                    case "FirstOrDefault": return query.FirstOrDefault();
                    case "Last": return query.Last();
                    case "LastOrDefault": return query.LastOrDefault();
                    case "Count": return query.Count();
                    case "ToList":
                    case "":
                    case null:
                        return query;
                }

                throw new NotSupportedException("Method not support:" + methodName);
            };

            return QueryableBuilder.Build<ExpressionTester.User>(QueryExecutor, queryTypeName);
        }



    }
}
