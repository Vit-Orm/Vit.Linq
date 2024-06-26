# Not supported ExpressionType

``` csharp
public enum ExpressionType
{
	//
	// Summary:
	//     An addition operation, such as (a + b), with overflow checking, for numeric operands.
	AddChecked = 1,

	//
	// Summary:
	//     A cast or conversion operation, such as (SampleType)obj in C#or CType(obj, SampleType)
	//     in Visual Basic. For a numeric conversion, if the converted value does not fit
	//     the destination type, an exception is thrown.
	ConvertChecked = 11,


	//
	// Summary:
	//     An operation that invokes a delegate or lambda expression, such as sampleDelegate.Invoke().
	Invoke = 17,
	//
	// Summary:
	//     A lambda expression, such as a => a + a in C# or Function(a) a + a in Visual
	//     Basic.
	Lambda = 18,



	//
	// Summary:
	//     An operation that creates a new System.Collections.IEnumerable object and initializes
	//     it from a list of elements, such as new List(){ a, b, c } in C# or Dim sampleList
	//     = { a, b, c } in Visual Basic.
	ListInit = 22,

	//
	// Summary:
	//     An operation that creates a new object and initializes one or more of its members,
	//     such as new Point { X = 1, Y = 2 } in C# or New Point With {.X = 1, .Y = 2} in
	//     Visual Basic.
	MemberInit = 24,


	//
	// Summary:
	//     An multiplication operation, such as (a * b), that has overflow checking, for
	//     numeric operands.
	MultiplyChecked = 27,

	//
	// Summary:
	//     A unary plus operation, such as (+a). The result of a predefined unary plus operation
	//     is the value of the operand, but user-defined implementations might have unusual
	//     results.
	UnaryPlus = 29,

	//
	// Summary:
	//     An arithmetic negation operation, such as (-a), that has overflow checking. The
	//     object a should not be modified in place.
	NegateChecked = 30,

	//
	// Summary:
	//     An operation that calls a constructor to create a new object, such as new SampleType().
	New = 31,

	//
	// Summary:
	//     An operation that creates a new one-dimensional array and initializes it from
	//     a list of elements, such as new SampleType[]{a, b, c} in C# or New SampleType(){a,
	//     b, c} in Visual Basic.
	NewArrayInit = 32,

	//
	// Summary:
	//     An operation that creates a new array, in which the bounds for each dimension
	//     are specified, such as new SampleType[dim1, dim2] in C# or New SampleType(dim1,
	//     dim2) in Visual Basic.
	NewArrayBounds = 33,



	//
	// Summary:
	//     A reference to a parameter or variable that is defined in the context of the
	//     expression. For more information, see System.Linq.Expressions.ParameterExpression.
	Parameter = 38,

	//
	// Summary:
	//     An expression that has a constant value of type System.Linq.Expressions.Expression.
	//     A System.Linq.Expressions.ExpressionType.Quote node can contain references to
	//     parameters that are defined in the context of the expression it represents.
	Quote = 40,

	//
	// Summary:
	//     An arithmetic subtraction operation, such as (a - b), that has overflow checking,
	//     for numeric operands.
	SubtractChecked = 43,
	//
	// Summary:
	//     An explicit reference or boxing conversion in which null is supplied if the conversion
	//     fails, such as (obj as SampleType) in C# or TryCast(obj, SampleType) in Visual
	//     Basic.
	TypeAs = 44,

	//
	// Summary:
	//     A type test, such as obj is SampleType in C# or TypeOf obj is SampleType in Visual
	//     Basic.
	TypeIs = 45,

	//
	// Summary:
	//     An assignment operation, such as (a = b).
	Assign = 46,
	//
	// Summary:
	//     A block of expressions.
	Block = 47,
	//
	// Summary:
	//     Debugging information.
	DebugInfo = 48,
	//
	// Summary:
	//     A unary decrement operation, such as (a - 1) in C# and Visual Basic. The object
	//     a should not be modified in place.
	Decrement = 49,
	//
	// Summary:
	//     A dynamic operation.
	Dynamic = 50,
	//
	// Summary:
	//     A default value.
	Default = 51,
	//
	// Summary:
	//     An extension expression.
	Extension = 52,
	//
	// Summary:
	//     A "go to" expression, such as goto Label in C# or GoTo Label in Visual Basic.
	Goto = 53,
	//
	// Summary:
	//     A unary increment operation, such as (a + 1) in C# and Visual Basic. The object
	//     a should not be modified in place.
	Increment = 54,
	//
	// Summary:
	//     An index operation or an operation that accesses a property that takes arguments.
	Index = 55,
	//
	// Summary:
	//     A label.
	Label = 56,
	//
	// Summary:
	//     A list of run-time variables. For more information, see System.Linq.Expressions.RuntimeVariablesExpression.
	RuntimeVariables = 57,
	//
	// Summary:
	//     A loop, such as for or while.
	Loop = 58,
	//
	// Summary:
	//     A switch operation, such as switch in C# or Select Case in Visual Basic.
	Switch = 59,
	//
	// Summary:
	//     An operation that throws an exception, such as throw new Exception().
	Throw = 60,
	//
	// Summary:
	//     A try-catch expression.
	Try = 61,
	//
	// Summary:
	//     An unbox value type operation, such as unbox and unbox.any instructions in MSIL.
	Unbox = 62,
	//
	// Summary:
	//     An addition compound assignment operation, such as (a += b), without overflow
	//     checking, for numeric operands.
	AddAssign = 63,
	//
	// Summary:
	//     A bitwise or logical AND compound assignment operation, such as (a &= b) in C#.
	AndAssign = 64,
	//
	// Summary:
	//     An division compound assignment operation, such as (a /= b), for numeric operands.
	DivideAssign = 65,
	//
	// Summary:
	//     A bitwise or logical XOR compound assignment operation, such as (a ^= b) in C#.
	ExclusiveOrAssign = 66,
	//
	// Summary:
	//     A bitwise left-shift compound assignment, such as (a <<= b).
	LeftShiftAssign = 67,
	//
	// Summary:
	//     An arithmetic remainder compound assignment operation, such as (a %= b) in C#.
	ModuloAssign = 68,
	//
	// Summary:
	//     A multiplication compound assignment operation, such as (a *= b), without overflow
	//     checking, for numeric operands.
	MultiplyAssign = 69,
	//
	// Summary:
	//     A bitwise or logical OR compound assignment, such as (a |= b) in C#.
	OrAssign = 70,
	//
	// Summary:
	//     A compound assignment operation that raises a number to a power, such as (a ^=
	//     b) in Visual Basic.
	PowerAssign = 71,
	//
	// Summary:
	//     A bitwise right-shift compound assignment operation, such as (a >>= b).
	RightShiftAssign = 72,
	//
	// Summary:
	//     A subtraction compound assignment operation, such as (a -= b), without overflow
	//     checking, for numeric operands.
	SubtractAssign = 73,
	//
	// Summary:
	//     An addition compound assignment operation, such as (a += b), with overflow checking,
	//     for numeric operands.
	AddAssignChecked = 74,
	//
	// Summary:
	//     A multiplication compound assignment operation, such as (a *= b), that has overflow
	//     checking, for numeric operands.
	MultiplyAssignChecked = 75,
	//
	// Summary:
	//     A subtraction compound assignment operation, such as (a -= b), that has overflow
	//     checking, for numeric operands.
	SubtractAssignChecked = 76,
	//
	// Summary:
	//     A unary prefix increment, such as (++a). The object a should be modified in place.
	PreIncrementAssign = 77,
	//
	// Summary:
	//     A unary prefix decrement, such as (--a). The object a should be modified in place.
	PreDecrementAssign = 78,
	//
	// Summary:
	//     A unary postfix increment, such as (a++). The object a should be modified in
	//     place.
	PostIncrementAssign = 79,
	//
	// Summary:
	//     A unary postfix decrement, such as (a--). The object a should be modified in
	//     place.
	PostDecrementAssign = 80,
	//
	// Summary:
	//     An exact type test.
	TypeEqual = 81,
	//
	// Summary:
	//     A ones complement operation, such as (~a) in C#.
	OnesComplement = 82,
	//
	// Summary:
	//     A true condition value.
	IsTrue = 83,
	//
	// Summary:
	//     A false condition value.
	IsFalse = 84
}

```