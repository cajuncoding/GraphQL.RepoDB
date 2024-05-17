## Overview 
*RepoDb (Unofficial) Extension for Sql Server to simplify Pagination of Queries.*

This library greatly simplifies working with paginated data in Sql Server for most common use cases.
It is built on top of RepoDb and follows the same naming conventions to provide new APIs for executing SQL with
automatic hanlding of both Cursor based pagination (following the GraphQL Relay Spec as [outlined here](https://relay.dev/graphql/connections.htm)) 
as well as Offset based pagination (following HotChocolate GraphQL platform implementation). Though these two implementations
are native to GraphQL they are not limited to only that and can be fully leveraged for any pagination based API over
your data in Sql Server (e.g. even REST based APIs).

This extension pack provides access to key elements such as ready-to-use Paging argument models, Paging result models, etc.
And completely manages the generation of valid Cursors over properly sorted query results.


The reason we want to follow the GraphQL spec -- even when not implementing GraphQL per se -- is because it provides one 
of the most clear guidance on how pagination can be managed flexibly and consistently without each and every API/Team/Developer 
re-creating their own version. And they outline the benefits of Cursor based pagination really well 
[here](https://graphql.org/learn/pagination/#pagination-and-edges):

> In general, we’ve found that cursor-based pagination is the most powerful of those designed. 
> Especially if the cursors are opaque, either offset or ID-based pagination can be implemented using cursor-based 
> pagination (by making the cursor the offset or the ID), and using cursors gives additional flexibility if the 
> pagination model changes in the future. As a reminder that the cursors are opaque and that their format should not be 
> relied upon, we suggest base64 encoding them.

### *Query Limitations*
There are a few limitations on the queries that can be used. Any query that can be executed using 
RepoDb's [_Query_](https://repodb.net/operation/query) APIs -- which use Field args, SortField args, and Where expressions 
via C# code -- should be supported. In addition raw sql is supported similar to using RepoDb's 
[_ExecuteQuery_](https://repodb.net/operation/executequery) as long as the query does not use CTE's or other 
complexities that prevent it from being able to be re-written into a wrapper CTE (which the API does to provide dynamic pagination).

The main workaround to needing to use a complex query with your own CTE's etc. would be to encapsulate & deploy 
that Query as a _**SQL View**_ which can then be used easily via RepoDb's _Query_ API (mapped to a Model) or Sql statement.

### [Buy me a Coffee ☕](https://www.buymeacoffee.com/cajuncoding)
*I'm happy to share with the community, but if you find this useful (e.g for professional use), and are so inclinded,
then I do love-me-some-coffee!*

<a href="https://www.buymeacoffee.com/cajuncoding" target="_blank">
<img src="https://cdn.buymeacoffee.com/buttons/default-orange.png" alt="Buy Me A Coffee" height="41" width="174">
</a>

#### Nuget Package (>=netstandard2.0)
To use this in your project, add the [RepoDb.SqlServer.PagingOperations](https://www.nuget.org/packages/RepoDb.SqlServer.PagingOperations/) 
NuGet package to your project.

## Examples and Use:
### *Cusor Pagination* using Raw SQL
Here is an example of iterating through a large data set using `Cursor` based pagination:

NOTE: RepoDb has built in functionality for [`BatchQuerying`](https://repodb.net/operation/batchquery) as a paginated query feature, however, the current
_Batch_ query API is notably less flexible than true Cursor Based pagination which provides a slice based approach that is 
both more flexible and (subjectively) more intuitive or at least more consistent with Linq Skip/Take paradigm.

```csharp
using var sqlConnection = await CreateSqlConnectionAsync().ConfigureAwait(false);

const int pageSize = 5;
ICursorPageResults<CharacterDbModel> page = null;
do
{
    page = await sqlConnection.ExecutePagingCursorQueryAsync<CharacterDbModel>(
        sql: "SELECT * FROM [dbo].[StarWarsCharacters]",
        orderBy: new[] { OrderField.Descending<CharacterDbModel>(c => c.Id) },
        first: pageSize,
        afterCursor: page?.EndCursor,
        retrieveTotalCount: page?.HasPreviousPage == null
    );

    //Output the Total Count (only once)
    if(page.TotalCount != null)
        TestContext.WriteLine($"Total Record Count = [{page.TotalCount}]");

    foreach (var result in page.CursorResults)
        TestContext.WriteLine($"[{result.Cursor}] ==> ({result.Entity.Id}) {result.Entity.Name}");

} while (page.HasNextPage);
```

Alternatively the RepoDb _Query_ API may also be used...
```csharp
var page = await sqlConnection.PagingCursorQueryAsync<CharacterDbModel>(
    fields: Field.Parse<CharacterDbModel>(c => new { c.Id, c.Name }),
    orderBy: new[] { OrderField.Descending<CharacterDbModel>(c => c.Id) },
    pagingParams: RepoDbCursorPagingParams.ForCursors(
        first: pageSize, 
        afterCursor: page?.EndCursor,
        retrieveTotalCount: page?.HasPreviousPage == null
    ),
    whereExpression: c => c.HomePlanet == "Tatooine"
)
```


### *Offset Pagination*
Here is an example of iterating through a large data set using `Offset` based pagination:

NOTE: RepoDb has built in functionality for `BatchQuerying` as a paginated query feature, however, the current
_Batch_ query API is less flexible than true Offset Based pagination which provides a slice based approach that is 
both more flexible and (subjectively) more intuitive or at least more consistent with Linq Skip/Take paradigm.
```csharp
using var sqlConnection = await CreateSqlConnectionAsync().ConfigureAwait(false);

const int pageSize = 5;
int counter = 0;
IOffsetPageResults<CharacterDbModel> page = null;
do
{
    page = await sqlConnection.ExecutePagingOffsetQueryAsync<CharacterDbModel>(
        sql: "SELECT * FROM [dbo].[StarWarsCharacters]",
        orderBy: new[] { OrderField.Descending<CharacterDbModel>(c => c.Id) },
        skip: page?.EndIndex,
        take: pageSize,
        retrieveTotalCount: page?.HasPreviousPage == null
    );

    //Output the Total Count (only once)
    if (page.TotalCount != null)
        TestContext.WriteLine($"Total Record Count = [{page.TotalCount}]");

    foreach (var result in page.Results)
        TestContext.WriteLine($"[{++counter}] ==> ({result.Id}) {result.Name}");

} while (page.HasNextPage);
```

Alternatively the RepoDb _Query_ API may also be used...
```csharp
var page = await sqlConnection.PagingOffsetQueryAsync<CharacterDbModel>(
    fields: Field.Parse<CharacterDbModel>(c => new { c.Id, c.Name }),
    orderBy: new[] { OrderField.Descending<CharacterDbModel>(c => c.Id) },
    pagingParams: RepoDbCursorPagingParams.ForCursors(
        first: pageSize, 
        afterCursor: page?.EndCursor,
        retrieveTotalCount: page?.HasPreviousPage == null
    ),
    whereExpression: c => c.HomePlanet == "Tatooine"
)
```


### Release Notes v1.1.5:
- Initial release of independent custom extensions for RepoDb to support enhanced Cursor &amp; Offset Paging Query Operations.
- This allows non-GraphQL projects (e.g. normal REST APIs) to more easily implement modern paging (Cursor or Offset) with the RepoDb ORM and SQL Server.
- These extensions have been in use in production applications using GraphQL.RepoDb.SqlServer for a long while, but are now available independently.

### NOTES: 
1. **NOTE:** This is not necessarily the fastest way to paginate of large datasets (e.g. millions of records),
however it is extremely flexible and will work for the vast majority of use cases with tens or hundreds of thousands of records.
And is significantly more simple than manually coding every SQL query to support dynamic pagination along with dynamic sorting, etc. 
3. **DISCLAIMER: Testing has been done on my use-cases and I am actively using it on projects, 
and will update with any findings. And features are being added as this project evolves.**

