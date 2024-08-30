
# Vit.Linq
Vit.Linq 提供两个针对Expression表达式的工具，Filter 和 ExpressionTree    
 _ **Filter** 可以把 FilterRule 和 Expression Predicate 进行相互转换， 由此可以通过 json 数据 对结果集进行动态化筛选。    
 - **ExpressionTree** 实现 ExpressionNode 和 Expression 表达式的相互转换实现数据和代码的转换。    
  > 因为非基本类型Type不能通过数据格式进行传递，所以转换并不是完全百分百互等的，会丢失部分类型信息    
> source address: [https://github.com/Vit-Orm/Vit.Linq](https://github.com/Vit-Orm/Vit.Linq "https://github.com/Vit-Orm/Vit.Linq")    

![](https://img.shields.io/github/license/Vit-Orm/Vit.Linq.svg)  
![](https://img.shields.io/github/repo-size/Vit-Orm/Vit.Linq.svg)  ![](https://img.shields.io/github/last-commit/Vit-Orm/Vit.Linq.svg)  
 

| Build | NuGet |
| -------- | -------- |
|![](https://github.com/Vit-Orm/Vit.Linq/workflows/ki_devops3_build/badge.svg) | [![](https://img.shields.io/nuget/v/Vit.Linq.svg)](https://www.nuget.org/packages/Vit.Linq) ![](https://img.shields.io/nuget/dt/Vit.Linq.svg) |



# FilterRules
FilerRule 可以表达 逻辑组合 （And / Or / Not ） 和 基本的逻辑判断 （如 数值比较 / 字符串匹配 / 非空判定 等），如下为所有功能：
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
安装引用的包:
``` bash
dotnet add package Vit.Linq
dotnet add package Vit.Core
```

创建 console project 并按如下修改 Program.cs
> code address: [Program.cs](https://github.com/Vit-Orm/Vit.Linq/tree/master/test/Vit.Linq.Console/Program.cs)    
``` csharp
using Vit.Core.Module.Serialization;
using Vit.Linq.Filter.ComponentModel;
using Vit.Linq;

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


## FilterRule 的格式
> FilterRule 为json格式的数据，其中 condition 可以为 `and`, `or`, `not`, `notand`, `notor`(not or 的组合)    
> `rules` 可以为嵌套的 FilterRule。    
> `field` 可以为嵌套属性, 例如 `id` , `job.name` .    
> `operator` 可以为 `IsNull`, `IsNotNull`, `In`, `NotIn`, `=`, `!=`, `>`, `>=`, `<`, `<=`, `Contains`, `NotContain`, `StartsWith`, `EndsWith`, `IsNullOrEmpty`, `IsNotNullOrEmpty` 等    
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
ExpressionNode 可以通过 ExpressionNode(Data) 和 Expression(Code) 的相互转换达到数据和代码相互转换的功能, 支持所有查询相关的Expression(不支持Expression.Assign等功能)

## Example
安装引用的包:
``` bash
dotnet add package Vit.Linq
dotnet add package Vit.Core
```

创建 console project 并按如下修改 Program2.cs
> code address: [Program2.cs](https://github.com/Vit-Orm/Vit.Linq/tree/master/test/Vit.Linq.Console/Program2.cs)    
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



