using System;
using System.Linq;
using System.Linq.Expressions;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using Vit.Core.Module.Serialization;
using Vit.Linq;
using Vit.Linq.ExpressionTree.ComponentModel;
using Vit.Linq.ExpressionTree.ExpressionTreeTest;
using Vit.Linq.ExpressionTree.Query;

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
                ExpressionNode_Lambda node;

                // #1 Code to Data
                // query => query.Where().OrderBy().Skip().Take().Select().ToList();
                var isArgument = QueryableBuilder.CompareQueryByName(queryTypeName);
                node = convertService.ConvertToData_LambdaNode(expression, autoReduce: true, isArgument: isArgument);
                var strNode = Json.Serialize(node);

                // #2 Filter by CollectionQuery
                var collectionQuery = new CollectionQuery(node);
                var strQuery = Json.Serialize(collectionQuery);
                var predicate = convertService.ConvertToCode_PredicateExpression<ExpressionTester.User>(collectionQuery.filter);
                //var lambdaExp = (Expression<Func<Person, bool>>)convertService.ToLambdaExpression(queryAction.filter, typeof(Person));

                var query = sourceData.Where(predicate);

                query = query.OrderBy(collectionQuery.orders);

                var rangedQuery = query;

                if (collectionQuery.skip.HasValue)
                    rangedQuery = rangedQuery.Skip(collectionQuery.skip.Value);
                if (collectionQuery.take.HasValue)
                    rangedQuery = rangedQuery.Take(collectionQuery.take.Value);

                switch (collectionQuery.method)
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

                throw new NotSupportedException("Method not support:" + collectionQuery.method);
            };

            return QueryableBuilder.Build<ExpressionTester.User>(QueryExecutor, queryTypeName);
        }



    }
}
