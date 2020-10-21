# (Unofficial) HotChocolate v11 Extensions for working with Micro-ORM instead of IQueryable

## Overview
TODO...

### Currently Supported Functionality
* Sorting
* Cursor Paging

### Pending Functionality (TODO:)
* Filtering 
  * *(lower priority use case for me, but will be useful)*
* Offset Paging 
  * *(not a use case for me currently, but will work on it eventually)*

## Demo Site (Star Wars)
This project contains a clone of the HotChocolate GraphQL *Star Wars* example project (Pure Code First version)
running as an AzureFunctions app and mildly updated to use the new v11 API.

All Queries have been updated to do manual processing within the Resolver (using Linq) to simulate
the use of lower layer logic for Sorting, Paging, etc. as you would with a micro-orm for 
working with an external data source (e.g Azure Sql Server with RepoDb).

### NOTES: 
1. **NOTE:** This is not necessarily only, nor do I claim it's the best approach to working
with GraphQL or HotChocolate specifically.  
2. However in most enterprises it's very common to have constraints such as:
  * Existing logic that needs to be re-used from business layer, and/or existing service or 
  * repository classes.
  * Properly enabling IQueryable for exist code can be extra-ordinarily complex.
  * The heavy weight ORM(s) that support IQueryable may not be an option for various reasons (e.g.
it may be incongruent with existing tech. stack, or tech. team).
  * Many use-cases require more bare-metal control over Sql queries actual execution that 
is only availalbe in a lighter weight (bare-metal) ORM like Dapper or RepoDb.
  * Architecturally, you need to maintain a greater decoupling of your processing logic from
being depending on HotChocolate post-processing of IQueryable yet still have Sorting/Paging, etc.
middleware as part of v11. :-)
2. **WARNING: Very Limited Testing has been done on this but I am actively using it on projects, 
and will update with any findings.**

## Goals

* To provide a working approach that simplifies the use of HotChocolate with lower level micro-orms
and/or bare-metal Sql (database) mechanisms, while preventing duplicate post-processing by 
HotChocolate existing processes. 
* At the same time I want to use as much of the existing functionality of HotChocolate for 
dynamically generating schema elements, arguments, etc. for Sorting, Paging, etc.
  * I do not want to have to create more code/classes/ceremonial elements than necessary.
* And as a critical benefit, all pre-processed result extensions should not interfere with existing
out-of-the-box functionality except when we explicitly want it to (via our Decorators and Conventions);
  * Otherwise in all other cases the original HotChocolate behavior should work as expected - 
correctly processing both IQueryable & IEnumerable results.
* Keep this code encapsulated (to the extent possible) so that the data processing isn't 
tightly coupled with HotChocolate by way of dependency on functionality (IQueryable) or by
dependency on types (classes, etc.)
switching to the official Middleware will be as
painless and simple as possible *(with a few design assumptions aside)*.
* Provide at least one layer of abstraction between HotChocolate and the RepoDb specific Extensions;
which is maintained in the HotChocolate.PreProcessedExtensions project (e.g. using this only a set of 
helpers for Dapper could be similarly created).
* Have NO requirement for special inerfaces to be defined on the Entity Model's themselves, mitigate
this by using Decorator classes/interfaces only as needed.


## Implementation:
TODO...

/NOTE: The HotChocolate default behaviour will occur anytime a normal IEnumerable or IQueryable result
is returned. This ia accomplished by ensuring that the new Sorring/Paging Providers have 
"right of first refusal" for handling, but will always default back to the existing HotChocolate Queryable 
implementations.


## Key Elements:
### Startup Configuration
1. The following Middleware initializer may be added into the Startup.cs to enable these extensions.
  * All other elements of HotChocolate initialization are the same using the v11 API. 
```csharp
        //Initialize AzureFunctions Executor Proxy here...
        builder.Services
            .AddGraphQLServer()
            .AddQueryType<MarketingEventsQueryResolver>()
            .AddPreProcessedResultsExtensions()
```

2. Now you can Dependency Inject the new **IParamsContext** into your Resolvers:
This greatly simplifies access to key parameters such as selection names, sort arguments, and cursor
paging arguments.

NOTE: The selection names will map to your Sql Select properties names from your Entity model, but these
may not be the same as your Database fields.  The same applies for the sort arguments.  These likely
need to be mapped to the real field names of the underlying data source.  RepoDb makes this easy, but
other orm's may be different -- but at least now, these are surfaced as defined in the GraphQL Schema
is an easy to consume form.

```csharp
TODO...
```



## Disclaimers:
* Subscriptions were disabled in the example project due to unknown supportability in a 
serverless environment. 
  * The StarWars example uses in-memory subscriptions which are incongruent with the serverless
paradigm of AzureFunctions.

## Credits:
* The [HotChocolage Slack channel](https://join.slack.com/t/hotchocolategraphql/shared_invite/enQtNTA4NjA0ODYwOTQ0LTViMzA2MTM4OWYwYjIxYzViYmM0YmZhYjdiNzBjOTg2ZmU1YmMwNDZiYjUyZWZlMzNiMTk1OWUxNWZhMzQwY2Q)
was helpful for searching and getting some feedback to iron this out effectively.
