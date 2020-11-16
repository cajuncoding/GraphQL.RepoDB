## Overview 
*(Unofficial) HotChocolate v11 Extension Pack for working with Micro-ORM(s) and encapsulated data access (instead of IQueryable).*

*Note: Updated Repo & Package names to eliminate conflicts with the core HotChocolate packages.*

### *GraphQL.PreprocessingExtensions*
A set of extensions for working with HotChocolate GraphQL and Database access with micro-orms such as RepoDb (or Dapper).  This extension pack provides access to key elements such as Selections/Projections, Sort arguments, & Paging arguments in a significantly simplified facade so this logic can be leveraged in the Serivces/Repositories that encapsulate all data access (without dependency on IQueryable and execution outside of the devs control).

#### Nuget Package (>=.netcoreapp3.0)
To use this in your project, add the [GraphQL.PreprocessingExtensions](https://www.nuget.org/packages/GraphQL.PreProcessingExtensions/) 
NuGet package to your project and wire up your Starup  middleware and inject / instantiate params in your resolvers as outlined below...

### *GraphQL.RepoDb.SqlServer*
A set of extensions for working with HotChocolate GraphQL and RepoDb as the data access micro-orm without dependency on IQueryable.  This enables fully encapsulated control over SQL queries in every way within a Service or Repository layer of your application. This extension pack provides a significantly simplified facade to access critial elements such as Selections/Projections, Sort arguments, & Paging arguments with support for mapping them to Models using built in RepoDb functionality.  It also leverages RepoDb to provide a generic, Relay spec compatible, cursor pagination/slice query api for Sql Server.


## Work in Progress...
### Pending:
1. Improved support for field selections with Mutations.
2. Support for Default Paging options use in IParamsContext... 
3. Offset Paging support in the Facade is pending....
4. Update Implementation summary detais below in README...

### Planned:
1. Support for enhanced ability to work with Dynamic Filtering (WHERE clause) arguments; support will be added as time permits.

### Completed:
1. Generic facade for pre-processed results to safely bypass the HotChocolate out-of-the-box pipeline (IQueryable dependency) for Sorting & Paging; eliminatues redundant processing and possilby incorrect results from re-processing what has already been 'pre-processed'.
2. Supports encapsulated service/repository pattern whereby all data retrieval is owned in the same portable layer, and not dependent on HotChocolate internal procesing via IQueryable. 
3. Provides abstraction facade with *HotChocolate.PreprocessingExtensions* package that can be used for any micro-orm.
4. Implemented RepoDb on top of HotChocolate.PreprocessingExtensions, as a great primary DB interface with helper classes for mapping Selections from GraphQL to DB layer: (GraphQL Schema names -> Model properties -> DB Column names).
5. Supports abstracted facade for: 
   - Projections of Selection (SELECT X, Fields) down to the Repository Layer and therefore down to the SQL Queries themselves via RepoDb -- works correctly with GraphQL Objects (classes), and now GraphQL Interfaces with query fragments (C# interfaces) too!  And supports correct GraphQL Schema to Class property mapping.
   - Support for Sorting arguments down to the Repository/Service layer & into Sql queries via RepoDb -- with full GraphQL Schema to Class property mapping.
   - Support for Cursor based Pagination arguments down to the the Repository/Service layer & into Sql queries via RepoDb -- Relay spec cursors are fully implemented via Sql Server api extensions to RepoDb.
6. Implemented configuration based control over Projection Dependencies and Pure Code First Attribute to simplify this -- so if a child or virtual field resolver needs a field of the parent, but it wasn't actually part of the selection from the client's query, it is added to the Selections if/when it is necessary.
7. Fixed/Changed repo & package names to address conflicts with HotChocolate core packages.


## Demo Site (Star Wars)
This project provides multiple versions of the HotChocolate GraphQL *Star Wars* example project using the Pure Code First approach.  
Each of the examples are setup to run as an AzureFunctions app and updated/enhanced to use the new v11 API along with example cases 
for the various features of this package -- CharacterQueries and CharacterRepository now all use RepoDb and Preprocessing extensions 
to push logic down to be encapsulated in the data access layer.

Two versions of the Example Project included:
1. **StarWars-AzureFunctions:** A version using Pre-processing extensions only along with in-memory processing with Linq over IEnumerable, 
but all encapsulated in the Query/Repository layer and no longer using the IQueryable interface.  This simulates the use of lower layer logic
for Sorting, Paging, etc. as you would with a micro-orm for working with an external data source, and helps show how any micro-orm or other
functionality can now leverage the simplified facade to HotChocolate provided by *HotChocolate.PreprocessingExtensions*.
2. **StarWars-AzureFunctions-RepoDb:** A version that does the above but implements RepoDb as a fully fledged micro-orm implemenation. 
And illustrates a number of related features to show its use in multiple typs of Resolvers (Attribute, and virtual field resolvers), with and without ProjectionDependencies, nested Paging and Sorting, etc. with all logic encapsulated in the Query
and Repository layer with no dependency on IQueryable.
   - As a fully integrated DB example, the schema & sample data has been scripted and provided in:
      **DatabaseScripts/CreateStarWarsSchema.sql**

### NOTES: 
1. **NOTE:** This is not necessarily the only, nor do I claim it's the best, approach to working
with GraphQL or HotChocolate specifically -- but it greatly simplifies the amount of work and effort needed to
use it in with a Service or Repository pattern of encapsulated data access.
2. In most enterprises it's very common to have constraints such as:
   - Existing logic that needs to be re-used from business layer, and/or existing service or 
   - repository classes.
   - Properly enabling IQueryable for exist code can be extra-ordinarily complex.
   - The heavy weight ORM(s) that support IQueryable may not be an option for various reasons (e.g.
it may be incongruent with existing tech. stack, or tech. team).
   - Many use-cases require more bare-metal control over Sql queries actual execution that 
is only availalbe in a lighter weight (bare-metal) ORM like Dapper or RepoDb.
   - Architecturally, you need to maintain a greater decoupling of your processing logic from
being depending on HotChocolate post-processing of IQueryable yet still have Sorting/Paging, etc.
middleware as part of v11. :-)
3. **DISCLAIMER: Limited Testing has been done on this but I am actively using it on projects, 
and will update with any findings. And features are being added as this project evolves.**

## Goals

- To provide a working approach that simplifies the use of HotChocolate with lower level micro-orms
and/or bare-metal Sql (database) mechanisms, while preventing duplicate post-processing by 
HotChocolate existing processes. 
- At the same time I want to use as much of the existing functionality of HotChocolate for 
dynamically generating schema elements, arguments, etc. for Sorting, Paging, etc.
  - I do not want to have to create more code/classes/ceremonial elements than necessary.
- And as a critical benefit, all pre-processed result extensions should not interfere with existing
out-of-the-box functionality except when we explicitly want it to (via our Decorators and Conventions);
  - Otherwise in all other cases the original HotChocolate behavior should work as expected - 
correctly processing both IQueryable & IEnumerable results.
- Keep this code encapsulated (to the extent possible) so that the data processing isn't 
tightly coupled with HotChocolate by way of dependency on functionality (IQueryable) or by
dependency on types (classes, etc.)
switching to the official Middleware will be as
painless and simple as possible *(with a few design assumptions aside)*.
- Provide at least one layer of abstraction between HotChocolate and the RepoDb specific Extensions;
which is maintained in the HotChocolate.PreProcessedExtensions project (e.g. using this only a set of 
helpers for Dapper could be similarly created).
- Have NO requirement for special inerfaces to be defined on the Entity Model's themselves, mitigate
this by using Decorator classes/interfaces only as needed.


## Implementation:
*TODO... add implementation summary*

*NOTE: The HotChocolate default behaviour will occur anytime a normal IEnumerable or IQueryable result
is returned. This ia accomplished by ensuring that the new Sorring/Paging Providers have 
"right of first refusal" for handling, but will always default back to the existing HotChocolate Queryable 
implementations.*

## Configuration and Use:
### Startup Configuration - HotChocolate.PreprocessingExtensions
1. Add the following initializer into the Startup.cs to enable these extensions.
   - All other elements of HotChocolate initialization are the same using the v11 API. 
```csharp
        builder.Services
            .AddGraphQLServer()
            .AddQueryType<YourQueryResolverClass>()
            //This Below is the initializer to be added...
            //NOTE: This Adds Sorting & Paging providers/conventions by default!  Do not AddPaging() & 
            //      AddSorting() in addition to '.AddPreProcessedResultsExtensions()', or the HotChocolate 
            //      Pipeline will not work as expected!
            .AddPreProcessedResultsExtensions()
```
### Simplified Facade for key elements of the request [GraphQLParams]
2. Now you can Dependency Inject the new **IParamsContext** into your Resolvers:
This greatly simplifies access to key parameters such as selection names, sort arguments, and cursor
paging arguments.
    * *NOTE: The selection/projection names will map to the GraphQL schema names and the class model properties/members
of the entity model, but these may not be the same as your Database fields for SELECT clauses. The same applies for the 
sort arguments.  These likely need to be mapped to the real field names of the underlying data source; a micro-orm like RepoDb makes this easy, but
other orm's may be different -- but at least now, these are surfaced as defined in the GraphQL Schema
is an easy to consume form.*

```csharp
        [UsePaging]
        [UseSorting]
        [GraphQLName("characters")]
        public async Task<IPreProcessedCursorSlice<ICharacter>> GetCharactersPaginatedAsync(
            [Service] ICharacterRepository repository,
            //This facade is now injected by the Pre-Processing extensions middleware...
            [GraphQLParams] IParamsContext graphQLParams
        )
        {

```
### Putting it all Together...
3. Here's a full overview of a Resolver and what these packages make significantly easier:
   * NOTE: This sample uses dynamic injection for elegant/easy consumtion of the IParamsContext, but it can also be instantiated (see below).

```csharp
//Sample Extraced from StarWars-AzureFunctions-RepoDb example project:
namespace StarWars.Characters
{
    [ExtendObjectType(Name = "Query")]
    public class CharacterQueries
    {
        [UsePaging]
        [UseSorting]
        [GraphQLName("characters")]
        public async Task<IPreProcessedCursorSlice<ICharacter>> GetCharactersPaginatedAsync(
            //This is just our Repository being injected as a normal Service configured in Startup.cs
            [Service] ICharacterRepository repository,
            //This facade is now injected by the Pre-Processing extensions middleware...
            [GraphQLParams] IParamsContext graphQLParams
        )
        {
            //With HotChocolate.RepoDb.SqlServer package we can easily map the inputs from GraphQL
            // IParamsContext into the RepoDb specific helper for DB mapping:
            // NOTE: Other Micro ORMs (e.g. Dapper) may need similar mapping capabiliteis to be built
            //       and are not yet provided here in this project...
            var repoDbParams = new GraphQLRepoDbMapper<CharacterDbModel>(graphQLParams);

            //********************************************************************************//
            // Push the Selections, SortArgumetns, and Paging Arguments down to our Repository
            //  layer as RepoDb specific mapped models (Field, OrderField, etc.
            // Using the exentions for Cursor Pagination our Repository API reuturns
            //   a 'Page Slice' model.
            var charactersSlice = await repository.GetPagedCharactersAsync(
                repoDbParams.SelectFields,
                repoDbParams.SortOrderFields ?? OrderField.Parse(new { name = Order.Ascending })
                repoDbParams.PagingParameters
            );

            //Now With a valid Page/Slice we can return a PreProcessed Cursor Result so that
            //  it will not have any additional post-processing in the HotChocolate pipeline!
            //NOTE: Filtering can be applied but it will ONLY be applied to the results we 
            //      are now returning because this would normally be pushed down to the 
            //      Sql Database layer also (pending support).
            return charactersSlice.AsPreProcessedCursorSlice();
            //*******************************************************************************//
        }
    }
}

```

4. If you aren't using Pure Code First and/or just need access to the IParamsContext anywhere else, it can be easily
 instantiated anytime you have a valid IResolverContext from HotChocolate:
   * NOTE: This sample uses dynamic injection for elegant/easy consuming of the IParamsContext, but it can also be instantiated (see below).

```csharp
    public class QueryResolverHelpers
    {
        public SomeResult DoSomethingWithTheResovlerContext(IResolverContext resolverContext)
        {
                var paramsContext = new GraphQLParamsContext(resolverContext);
                
                ...... now you can work with selections, sort args, etc. easily.....
        {
```

5. A Common use case that will occur is that a GraqhQL query may requested a realated child entity but not
    necessarily request the dependenty fields on the Parent entity that are required to correctly retrieve the
    child/related data.  In a Pure Code First implementation, this can be handled easily by defining dependencies with the [PreProcessingParentDepdencies(...)]
    attribute:
   * This will enforce the fact that anytime this Field is requested then the Selections List
        will have the dependent Selections (as defined on the Pure Code First model), and automatically
        add it ot the list of Projection/Selection fields when requested via `paramsContext.GetSelectFields()`.
   * With this in place you don't have to worry about these dependencies or the ceremonial code to handle
        them in all of your field resolvers.
   * You can also configure a dependency for any Field via the IObjectFieldDescriptor.ConfigureContextData() method
        and the custom extension provided by this project as shown below.

##### PreProcessingDependencies - Pure Code First
```csharp
    //Here we define an extension to the Human Query and expose a 'droids' field via our virtual resolver.
    [ExtendObjectType(nameof(Human))]
    public class HumanFieldResolvers
    {
        //However, we MUST have the Id field of the parent entity `Character.Id` as part of the original
        //  selection in order to get related droid data!  This is done by defining a dependency here
        //  with the [PreProcessingDependencies(....)] attribute; we state that this resolver is dependent
        //  on the Character entity's Id field!
        //NOTE: The original resolver for the parent Character entities will know about this dependency
        //       automatically (auto-magically) because the `Id` field will be included in the selection fields,
        //       anytime the 'droids' field is requested, due to this dependency, and will be readily available
         //      field when the parent resolver calls paramsContext.GetSelectFields().
        //NOTE: You can specify any number of dependencys (as string names) in the one attribute via the params aray.
        [GraphQLName("droids")]
        [PreProcessingParentDependencies(nameof(ICharacter.Id))]
        public async Task<IEnumerable<Droid>> GetDroidsAsync(
            [Service] ICharacterRepository repository,
            [Parent] ICharacter character
        )
        {
            //NOW we can rely on the fact that Character.Id won't be null because the parent Resolver
            //      had it as a field to be selected and populated.
            //NOTE: Error checking isn't a bad idea anyway...
            var friends = await repository.GetCharacterFriendsAsync(character.Id);
            var droids = friends.OfType<Droid>();
            return droids;
        }
    }
```

##### PreProcessingDependencies - Code First (Manually wire-up via HotChocolate Configuration)
```csharp
public class HumanType : ObjectType<Human>
{
    protected override void Configure(IObjectTypeDescriptor<Human> descriptor)
    {
        descriptor.Name("human");
        descriptor.Field(t => t.Name).Type<NonNullType<StringType>>();
        descriptor.Field(t => t.Friends).Name("friends")
            .ConfigureContextData(d =>
            {
                //Manually define a Selection/Projection dependency on the "Id" field of
                //  the parent entity so that it is always provided to the parent resolver.
                //This helps ensure that the value is not null in our resolver anytime "friends"
                //  is part of the selection, and the parent "Id" field is not.
                d.AddPreProcessingParentProjectionDependencies(nameof(Human.Id));
            })
            .Resolver(ctx =>
            {
                var repository = ctx.Service<IRepository>();
                var parentHumanId = ctx.Parent<Human>().Id;
                return repository.GetHuman();
            });
    }
}
```

## Disclaimers:
- Subscriptions were disabled in the example project(s) due to unknown supportability in a serverless environment. 
  - The StarWars example uses in-memory subscriptions which are incongruent with the serverless paradigm of AzureFunctions.

## Credits:
- The [HotChocolage Slack channel](https://join.slack.com/t/hotchocolategraphql/shared_invite/enQtNTA4NjA0ODYwOTQ0LTViMzA2MTM4OWYwYjIxYzViYmM0YmZhYjdiNzBjOTg2ZmU1YmMwNDZiYjUyZWZlMzNiMTk1OWUxNWZhMzQwY2Q)
was helpful for searching and getting some feedback to iron this out effectively.
