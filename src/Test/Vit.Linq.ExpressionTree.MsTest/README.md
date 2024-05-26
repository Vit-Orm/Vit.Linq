
# TODO: 

# TotalCount  : Count without take and skip

# CollectionStream. Select
TODO: .OrderBy().Skip().Take()

# join
select u.id,u.name, u.birth,u.fatherId ,u.motherId, father.name,mother.name
from `User` u
left join `User` father on u.fatherId = father.id 
inner join `User` mother on u.motherId = mother.id
where u.id>1 
limit 1,5;

# group by
select WarpWarehouseID 
from location l where WarpWarehouseID !=2300
group by l.WarpWarehouseID having WarpWarehouseID>10


# QueryAction

# LeftJoin
# InnerJoin

# SelectMany

# GroupBy 


# support ExpressionType.Quote

# Deserialize


-------------
release note: 

# Join 
# Select


# fix parameterName duplicate issue
# try convert constant value as parameters

# Any
      var persons = new List<Person> { new Person { id = 2 }, new Person { id = 3 } }.AsQueryable();
                    query = query
                        .Where(m => persons.Any(p => p.id == m.id))