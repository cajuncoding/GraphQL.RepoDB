using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using HotChocolate.PreProcessingExtensions.Tests.GraphQL;
using HotChocolate.AspNetCore.Extensions;
using HotChocolate.AspNetCore.Serialization;
using HotChocolate.Execution.Configuration;
using HotChocolate.Resolvers;

namespace HotChocolate.PreProcessingExtensions.Tests
{
    /// <summary>
    /// BBernard
    /// Generate an in-memory Test Server for Asp.Net Core!
    /// NOTE: Borrowed from HotChocolate.AspNet.Core tests project from the Core HotChocolate source.
    /// </summary>

    public class GraphQLTestBase : IDisposable
    {
        protected GraphQLTestServerFactory ServerFactory { get; }

        public GraphQLTestBase(GraphQLTestServerFactory serverFactory)
        {
            ServerFactory = serverFactory;
        }

        protected GraphQLHelloWorldTestServer CreateHelloWorldTestServer()
        {
            return new GraphQLHelloWorldTestServer(ServerFactory);
        }

        protected GraphQLStarWarsTestServer CreateStarWarsTestServer()
        {
            return new GraphQLStarWarsTestServer(ServerFactory);
        }

        public void Dispose()
        {
            this.ServerFactory?.Dispose();
        }
    }
}
