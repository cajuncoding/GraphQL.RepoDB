#nullable enable

using HotChocolate.Execution.Configuration;
using HotChocolate.PreProcessingExtensions;
using HotChocolate.PreProcessingExtensions.Pagination;
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
            return builder
                //Add DI Middleware for Parameter
                .AddMiddlewareForPreProcessedResults()
                //Add Paging extensions
                .AddPagingForPreProcessedResults();
        }

        /// <summary>
        /// Manually add only support for PreProcessed Paging; 
        /// recommended to use AddPreProcessedResultsExtensions() to enable all support instead.
        /// </summary>
        /// <param name="builder"></param>
        /// <returns></returns>
        [Obsolete("It is now Recommended to simply use ToGraphQLConnection() for directly returning a GraphQL Connection from Hot Chocolate Resolvers instead;"
                  + " since HC has resolved internal bug(s), a Connection result will offer improved performance. This will likely be removed in future release"
                  + " (especially once the new Paging features are available in a later version of v13.")]
        public static IRequestExecutorBuilder AddPagingForPreProcessedResults(
            this IRequestExecutorBuilder builder
        )
        {
            if (builder is null)
                throw new ArgumentNullException(nameof(builder));

            //Updated for v12 to now use the Extension Helpers provided to correctly initialize the Paging Providers for Pre Processed Results...
            builder
                .AddCursorPagingProvider<PreProcessedCursorPagingProvider>()
                .AddOffsetPagingProvider<PreProcessedOffsetPagingProvider>();

            return builder;
        }

        /// <summary>
        /// Add Middleware Logic that enables Dynamic Injection of the GraphQLParamsContext which
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
