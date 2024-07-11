# Vit.Library ReleaseLog

-----------------------
# 2.2.25

- [ExpressionNode] ExpressionNode_New store ConstructorArgTypes to CodeArg
- fix treat String.Add as Numeric.Add issue

- [ExpressionNode] support Data to Code
    - 24 MemberInit
    - 31 New
    - 22 ListInit
    - 32 NewArrayInit
    - 44 TypeAs
    - 45 TypeIs
    - 51 Default
- [ExpressionNode] remove config of reduceConvertExpression, judge by ConvertExpression.Type.IsValueType

- support TotalCount (Vit.Linq.Queryable_Extensions.TotalCount) 
- support ToListAndTotalCount (Vit.Linq.Queryable_Extensions.ToListAndTotalCount)

-----------------------
# 2.2.23

- support bitwise and other binary operators



