using System.Linq.Expressions;

namespace Vit.Linq.ExpressionNodes.ExpressionTreeTest
{

    public partial class ExpressionTester
    {
        public static void TestEnumerable_NoSerializable(IQueryable<User> query)
        {
            Expression<Func<User, bool>> predicate;


            #region #24 MemberInit,  An operation that creates a new object and initializes one or more of its members, such as new Point { X = 1, Y = 2 }
            {
                predicate = u => new User { id = 2 }.id == u.id;
                var rows = ExpressionTester.Test(query, predicate);
            }
            #endregion

            #region #31 New,  An operation that calls a constructor to create a new object, such as new SampleType().
            {
                predicate = u => new User(2).id == u.id;
                var rows = ExpressionTester.Test(query, predicate);
            }
            {
                predicate = u => new User("name3").name == u.name;
                var rows = ExpressionTester.Test(query, predicate);
            }
            {
                predicate = u => new User(2) { name = "name3" }.id == u.id || new User(2) { name = "name3" }.name == u.name;
                var rows = ExpressionTester.Test(query, predicate);
            }
            #endregion

        }

        public static void TestEnumerable(IQueryable<User> query)
        {
            Expression<Func<User, bool>> predicate;

            #region #2 And, A bitwise or logical AND operation, such as (a & b)
            {
                predicate = u => (u.id & 0b_0101) == 5;
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

            #region #6 Call,  A method call, such as in the obj.sampleMethod() expression
            {
                predicate = u => u.GetId() == 2;
                var rows = Test(query, predicate);
            }
            {
                // call MethodCall get_Item("parent", 5)
                predicate = u => (u["parent", 5] as int?) == 5;
                var rows = ExpressionTester.Test(query, predicate);
            }
            {  // call MethodCall  get_Item("id")
                predicate = u => (u["id"] as int?) == 2;
                var rows = ExpressionTester.Test(query, predicate);
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

            #region #22 ListInit,  An operation that creates a new System.Collections.IEnumerable object and initializes it from a list of elements, such as new List(){ a, b, c } 
            {
                predicate = u => !new List<int>() { 2, u.id + 1 }.Contains(u.id);
                var rows = ExpressionTester.Test(query, predicate);
            }
            {
                predicate = u => !new List<int>(2) { 2, u.id + 1 }.Contains(u.id);
                var rows = ExpressionTester.Test(query, predicate);
            }
            {
                predicate = u => !new List<int>(2) { }.Contains(u.id);
                var rows = ExpressionTester.Test(query, predicate);
            }
            #endregion


            #region #32 NewArrayInit,  An operation that creates a new one-dimensional array and initializes it from a list of elements, such as new SampleType[]{a, b, c} 
            {
                predicate = u => !new int[] { 2, u.id + 1 }.Contains(u.id);
                var rows = ExpressionTester.Test(query, predicate);
            }
            {
                predicate = u => !new int[] { }.Contains(u.id);
                var rows = ExpressionTester.Test(query, predicate);
            }
            {
                predicate = u => !new int[0].Contains(u.id);
                var rows = ExpressionTester.Test(query, predicate);
            }
            {
                predicate = u => !new int[2].Contains(u.id);
                var rows = ExpressionTester.Test(query, predicate);
            }
            #endregion

            #region #34 Not,  A bitwise complement or logical negation operation. It is equivalent to  (~a) for integral types and to (!a) for Boolean values
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

            #region #41 RightShift,  A bitwise right-shift operation, such as (a >> b)
            {
                predicate = u => (u.id >> 2) == 1;
                var rows = Test(query, predicate);
            }
            #endregion

            #region #44 TypeAs,  An explicit reference or boxing conversion in which null is supplied if the conversion fails, such as (obj as SampleType)
            {
                predicate = u => (((object)u.id) as int?) == 2;
                var rows = ExpressionTester.Test(query, predicate);
            }
            #endregion

            #region #45 TypeIs,  A type test, such as obj is SampleType
            {
                predicate = u => ((object)u.id) is int;
                var rows = ExpressionTester.Test(query, predicate);
            }
            #endregion

            #region #51 Default,  A default value
            {
                predicate = u => (u.id - 1) == default(int);
                var rows = ExpressionTester.Test(query, predicate);
            }
            #endregion

        }
    }
}
