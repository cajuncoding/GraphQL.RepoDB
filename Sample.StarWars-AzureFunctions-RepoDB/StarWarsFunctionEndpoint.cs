using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using HotChocolate.AzureFunctions;

namespace StarWars.AzureFunctions
{
    /// <summary>
    /// AzureFunction Endpoint for the Star Wars GraphQL Schema queries
    /// NOTE: This class is not marked as static so that .Net Core DI handles injecting
    ///         the Executor Proxy for us.
    /// </summary>
    public class StarWarsFunctionEndpoint
    {
        private readonly IGraphQLRequestExecutor _graphqlExecutor;

        public StarWarsFunctionEndpoint(IGraphQLRequestExecutor graphqlExecutor)
        {
            _graphqlExecutor = graphqlExecutor;
        }

        [FunctionName(nameof(StarWarsFunctionEndpoint))]
        public Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = "graphql/{**slug}")] HttpRequest req) 
            => _graphqlExecutor.ExecuteAsync(req);
    }
}
