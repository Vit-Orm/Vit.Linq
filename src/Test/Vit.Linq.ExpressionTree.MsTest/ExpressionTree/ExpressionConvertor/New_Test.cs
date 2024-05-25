using Microsoft.VisualStudio.TestTools.UnitTesting;

using System;
using System.Linq;
using System.Linq.Expressions;

using Vit.Core.Module.Serialization;
using Vit.Linq.ExpressionTree;
using Vit.Linq.ExpressionTree.ComponentModel;

namespace Vit.Linq.MsTest.New.test2.ExpressionConvertor
{
    [TestClass]
    public class New_Test
    {
        [TestMethod]
        public void Test()
        {


            Func<Expression,  Type, object> QueryExecutor = (expression, type) =>
            {
                var convertService = ExpressionConvertService.Instance;

                ExpressionNode node;


                #region ExpressinNode
                {
                    // #1 Code to Data
                    // (query) => query.Where().OrderBy().Skip().Take().Select().ToList();
                    node = convertService.ConvertToData(expression);

                    // {"nodeType":"Lambda","parameterNames":["Param_1"],"body":{"nodeType":"Call","methodName":"Where","arguments":[{"parameterName":"Param_1","nodeType":"Member"},{"nodeType":"Lambda","parameterNames":["Param_0"],"body":{"left":{"nodeType":"Member","parameterName":"Param_0","memberName":"id"},"right":{"nodeType":"Constant","valueType":{"typeName":"Int32"},"value":10},"nodeType":"GreaterThanOrEqual"}}]}}
                    var strNode = Json.Serialize(node);

                    // #2 Data to Code
                    // Param_1 => Param_1.Where(Param_0 => (Param_0.id >= 10))
                    var lambdaExp = convertService.ToLambdaExpression(node, typeof(IQueryable<Person>));
                    //var exp3 = (Expression<Func<IQueryable<Person>, IQueryable<Person>>>)lambdaExp;
                    var predicate = lambdaExp.Compile();
                }
                #endregion

 

                //var list = DataSource.GetQueryable().Where(predicate);


               

                throw new NotSupportedException("Method not support"  );
            };

            var query = QueryableBuilder.Build<Person>(QueryExecutor);




            var list = query.Select(p => new { name =  p.name }).ToList();
            //Assert.AreEqual(5, list.Count);
            //Assert.AreEqual(17, list[0].id);
            //Assert.AreEqual(15, list[1].id);
        }


    }
}
