using System;
using HotChocolate.ResolverProcessingExtensions.Tests.GraphQL;
using HotChocolate.Types.Pagination;
using Microsoft.Extensions.DependencyInjection;
using StarWars.Characters;

namespace HotChocolate.ResolverProcessingExtensions.Tests
{
    public class GraphQLStarWarsTestServer : GraphQLTestServerBase
    {
        public GraphQLStarWarsTestServer(GraphQLTestServerFactory serverFactory)
        {
            this.Server = CreateTestServer(
                serverFactory,
                servicesConfigure: services =>
                {
                    //BBernard
                    //Configure the test server and Load ResolverProcessedResults Custom Middleware!
                    var graphQLBuilder = services
                        .AddGraphQLServer()
                        .AddQueryType(d => d.Name("Query"))
                        .AddType<HelloWorldResolver>()
                        .AddType<StarWarsCharacterResolver>()
                        .AddType<StarWarsHuman>()
                        .AddType<StarWarsDroid>()
                        .AddTypeExtension<HumanFieldResolvers>()
                        .AddSorting()
                        .SetPagingOptions(new PagingOptions()
                        {
                            DefaultPageSize = 5,
                            IncludeTotalCount = true,
                            MaxPageSize = 10
                        })
                        .AddResolverProcessedResultsExtensions();

                    return graphQLBuilder;
                }
            );
        }

    }
}
