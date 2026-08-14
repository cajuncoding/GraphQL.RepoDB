using System;
using HotChocolate;
using HotChocolate.ResolverProcessingExtensions.Tests.GraphQL;
using Microsoft.Extensions.DependencyInjection;

namespace HotChocolate.ResolverProcessingExtensions.Tests
{
    public class GraphQLHelloWorldTestServer : GraphQLTestServerBase
    {
        public GraphQLHelloWorldTestServer(GraphQLTestServerFactory serverFactory)
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
                        .AddSorting()
                        //We ONLY Add Middleware for testing without Advanced support for Sorting, etc...
                        //  to help determine if ParamsContext has graceful fallback functionality.
                        .AddMiddlewareForResolverProcessedResults();

                    return graphQLBuilder;
                }
            );
        }

    }
}
