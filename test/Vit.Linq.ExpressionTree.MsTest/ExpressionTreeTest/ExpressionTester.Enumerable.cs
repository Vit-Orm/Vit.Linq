using System;
using System.Linq;
using System.Linq.Expressions;

namespace Vit.Linq.ExpressionTree.ExpressionTreeTest
{

    public partial class ExpressionTester
    {

        public static void TestEnumerable(IQueryable<User> query)
        {
            Expression<Func<User, bool>> predicate;


            #region #2 And, A bitwise or logical AND operation, such as (a & b)
            {
                predicate = u => (u.id & 0b_1001) == 6;
                var rows = Test(query, predicate);
            }
            #endregion


            #region #4 ArrayLength, An operation that obtains the length of a one-dimensional array, such as array.Length.
            {
                predicate = u => u.childrenArray != null && u.childrenArray.Length == 2;
                var rows = Test(query, predicate);
            }
            {
                predicate = u => u.childrenArray == null || u.childrenArray.Length == 0;
                var rows = Test(query, predicate);
            }
            #endregion

            #region #5 ArrayIndex,  An indexing operation in a one-dimensional array, such as array[index]
            {
                predicate = u => u.childrenArray != null && u.childrenArray[0].id == 2;
                var rows = Test(query, predicate);
            }
            #endregion

            #region #6 Call,  A method call, such as in the obj.sampleMethod() expression.
            {
                predicate = u => u.GetId() == 2;
                var rows = Test(query, predicate);
            }
            #endregion

            #region #14 ExclusiveOr, A bitwise or logical XOR operation, such as (a ^ b)
            {
                predicate = u => (u.id ^ 0b_1001) == 6;
                var rows = Test(query, predicate);
            }
            #endregion

            #region #19 LeftShift, A bitwise left-shift operation, such as (a << b).
            {
                predicate = u => (u.id << 2) == 12;
                var rows = Test(query, predicate);
            }
            #endregion

            #region #34 Not,  A bitwise complement or logical negation operation. It is equivalent to  (~a) for integral types and to (!a) for Boolean values.
            {
                predicate = u => ~u.id == ~2;
                var rows = Test(query, predicate);
            }
            #endregion

            #region #36 Or,  A bitwise or logical OR operation, such as (a | b)
            {
                predicate = u => (u.id | 0b_0000) == 2;
                var rows = Test(query, predicate);
            }
            #endregion


            #region #41 RightShift,  A bitwise right-shift operation, such as (a >> b).
            {
                predicate = u => (u.id >> 2) == 1;
                var rows = Test(query, predicate);
            }
            #endregion

        }
    }
}
