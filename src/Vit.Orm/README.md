
complex-query-operators https://learn.microsoft.com/en-us/ef/core/querying/complex-query-operators



--------------
# cur

CREATE TABLE `user` (
  `id` int NOT NULL ,
  `name` varchar(100) DEFAULT NULL,
  `birth` date DEFAULT NULL,
  `fatherId` int DEFAULT NULL,
  `motherId` int DEFAULT NULL,
  PRIMARY KEY (`id`)
) ;


INSERT INTO `user` (id,name,birth,fatherId,motherId) VALUES
	 (1,'u1','2021-01-01',5,6),
	 (2,'u2','2021-01-02',5,6),
	 (3,'u3','2021-01-03',5,6),
	 (4,'u4','2021-01-04',NULL,NULL),
	 (5,'uF','2021-01-05',NULL,NULL),
	 (6,'uM','2021-01-06',NULL,NULL);




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