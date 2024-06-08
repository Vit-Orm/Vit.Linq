
complex-query-operators https://learn.microsoft.com/en-us/ef/core/querying/complex-query-operators
sqlite/transactions  https://learn.microsoft.com/en-us/dotnet/standard/data/sqlite/transactions

--------------
# cur

# support Mysql



# sqlite upgrade to latest version
# remove depency of Dapper
# try to make it clean


# support SqlServer
# support ElasticSearch
# support ClickHouse


# DbContext.QueryProcedure<Entity>(arg)

--------------
# TODO

# sqlite nested transaction
# Save SaveRange



#region #4 cross database join
{
    var dbContext = DataSource.BuildInitedDatabase(System.Reflection.MethodBase.GetCurrentMethod()?.Name ?? "_");
    var users2 = dbContext.Query<User>();

    var query = (from user in users
                from father in users2.Where(father => user.fatherId == father.id).DefaultIfEmpty()
                select new
                {
                    user,
                    father
                });  

    var userList = query.ToList();
    Assert.AreEqual(4, userList.Count);
    Assert.AreEqual(3, userList.First().user.id);
}
#endregion

##   where (`t0`.`id` + 1 = 4) and (`t0`.`fatherId` = cast(5 as integer))

# will cause column mismatch if using inner select like:
select `t2`.`id`,`t2`.`name`,`t2`.`birth`,`t2`.`fatherId`,`t2`.`motherId`,`t3`.`name`
 from 
 (
	 select `t0`.`id`,`t0`.`name`,`t0`.`birth`,`t0`.`fatherId`,`t0`.`motherId`,`t1`.`id`,`t1`.`name`,`t1`.`birth`,`t1`.`fatherId`,`t1`.`motherId`
	 from `User` as t0
	 left join `User` as t1 on `t0`.`fatherId` = `t1`.`id`
 ) as t2
 left join `User` as t3 on `t2`.`motherId` = `t3`.`id`

--------------
# done


## sql Func
## sql like
# join with Queryable.Join or GroupJoin

# GroupBy

# distinct
# sql Contains

# Add AddRange
# Get(keyValue)
# Update UpdateRange
# ExecuteDelete
# DeleteByKey DeleteByKeys
# Delete DeleteRange


## ToArray First FirstOrDefault Last LastOrDefault

## ExecuteUpdate
## sql calc:      set name = a.name + '22' 
## sql calc:      where a.name + '22' = 'lith22'

## Insert
## Update
## Delete

 

## remove nullable convert in sql
## limit (Take Skip)
## OrderBy
## Count
## select return null entity if not exist

## left join

## where
 "= null"  ->   "is null" ,    "!=null" -> "is not null"   
  is null , check by primary key