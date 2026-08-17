using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Functions.Worker.ContextAccessor;
using Functions.Worker.HttpResponseDataCompression;
using Functions.Worker.HttpResponseDataJsonMiddleware;
using RepoDb;
using StarWars.Characters;
using StarWars.Repositories;
using StarWars.Reviews;
using System;
using System.Diagnostics;
using Microsoft.Extensions.Logging;
using Functions.Worker.ILoggerSupport;

//RepoDb Bootstrapper for Sql Server
GlobalConfiguration.Setup().UseSqlServer();

//BBernard
//Initialize Bootstrap Dependencies...
var host = Host
    .CreateDefaultBuilder()
    .ConfigureFunctionsWorkerDefaults(appBuilder =>
    {
        appBuilder
            .UseFunctionContextAccessor()
            .UseHttpResponseDataCompression()
            .UseJsonResponses();
    })
    //NOTE: This is CRITICAL for Logging - We must enforce a minimum logging level for the isolated process Worker which is NOT the same as the HOST!
    //The Host's logging configuration will NOT apply to the Worker if the Worker's default level is too high thereforew we must set the minimum to be Trace!
    //Our GraphQL Logging Event Listener is running within the Worker so this enables Trace logging outputs to write to the Console & App Insights (when deployed) as expected!
    //MORE INFO: The way this works isILogger → (Worker filtering FIRST) → (Host filtering SECOND via host.json) → Output to Console & App Insights...
    //  - If Worker blocks it → Host never sees it
    //  - If Worker allows it → Host decides whether to emit/store it as configured via host.json!
    .ConfigureLogging(logConfiguration =>
    {
        logConfiguration.SetMinimumLevel(LogLevel.Trace);
    })
    .ConfigureServices((context, services) =>
    {
        string sqlConnectionString = Environment.GetEnvironmentVariable("SqlConnectionString");

        // Add the custom services like repositories etc ...
        services.AddTransient<ICharacterRepository, CharacterRepository>(c => new CharacterRepository(sqlConnectionString));
        services.AddSingleton<IReviewRepository, ReviewRepository>();

        services.AddFunctionILoggerSupport();

        // Add GraphQL Services
        services
            //Add the GraphQL Server for Azure Functions with Official Implementation!
            .AddGraphQLFunction()
            //.UsePersistedQueryPipeline().AddReadOnlyFileSystemQueryStorage("./PersistedQueries")
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
            //Enable extensions for RepoDb & Resolver Processing Results!
            //NOTE This allows all OOTB behaviors except for when we want to control the processing
            //  of results for sorting, paging, etc. and do not want redundant post-processing to occur
            //  by HotChocolate internals...
            //NOTE: This Adds Sorting & Paging providers/conventions by default!
            .ModifyPagingOptions(options =>
            {
                options.DefaultPageSize = 2;
                options.IncludeTotalCount = true;
                options.MaxPageSize = 5;
            })
            .ModifyRequestOptions(o => {
                //Enable better Debugging Experience!
                if (Debugger.IsAttached)
                    o.ExecutionTimeout = TimeSpan.FromHours(1);
            })
            .AddRepoDbExtensions()
            //*******************************************************************************************
            //*******************************************************************************************
            //Now Required in v11 to support the Attribute Usage (e.g. you may see the
            //  error: No filter convention found for scope `none`
            .AddFiltering()
            .AddSorting();
    })
    .Build();

await host.RunAsync().ConfigureAwait(false);