using System.Threading.Tasks;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using HotChocolate.AzureFunctions;

namespace StarWars.AzureFunctions
{
    /// <summary>
    /// AzureFunction Endpoint for the Star Wars GraphQL Schema queries
    /// NOTE: This class is not marked as static so that .Net Core DI handles injecting
    ///         the Executor Proxy for us.
    /// </summary>
    public class StarWarsGraphQLFunctionEndpoint(IGraphQLRequestExecutor graphqlExecutor)
    {
        [Function(nameof(StarWarsGraphQLFunctionEndpoint))]
        public Task<HttpResponseData> Run([HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = "graphql/{**slug}")] HttpRequestData req) 
            => graphqlExecutor.ExecuteAsync(req);
    }
}
