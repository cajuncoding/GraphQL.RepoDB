#nullable enable

using HotChocolate.Execution.Configuration;
using HotChocolate.PreProcessingExtensions;
using System;

//Use the Same namespace as HotChocolate...
namespace Microsoft.Extensions.DependencyInjection
{ 
    /// <summary>
    /// Custom Extensions to initialize the Sorting Middleware with compatibility for PreProcessed 
    /// results, interfaces and IEnumerable decorator objects returned from Resolvers whereby Sorting
    /// was already implemented at the Resolver, or lower level, business logic.
    /// 
    /// This works in collaboration with OOTB Queryable functionality, intercepting only PreProcessed result
    /// decorated results.
    /// </summary>
    public static class PreProcessedResultsMiddlewareExtensions
    {
        /// <summary>
        /// This is the Primary method to wire up all extensions for working with
        /// Pre-Processed results in the Resolver or lower layer (e.g. Services/Repositories).
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static IRequestExecutorBuilder AddPreProcessedResultsExtensions(
            this IRequestExecutorBuilder builder,
            string? name = null
        )
        {
            //Add DI Middleware for Parameter Facade to be Injected and results
            //  (pre-processed in the resolver) to be correctly handled...
            return builder.AddMiddlewareForPreProcessedResults();
        }

        /// <summary>
        /// Add Middleware Logic that enables Dynamic Injection of the graphqlParamsContext which
        ///  makes working with Select Fields, Sort Fields, Paging Arguments, etc. much easier and elegant
        ///  in a pure code first use case; but can always be manually constructed from IResolverContext
        ///  in any other scenario needed.
        /// </summary>
        /// <param name="builder"></param>
        /// <returns></returns>
        public static IRequestExecutorBuilder AddMiddlewareForPreProcessedResults(
            this IRequestExecutorBuilder builder
        )
        {
            if (builder is null)
                throw new ArgumentNullException(nameof(builder));

            //Add the middleware DI logic as Local Scoped data for Parameters context...
            return builder.UseField<PreProcessingResultsMiddleware>();
        }


    }
}
