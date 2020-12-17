using System;
using System.Collections.Generic;
using System.Text;
using HotChocolate;
using HotChocolate.AspNetCore.Extensions;
using HotChocolate.AspNetCore.Serialization;
using HotChocolate.Execution.Configuration;
using HotChocolate.PreProcessingExtensions;
using HotChocolate.PreProcessingExtensions.Tests.GraphQL;
using HotChocolate.Resolvers;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;

namespace HotChocolate.PreProcessingExtensions.Tests
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
                    //Configure the test server and Load PreProcessedResults Custom Middleware!
                    var graphQLBuilder = services
                        .AddGraphQLServer()
                        .AddQueryType(d => d.Name("Query"))
                        .AddType<HelloWorldResolver>()
                        .AddType<StarWarsCharacterResolver>()
                        .AddType<StarWarsHuman>()
                        .AddType<StarWarsDroid>()
                        .AddPreProcessedResultsExtensions();

                    return graphQLBuilder;
                }
            );
        }

    }
}
