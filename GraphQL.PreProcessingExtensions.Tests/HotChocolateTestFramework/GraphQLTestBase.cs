using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using GraphQL.PreProcessingExtensions.Tests.GraphQL;
using HotChocolate.AspNetCore.Extensions;
using HotChocolate.AspNetCore.Serialization;
using HotChocolate.Execution.Configuration;
using HotChocolate.Resolvers;

namespace GraphQL.PreProcessingExtensions.Tests
{
    /// <summary>
    /// BBernard
    /// Generate an in-memory Test Server for Asp.Net Core!
    /// NOTE: Borrowed from HotChocolate.AspNet.Core tests project from the Core HotChocolate source.
    /// </summary>

    public class GraphQLTestBase
    {
        public GraphQLTestBase(GraphQLTestServerFactory serverFactory)
        {
            ServerFactory = serverFactory;
        }

        protected GraphQLTestServerFactory ServerFactory { get; }

        protected virtual TestServer CreateSimpleTestServer(
            string pattern = "/graphql",
            Action<IRequestExecutorBuilder> testBuilderConfigure = default,
            FieldMiddleware fieldMiddleware = default,
            Action<GraphQLEndpointConventionBuilder> configureConventions = default)
        {
            return ServerFactory.Create(
                services =>
                {
                    services
                        .AddRouting()
                        .AddHttpResultSerializer(HttpResultSerialization.JsonArray);

                    //BBernard
                    //Configure the test server and Load PreProcessedResults Custom Middleware!
                    var graphQLBuilder = services
                        .AddGraphQLServer()
                        .AddQueryType(d => d.Name("Query"))
                        .AddType<HelloWorldResolver>()
                        .AddType<StarWarsCharacterResolver>()
                        .AddType<StarWarsHuman>()
                        .AddType<StarWarsDroid>()
                        .AddMiddlewareForPreProcessedResults();

                    //BBernard
                    //Allow Tests to provide (IoC) custom configuration to the builder
                    //  as needed for custom testing via Middleware!
                    testBuilderConfigure?.Invoke(graphQLBuilder);

                    //BBernard
                    //Allow convenience of just passing in Field Middleware for configuration without
                    //  using an entire builder (reduces ceremonial code in tests).
                    if(fieldMiddleware != null)
                        graphQLBuilder.UseField(fieldMiddleware);
                },
                app =>
                {
                    app
                        .UseRouting()
                        .UseEndpoints(endpoints =>
                        {
                            GraphQLEndpointConventionBuilder builder = endpoints.MapGraphQL(pattern);

                            configureConventions?.Invoke(builder);
                        });
                }
            );
        }
    }
}
