#nullable enable

using HotChocolate.Execution.Configuration;
using HotChocolate.ResolverProcessingExtensions;
using System;

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
    public static class ResolverProcessedResultsMiddlewareExtensions
    {
        public static bool IsRegistered { get; private set; }

        /// <summary>
        /// This is the Primary method to wire up all extensions for working with
        /// Resolver Processed results that have been computed inside the Resolver or lower layer (e.g. Services/Repositories).
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static IRequestExecutorBuilder AddResolverProcessedResultsExtensions(
            this IRequestExecutorBuilder builder,
            string? name = null
        )
        {
            IsRegistered = true;

            //Add DI Middleware for Parameter Facade to be Injected and results
            //  (processed inside the resolver) to be correctly handled...
            return builder.AddMiddlewareForResolverProcessedResults();
        }

        /// <summary>
        /// Add Middleware Logic that enables Dynamic Injection of the GraphQLParamsContext which
        ///  makes working with Select Fields, Sort Fields, Paging Arguments, etc. much easier and elegant
        ///  via Resolver DI; but can always be manually constructed from IResolverContext
        ///  in any other scenario needed.
        /// </summary>
        /// <param name="builder"></param>
        /// <returns></returns>
        public static IRequestExecutorBuilder AddMiddlewareForResolverProcessedResults(
            this IRequestExecutorBuilder builder
        )
        {
            if (builder is null)
                throw new ArgumentNullException(nameof(builder));

            IsRegistered = true;

            //Add the middleware DI logic as Local Scoped data for Parameters context...
            return builder.UseField<ResolverProcessingResultsMiddleware>();
        }


    }
}
