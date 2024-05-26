
complex-query-operators https://learn.microsoft.com/en-us/ef/core/querying/complex-query-operators



--------------
# cur


    case "Where__":
        {
            var source = ReadStream(arg, call.arguments[0]);
            var predicateLambda = call.arguments[1] as ExpressionNode_Lambda;
            var where = ReadWhere(arg, source, predicateLambda);


            JoinedStream joinedStream = source as JoinedStream;
            if (joinedStream == null)
            {
                joinedStream = new JoinedStream(NewAliasName()) { left = source };
            }

            if (joinedStream.where == null)
            {
                joinedStream.where = where;
            }
            else
            {
                joinedStream.where = ExpressionNode.And(left: joinedStream.where, right: where);
            }
            return joinedStream;
        }



## sql calc:      where a.name + '22' = 'lith22'
## sql Func  
## sql support case when




--------------
# TODO


# distinct

## ExecuteDelete

## ExecuteUpdateToSql

## ToArray  First| FirstOrDefault|...


##   where (`t0`.`id` + 1 = 4) and (`t0`.`fatherId` = cast(5 as integer))





--------------
# done

## ExecuteUpdate
## sql calc:      set name = a.name + '22' 

## Insert
## Update
## Delete

## extensions for IQueryable.ToSql

## remove nullable convert in sql
## limit (Take Skip)
## OrderBy
## Count
## select return null entity if not exist

## left join

## where
 "= null"  ->   "is null" ,    "!=null" -> "is not null"   
  is null , check by primary key