using Microsoft.Extensions.DependencyInjection;
using HotChocolate;
using StarWars.Characters;
using StarWars.Repositories;
using StarWars.Reviews;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using HotChocolate.AzureFunctionsProxy;
using Microsoft.Data.SqlClient;
using System;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using HotChocolate.Types.Pagination;

//CRITICAL: Here we self-wire up the Startup into the Azure Functions framework!
[assembly: FunctionsStartup(typeof(StarWars.Startup))]

namespace StarWars
{
    /// <summary>
    /// Startup middleware configurator specific for AzureFunctions
    /// </summary>
    public class Startup : FunctionsStartup
    {
        // This method gets called by the AzureFunctions runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit:
        //  https://docs.microsoft.com/en-us/azure/azure-functions/functions-dotnet-dependency-injection
        public override void Configure(IFunctionsHostBuilder builder)
        {
            string sqlConnectionString = Environment.GetEnvironmentVariable("SqlConnectionString");

            //RepoDb Bootstrapper for Sql Server
            RepoDb.SqlServerBootstrap.Initialize();

            var services = builder.Services;

            // Add the custom services like repositories etc ...
            services.AddTransient<ICharacterRepository, CharacterRepository>(c => new CharacterRepository(sqlConnectionString));
            services.AddSingleton<IReviewRepository, ReviewRepository>();

            // Add GraphQL Services
            //Updated to Initialize StarWars with new v11 configuration...
            services
                .AddGraphQLServer()
                .AddQueryType(d => d.Name("Query"))
                .AddMutationType(d => d.Name("Mutation"))
                //Disabled Subscriptions for v11 and Azure Functions Example due to 
                //  supportability in Server-less architecture...
                //.AddSubscriptionType(d => d.Name("Subscription"))
                .AddType<CharacterQueries>()
                .AddType<ReviewQueries>()
                .AddType<ReviewMutations>()
                //Disabled Subscriptions for v11 and Azure Functions Example due to 
                //  supportability in Serverless architecture...
                //.AddType<ReviewSubscriptions>()
                .AddType<Human>()
                .AddType<HumanFieldResolvers>()
                .AddType<Droid>()
                .AddType<Starship>()
                //*******************************************************************************************
                //*******************************************************************************************
                //Enable extensions for Pre-Processed Results!
                //NOTE This allows all OOTB behaviors except for when we want to control the processing
                //  of results for sorting, paging, etc. and do not want redundant post-processing to occur
                //  by HotChocolate internals...
                //NOTE: This Adds Sorting & Paging providers/conventions by default!
                .SetPagingOptions(new PagingOptions()
                {
                    DefaultPageSize = 2,
                    IncludeTotalCount = true,
                    MaxPageSize = 5
                })
                .ModifyRequestOptions(o =>
                    {
                        //Enable better Debugging Experience!
                        if (Debugger.IsAttached)
                        {
                            o.ExecutionTimeout = TimeSpan.FromHours(1);
                        }
                    }
                )
                .AddPreProcessedResultsExtensions()
                //*******************************************************************************************
                //*******************************************************************************************
                //Now Required in v11 to support the Attribute Usage (e.g. you may see the
                //  error: No filter convention found for scope `none`
                .AddFiltering();
                //.AddSorting();

            //Finally Initialize AzureFunctions Executor Proxy here...
            //You man Provide a specific SchemaName for multiple Functions (e.g. endpoints).
            //TODO: Test multiple SchemaNames...
            services.AddAzureFunctionsGraphQL(options =>
            {
                options.AzureFunctionsRoutePath = "/api/graphql/playground";
            });
        }
    }
}
