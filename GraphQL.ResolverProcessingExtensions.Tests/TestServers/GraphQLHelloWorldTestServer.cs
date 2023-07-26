using System;
using System.Collections.Generic;
using System.Text;
using HotChocolate;
using HotChocolate.AspNetCore.Extensions;
using HotChocolate.AspNetCore.Serialization;
using HotChocolate.Execution.Configuration;
using HotChocolate.ResolverProcessingExtensions;
using HotChocolate.ResolverProcessingExtensions.Tests.GraphQL;
using HotChocolate.Resolvers;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.TestHost;
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
