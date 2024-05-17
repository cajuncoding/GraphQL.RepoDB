#nullable enable

using HotChocolate.Execution.Configuration;
using System;
using HotChocolate.RepoDb;

//Use the Same namespace as HotChocolate...
namespace Microsoft.Extensions.DependencyInjection
{ 
    /// <summary>
    /// Custom Extensions to initialize the Sorting Middleware with compatibility for ResolverProcessed 
    /// results, interfaces and IEnumerable decorator objects returned from Resolvers whereby Sorting
    /// was already implemented at the Resolver, or lower level, business logic.
    /// 
    /// This works in collaboration with OOTB Queryable functionality, intercepting only ResolverProcessed result
    /// decorated results.
    /// </summary>
    public static class GraphQLRepoDbMappingMiddlewareExtensions
    {
        public static bool IsRegistered { get; private set; }

        /// <summary>
        /// This is the Primary method to wire up all extensions for working with
        /// RepoDb with HotChocolate GraphQL Server.
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static IRequestExecutorBuilder AddRepoDbExtensions(
            this IRequestExecutorBuilder builder,
            string? name = null
        )
        {
            //Dynamically detect if AddResolverProcessedResultsExtensions() has been called and if not then we call it
            //  to ensure our dependencies are initialized...
            if (!ResolverProcessingMiddlewareExtensions.IsRegistered)
            {
                builder.AddResolverProcessingExtensions();
            }

            IsRegistered = true;

            //Add DI Middleware for RepoDb Mapping Facade to be Injected into Resolvers...
            return builder.AddMiddlewareForRepoDbMapping();
        }

        /// <summary>
        /// Add Middleware Logic that enables Dynamic Injection of the GraphQLRepoDbMapper which
        ///  makes working with Select Fields, Sort Fields, Paging Arguments, etc. much easier and elegant
        ///  via Resolver DI; but can always be manually constructed from IResolverContext
        ///  in any other scenario needed.
        /// </summary>
        /// <param name="builder"></param>
        /// <returns></returns>
        public static IRequestExecutorBuilder AddMiddlewareForRepoDbMapping(
            this IRequestExecutorBuilder builder
        )
        {
            if (builder is null)
                throw new ArgumentNullException(nameof(builder));

            IsRegistered = true;

            //Add the middleware DI logic as Local Scoped data for Parameters context...
            return builder.UseField<GraphQLRepoDbMappingMiddleware>();
        }


    }
}
