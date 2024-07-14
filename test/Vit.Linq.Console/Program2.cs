using Vit.Core.Module.Serialization;
using Vit.Linq;
using Vit.Linq.ExpressionTree;

namespace App
{
    internal class Program2
    {      
        static void Main2(string[] args)
        {
            var users = new[] { new User(1), new User(2), new User(3), new User(4)};
            var query = users.AsQueryable();

            var queryExpression = users.AsQueryable().Where(m => m.id > 0).OrderBy(m => m.id).Skip(1).Take(2);

            #region #1 Expression to ExpressionNode (Code to Data)
            var node = ExpressionConvertService.Instance.ConvertToData_LambdaNode(queryExpression.Expression);
            var strNode = Json.Serialize(node);
            #endregion

            #region #2 ExpressionNode to QueryAction
            var queryAction = new Vit.Linq.ExpressionTree.Query.QueryAction(node);
            var strQuery = Json.Serialize(queryAction);
            #endregion

            // #3 compile code
            var predicate = ExpressionConvertService.Instance.ConvertToCode_PredicateExpression<User>(queryAction.filter);
            //var lambdaExp = (Expression<Func<Person, bool>>)convertService.ToLambdaExpression(queryAction.filter, typeof(User));

            var rangedQuery = query.Where(predicate).OrderBy(queryAction.orders);
            if (queryAction.skip.HasValue)
                rangedQuery = rangedQuery.Skip(queryAction.skip.Value);
            if (queryAction.take.HasValue)
                rangedQuery = rangedQuery.Take(queryAction.take.Value);


            var result = rangedQuery.ToList();
            var count = result.Count;

        }

        class User
        {
            public User(int id) { this.id = id; this.name = "name" + id; }
            public int id { get; set; }
            public string name { get; set; }
        }
    }
}
