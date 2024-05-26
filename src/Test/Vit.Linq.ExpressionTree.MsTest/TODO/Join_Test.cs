using System;
using System.Linq;
using System.Linq.Expressions;

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Vit.Core.Module.Serialization;
using Vit.Extensions.Linq_Extensions;
using Vit.Linq.ExpressionTree;
using Vit.Linq.ExpressionTree.ComponentModel;

namespace Vit.Linq.ExpressionTree.MsTest
{
    [TestClass]
    public class Join_Test
    {
        [TestMethod]
        public void Test()
        {
            {
                Func<Expression, Type, object> QueryExecutor = (expression, type) =>
                {
                    var convertService = ExpressionConvertService.Instance;

                    ExpressionNode node;
                    Delegate predicate;

                    #region get ExpressionNode
                    {
                        // (query,query2) => query.SelectMany(query2).Where().OrderBy().Skip().Take().Select().ToList();

                        //  users
                        // .SelectMany(
                        //      user => users.Where(father => (Convert(father.id, Nullable`1) == user.fatherId)).DefaultIfEmpty(),
                        //      (user, father) => new <>f__AnonymousType4`2(user = user, father = father)
                        //  ).SelectMany(
                        //      item => users.Where(mother => (Convert(mother.id, Nullable`1) == item.user.fatherId)).DefaultIfEmpty(),
                        //      (item, mother) => new <>f__AnonymousType5`3(user = item.user, father = item.father, mother = mother)
                        //  )
                        node = convertService.ConvertToData(expression, autoReduce: true);

                        var strNode = Json.Serialize(node);
                    }
                    #endregion

                    var stream = ComponentModel.CollectionsQuery.StreamReader.ReadNode(node);
                    var strStream = Json.Serialize(stream);
                    return null;
                };


                //LeftJoin/InnerJoin  (SelectMany) single and select
                {
                    var users = QueryableBuilder.Build<User>(QueryExecutor);

                    var list = users
                        .SelectMany(
                            user => users.Where(father => father.id == user.fatherId).DefaultIfEmpty(),
                            (user, father) => new { user1 = user, father1 = father, id1 = user.id }
                        )
                        .SelectMany(
                            item => users.Where(mother => mother.id == item.user1.motherId).DefaultIfEmpty(),
                            (item, mother) => new { user2 = item.user1, father2 = item.father1, mother2 = mother, id2 = item.id1, fatherId2 = item.father1.id }
                        )
                        //.SelectMany(
                        //    item => users.Where(mother => mother.id == item.user.motherId).DefaultIfEmpty(),
                        //    (item, mother) => new { item.user, father2 = item.father1, mother2 = mother, item.id, fatherId = item.father1.id }
                        //)
                        .Where(items => items.user2.id > 0)
                        .Where(items => items.fatherId2 > 0)
                        .Select((item) => new
                        {
                            user9 = item.user2,
                            id9 = item.id2,
                            father9 = item.father2,
                            fatherId9 = item.fatherId2,
                            mother9 = item.mother2,
                            motherId9 = item.mother2.id
                        })
                        .ToList();
                }

                //LeftJoin/InnerJoin  (SelectMany)
                {
                    var users = QueryableBuilder.Build<User>(QueryExecutor);

                    var list = users
                        .SelectMany(
                            user => users.Where(father => father.id == user.fatherId).DefaultIfEmpty(),
                            (user, father) => new { user, father }
                        )
                        .SelectMany(
                            item => users.Where(mother => mother.id == item.user.fatherId).DefaultIfEmpty(),
                            (item, mother) => new { item.user, item.father, mother }
                        )
                        .ToList();
                }


                //LeftJoin/InnerJoin  (SelectMany) single
                {
                    var users = QueryableBuilder.Build<User>(QueryExecutor);

                    var list = users
                        .SelectMany(
                            user => users.Where(father => father.id == user.fatherId).DefaultIfEmpty(),
                            (user, father) => new { user, father }
                        )
                        .Select((item) => new { item.user.id })
                        .ToList();
                }



                {
                    //var users = QueryableBuilder.Build<User>(QueryExecutor);

                    //var list = users
                    //    .SelectMany(
                    //        user => users.Where(father => father.id == user.fatherId).DefaultIfEmpty(),
                    //         // (user, father) => new { user, father } //NewExpression {new <>f__AnonymousType4`2(user = user, father = father)}
                    //         (user, father) => new Result(user)   //NewExpression   new Result(user)
                    //        // (user, father) => new Result { user = user, father = father } // MemberInitExpression new Result() {user = user, father = father}
                    //     )
                    //    .ToList();
                }

                #region #3 LeftJoin/InnerJoin  (SelectMany)


                {
                    var users = QueryableBuilder.Build<User>(QueryExecutor);
                    /*
                    {value(Vit.Linq.Converter.OrderedQueryable`1[Vit.Linq.MsTest.Converter.Join_Test+User])
                    .SelectMany(
                        user => value(Vit.Linq.MsTest.Converter.Join_Test+<>c__DisplayClass0_0).users
                            .Where(f => (Convert(f.id, Nullable`1) == user.fatherId)).DefaultIfEmpty(),
                        (user, father) => new <>f__AnonymousType4`2(user = user, father = father)  )
                    .SelectMany(
                        <>h__TransparentIdentifier0 => value(Vit.Linq.MsTest.Converter.Join_Test+<>c__DisplayClass0_0).users
                                .Where(m => ((Convert(m.id, Nullable`1) == <>h__TransparentIdentifier0.user.motherId) 
                                    AndAlso (Convert(<>h__TransparentIdentifier0.father.id, Nullable`1) != m.fatherId))).DefaultIfEmpty(),
                        (<>h__TransparentIdentifier0, mother) => new <>f__AnonymousType5`3(
                            user = <>h__TransparentIdentifier0.user, 
                            father = <>h__TransparentIdentifier0.father,
                            mother = mother
                    ))
                    */
                    var list = (from user in users
                                from father in users.Where(f => f.id == user.fatherId).DefaultIfEmpty()
                                from mother in users.Where(m => m.id == user.motherId && father.id != m.fatherId).DefaultIfEmpty()
                                select new
                                {
                                    user,
                                    father,
                                    mother
                                }).ToList();

                }



                #region #1 Join
                {
                    //var lefts = QueryableBuilder.Build<Person>(QueryExecutor);
                    //var rights = QueryableBuilder.Build<Person>(QueryExecutor);

                    //var list = lefts.Join(
                    //    rights,
                    //    left => left.id,
                    //    right => right.id,
                    //    (left, right) => new Result
                    //    {
                    //        left = left,
                    //        right = right
                    //    }
                    //).ToList();

                    //Assert.AreEqual(1000, list.Count);
                }
                #endregion



                #region #2 Join
                {
                    //var lefts = QueryableBuilder.Build<Person>(QueryExecutor);
                    //var rights = QueryableBuilder.Build<Person>(QueryExecutor);

                    //var list = (from left in lefts
                    //            join right in rights on left.id equals right.id
                    //            // into rights_
                    //            // from right in rights_.DefaultIfEmpty()
                    //            where left.id == right.id && right.isEven
                    //            select new Result
                    //            {
                    //                left = left,
                    //                right = right,
                    //                id = left.id > 0 ? left.id : right.id,
                    //                departmentId = (right ?? left).departmentId
                    //            }
                    //    ).ToList();

                    //Assert.AreEqual(500, list.Count);
                }
                #endregion



                #endregion


                //var users = new List<Person> { new Person { departmentId = 1, id = 1 }, new Person { departmentId = 2, id = 21 } };
                //var departs = new[] { new { departmentId = 1, name = "part1" }, new { departmentId = 2, name = "part1" } };
                //var userInfos2 = users.Join(departs, user => user.departmentId, depart => depart.departmentId, (user, depart) => new { user, depart });
                //var userInfos = users.AsQueryable().Join(departs, user => user.departmentId, depart => depart.departmentId, (user, depart) => new { user, depart });


            }




        }
        class Result
        {
            public Result() { }
            public Result(User user) { this.user = user; }
            public User user;
            public User father { get; set; }
            public int? id { get; set; }
            public int? departmentId { get; set; }
        }


        public class User
        {

            //[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
            public int id { get; set; }
            public string name { get; set; }
            public DateTime? birth { get; set; }

            public int? fatherId { get; set; }
            public int? motherId { get; set; }
        }


    }
}
