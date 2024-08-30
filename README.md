
# Vit.Linq
Vit.Linq provides two tools for handling Expressions: Filter and ExpressionTree.    
- **Filter** can convert between FilterRule and Expression Predicate, allowing for dynamic filtering of result sets using JSON data.    
- **ExpressionTree** facilitates the conversion between ExpressionNode and Expression, enabling transformations between data and code.    
  > Note: Since non-primitive types cannot be transmitted via data formats, the conversion may not be fully equivalent, and some type information might be lost.    
> source address: [https://github.com/Vit-Orm/Vit.Linq](https://github.com/Vit-Orm/Vit.Linq "https://github.com/Vit-Orm/Vit.Linq")    

![](https://img.shields.io/github/license/Vit-Orm/Vit.Linq.svg)  
![](https://img.shields.io/github/repo-size/Vit-Orm/Vit.Linq.svg)  ![](https://img.shields.io/github/last-commit/Vit-Orm/Vit.Linq.svg)  
 

| Build | NuGet |
| -------- | -------- |
|![](https://github.com/Vit-Orm/Vit.Linq/workflows/ki_devops3_build/badge.svg) | [![](https://img.shields.io/nuget/v/Vit.Linq.svg)](https://www.nuget.org/packages/Vit.Linq) ![](https://img.shields.io/nuget/dt/Vit.Linq.svg) |




# FilterRules
FilterRule can express logical combinations (And / Or / Not) and basic logical evaluations (such as numerical comparisons / string matching / null checks, etc.). The complete set of features is as follows:
  - And
  - Or
  - Not
  - NotAnd
  - NotOr
  - Logical Judgement
    - [object] is null (==null) / is not null (!=null)
    - [numeric] compare: ==  !=  >  >=  <  <=
    - [string] compare: Contains NotContain StartsWith EndsWith IsNullOrEmpty IsNotNullOrEmpty
    - [array] contains: In / NotIn
    - custom operator

## Example

Install necessary packages:
``` bash
dotnet add package Vit.Linq
dotnet add package Vit.Core
```

Create console project and edit Program.cs
> code address: [Program.cs](https://github.com/Vit-Orm/Vit.Linq/tree/master/test/Vit.Linq.Console/Program.cs)    
``` csharp
using Vit.Core.Module.Serialization;
using Vit.Linq;
using Vit.Linq.FilterRules.ComponentModel;

namespace App
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var users = new[] { new { id = 1, name = "name1" }, new { id = 2, name = "name2" } };

            var strRule = "{\"field\":\"id\",  \"operator\": \"=\",  \"value\": 1 }";
            var rule = Json.Deserialize<FilterRule>(strRule);

            var result = users.AsQueryable().Where(rule).ToList();

            var count = result.Count;
        }
    }
}


```


## FilterRule Format
> FilterRule is JSON-formatted data where the condition can be `and`, `or`, `not`, `notand`, or `notor` (a combination of `not` and `or`).   
> `rules` can be nested FilterRules.   
> `field` can be a nested property, such as `id` or `job.name`.   
> `operator` can be one of the following: `IsNull`, `IsNotNull`, `In`, `NotIn`, `=`, `!=`, `>`, `>=`, `<`, `<=`, `Contains`, `NotContain`, `StartsWith`, `EndsWith`, `IsNullOrEmpty`, `IsNotNullOrEmpty`, etc.    
``` json
{
  "condition": "and",
  "rules": [
    {
      "field": "job.name",
      "operator": "!=",
      "value": "name987_job1"
    },
    {
      "field": "name",
      "operator": "IsNotNull"
    },
    {
      "field": "name",
      "operator": "NotIn",
      "value": [
        "name3",
        "name4"
      ]
    }
  ]
}
```


# ExpressionNodes
ExpressionNode enables the transformation between ExpressionNode (data) and Expression (code), allowing for data and code interchangeability. It supports all query-related expressions (excluding functionalities like Expression.Assign).

## Example
Install necessary packages:
``` bash
dotnet add package Vit.Linq
dotnet add package Vit.Core
```

Create console project and edit Program.cs
> code address: [Program.cs](https://github.com/Vit-Orm/Vit.Linq/tree/master/test/Vit.Linq.Console/Program2.cs)    
``` csharp
using Vit.Core.Module.Serialization;
using Vit.Linq;
using Vit.Linq.ExpressionNodes;

namespace App
{
    internal class Program2
    {
        static void Main2(string[] args)
        {
            var users = new[] { new User(1), new User(2), new User(3), new User(4) };
            var query = users.AsQueryable();

            var queryExpression = users.AsQueryable().Where(m => m.id > 0).OrderBy(m => m.id).Skip(1).Take(2);

            #region #1 Expression to ExpressionNode (Code to Data)
            var node = ExpressionConvertService.Instance.ConvertToData_LambdaNode(queryExpression.Expression);
            var strNode = Json.Serialize(node);
            #endregion

            #region #2 ExpressionNode to QueryAction
            var queryAction = new Vit.Linq.ExpressionNodes.Query.QueryAction(node);
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

```











Examples:  
- [FilterRules](https://github.com/Vit-Orm/Vit.Linq/tree/master/test/Vit.Linq.MsTest/FilterRules/Filter_TestBase.cs)    
- [ExpressionNodes](https://github.com/Vit-Orm/Vit.Linq/tree/master/test/Vit.Linq.ExpressionNodes.MsTest)    




