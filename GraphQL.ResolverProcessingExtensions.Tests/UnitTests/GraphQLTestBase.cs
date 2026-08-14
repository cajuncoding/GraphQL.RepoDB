using System;

namespace HotChocolate.ResolverProcessingExtensions.Tests
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
