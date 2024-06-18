# Linq Example


# 1. InnerJoin with Queryable.SelectMany

``` csharp

// Linq Expresssion
var query =
    from user in userQuery
    from father in userQuery.Where(father => user.fatherId == father.id)
    where user.id > 2
    select new { user, father };

// Lambda Expression
var query = 
  userQuery.SelectMany(
      user => userQuery.Where(father => user.fatherId == father.id)
      , (user, father) => new { user, father }
  )
  .Where(row => row.user.id > 2)
  .Select(row => new { row.user, row.father });

```

# 2. LeftJoin with Queryable.SelectMany

``` csharp

// Linq Expresssion
var query =
    from user in userQuery
    from father in userQuery.Where(father => user.fatherId == father.id).DefaultIfEmpty()
    where user.id > 2
    orderby user.id
    select new { user, father };

// Lambda Expression
var query = 
  userQuery.SelectMany(
      user => userQuery.Where(father => user.fatherId == father.id).DefaultIfEmpty()
      , (user, father) => new { user, father }
  )
  .Where(row => row.user.id > 2)
  .OrderBy(row => row.user.id)
  .Select(row => new { row.user, row.father });

```


# 3. InnerJoin with Queryable.Join

``` csharp

// Linq Expresssion
var query =
    from user in userQuery
    join father in userQuery on user.fatherId equals father.id
    where user.id > 2
    select new { user, father };

// Lambda Expression
var query = 
    userQuery.Join(
        userQuery
        , user => user.fatherId
        , father => father.id
        , (user, father) => new { user, father }
    );

```

# 4. LeftJoin with Queryable.GroupJoin

``` csharp

// Linq Expresssion
var query =
    from user in userQuery
    join father in userQuery on user.fatherId equals father.id into fathers
    from father in fathers.DefaultIfEmpty()
    where user.id > 2
    select new { user, father };

// Lambda Expression
var query =
    userQuery.GroupJoin(
        userQuery
        , user => user.fatherId
        , father => father.id
        , (user, fathers) => new { user, fathers }
    )
    .SelectMany(
        row => row.fathers.DefaultIfEmpty()
        , (row, father) => new { row, father }
    )
    .Where(row2 => row2.row.user.id > 2)
    .Select(row2 => new { row2.row.user, row2.father });

```


# 5. Group with Queryable.GroupBy
``` csharp

// Linq Expresssion
var query =
    from user in userQuery.Where(u => u.id > 2)
    group user by new { user.fatherId, user.motherId } into userGroup
    where userGroup.Key.motherId != null
    orderby userGroup.Key.motherId
    select new { userGroup.Key.fatherId, userGroup.Key.motherId, rowCount = userGroup.Count(), maxId = userGroup.Max(m => m.id) };

// Lambda Expression
var query =
    userQuery
    .Where(u => u.id > 2)
    .GroupBy(user => new { user.fatherId, user.motherId })
    .Where(userGroup => userGroup.Key.motherId != null)
    .OrderBy(userGroup => userGroup.Key.motherId)
    .Select(userGroup => new { userGroup.Key.fatherId, userGroup.Key.motherId, rowCount = userGroup.Count(), maxId = userGroup.Max(m => m.id) })
    ;

```