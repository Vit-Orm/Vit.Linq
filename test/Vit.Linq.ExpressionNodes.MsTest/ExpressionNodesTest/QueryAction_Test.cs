using System.Linq.Expressions;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using Vit.Core.Module.Serialization;
using Vit.Linq;
using Vit.Linq.ExpressionNodes.ComponentModel;
using Vit.Linq.ExpressionNodes.ExpressionNodesTest;
using Vit.Linq.ExpressionNodes.Query;

namespace Vit.Linq.ExpressionNodes.MsTest.ExpressionNodesTest
{
    [TestClass]
    public class CollectionQuery_Test
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
                ExpressionNode_Lambda node;

                // #1 Code to Data
                // query => query.Where().OrderBy().Skip().Take().Select().ToList();
                var isArgument = QueryableBuilder.CompareQueryByName(queryTypeName);
                node = convertService.ConvertToData_LambdaNode(expression, autoReduce: true, isArgument: isArgument);
                var strNode = Json.Serialize(node);

                // #2 Filter by QueryAction
                var queryAction = new QueryAction(node);
                var strQuery = Json.Serialize(queryAction);
                var predicate = convertService.ConvertToCode_PredicateExpression<ExpressionTester.User>(queryAction.filter);
                //var lambdaExp = (Expression<Func<Person, bool>>)convertService.ToLambdaExpression(queryAction.filter, typeof(ExpressionTester.User));

                var query = sourceData.Where(predicate);

                query = query.OrderBy(queryAction.orders);

                var rangedQuery = query;

                if (queryAction.skip.HasValue)
                    rangedQuery = rangedQuery.Skip(queryAction.skip.Value);
                if (queryAction.take.HasValue)
                    rangedQuery = rangedQuery.Take(queryAction.take.Value);

                switch (queryAction.method)
                {
                    case nameof(Queryable.First): return rangedQuery.First();
                    case nameof(Queryable.FirstOrDefault): return rangedQuery.FirstOrDefault();
                    case nameof(Queryable.Last): return rangedQuery.Last();
                    case nameof(Queryable.LastOrDefault): return rangedQuery.LastOrDefault();
                    case nameof(Queryable.Count): return rangedQuery.Count();
                    case nameof(Queryable_Extensions.TotalCount): return query.Count();
                    case nameof(Queryable_Extensions.ToListAndTotalCount): return (rangedQuery.ToList(), query.Count());
                    case nameof(Enumerable.ToList):
                    case "":
                    case null:
                        //default: 
                        return rangedQuery;
                }

                throw new NotSupportedException("Method not support:" + queryAction.method);
            };

            return QueryableBuilder.Build<ExpressionTester.User>(QueryExecutor, queryTypeName);
        }



    }
}
