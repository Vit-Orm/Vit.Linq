# Vit.Linq ReleaseNotes

-----------------------
# 3.1.5
- FilterService.checkNullForString = true;
- StringOperators will check null not String.IsNullOrEmpty

-----------------------
# 3.1.4
- FilterService.checkNullForString = false;

-----------------------
# 3.1.0
- [Vit.Linq] rename namespace ExpressionTree to ExpressionNodes , Filter to FilterRules


-----------------------
# 3.0.3
-  Demo for CustomMethod


-----------------------
# 3.0.1

- static method extension: Queryable_Extensions.ToListAsync 
- [ExpressionTree] CustomMethodAttribute and support custom method convertor


-----------------------
# 3.0.0

- [ExpressionTree] 
    - rename to ConvertToCode and ConvertToData
    - rename QueryAction to CollectionQuery
    - change returnType of ExpressionNode.Lambda to ExpressionNode_Lambda from ExpressionNode
    - rename ValueType to NodeValueType
    - rename Binarys to Binary, Unarys to Unary
    - Queryable.Where With ExpressionNode_Lambda
    - suport get parameter from static property or static field of Class

 - [Filter]
   - refactor FilterConvert to FilterGenerateService


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
- PagedQuery and RangedQuery



-----------------------
# 2.2.23

- support bitwise and other binary operators



