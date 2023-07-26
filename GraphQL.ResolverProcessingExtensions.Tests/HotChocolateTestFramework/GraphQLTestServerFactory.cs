using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;

namespace HotChocolate.ResolverProcessingExtensions.Tests
{
    /// <summary>
    /// BBernard
    /// Generate an in-memory Test Server for Asp.Net Core!
    /// NOTE: Borrowed from HotChocolate.AspNet.Core tests project from the Core HotChocolate source.
    /// </summary>
    public class GraphQLTestServerFactory : IDisposable
    {
        private readonly List<TestServer> _instances = new List<TestServer>();

        public TestServer Create(
            Action<IServiceCollection> configureServices,
            Action<IApplicationBuilder> configureApplication)
        {
            IWebHostBuilder builder = new WebHostBuilder()
                .Configure(configureApplication)
                .ConfigureServices(services =>
                {
                    services.AddHttpContextAccessor();
                    configureServices?.Invoke(services);
                });

            var server = new TestServer(builder);
            _instances.Add(server);
            return server;
        }

        public void Dispose()
        {
            foreach (TestServer testServer in _instances)
            {
                testServer.Dispose();
            }
        }
    }
}