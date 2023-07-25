using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HotChocolate.AspNetCore.Extensions;
using HotChocolate.Execution.Configuration;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;

namespace HotChocolate.PreProcessingExtensions.Tests
{
    public abstract class GraphQLTestServerBase
    {
        public TestServer Server { get; protected set;  }

        public List<KeyValuePair<string, IParamsContext>> ParamsContextList { get; } = new List<KeyValuePair<string, IParamsContext>>();

        public IParamsContext GetParamsContext(string fieldName)
        {
            var paramsContext = ParamsContextList
                .LastOrDefault(ctx => ctx.Key.Equals(fieldName, StringComparison.OrdinalIgnoreCase))
                .Value;

            return paramsContext;
        }

        protected TestServer CreateTestServer(
            GraphQLTestServerFactory serverFactory,
            Func<IServiceCollection, IRequestExecutorBuilder> servicesConfigure,
            string pattern = "/graphql",
            Action<GraphQLEndpointConventionBuilder> configureConventions = default)
        {
            return serverFactory.Create(
                services =>
                {
                    services
                        .AddRouting();
                        //.AddHttpResultSerializer(HttpResultSerialization.JsonArray);

                    //BBernard - Hook for IoC Services Configuration by implementing Test Servers...                    
                    var graphQLBuilder = servicesConfigure(services);

                    //BBernard
                    //Allow Tests to provide (IoC) custom configuration to the builder
                    //  as needed for custom testing via Middleware!
                    graphQLBuilder.UseField(next => context =>
                    {
                        //BBernard intercept the ParamsContext initialized by the PreProcessing Results Middleware
                        //  and expose it publicly for Test cases to utilize!
                        //NOTE: MANY middleware invocations will execute for various fields so have to track them all
                        //      for later access...
                        var paramsContext = context.GetLocalState<GraphQLParamsContext>(nameof(GraphQLParamsContext));
                        
                        ParamsContextList.Add(new KeyValuePair<string, IParamsContext>(
                            context.Selection.Field.Name, 
                            new ParamsContextTestHarness(paramsContext)
                        ));

                        //BBernard - WE MUST ALLOW THE PIPELINE TO CONTINUE!
                        return next.Invoke(context);
                    });
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

        /// <summary>
        /// Convenience method to simplify tests & ceremonial code
        /// </summary>
        /// <param name="graphQLRequest"></param>
        /// <returns></returns>
        public async Task<GraphQLQueryResult> PostAsync(GraphQLQueryRequest graphQLRequest)
        {
            var result = await Server.PostAsync(graphQLRequest);
            return result;
        }

        /// <summary>
        /// Convenience method to simplify tests & ceremonial code
        /// </summary>
        /// <param name="graphQLRequest"></param>
        /// <returns></returns>
        public async Task<GraphQLQueryResult> PostQueryAsync(string query)
        {
            var result = await PostAsync(new GraphQLQueryRequest()
            {
                Query = query
            });

            return result;
        }
    }
}
