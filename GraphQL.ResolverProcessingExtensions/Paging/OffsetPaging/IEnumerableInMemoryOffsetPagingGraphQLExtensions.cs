using HotChocolate.Types.Pagination;
using System.Collections.Generic;
using RepoDb.OffsetPaging;
using RepoDb.SqlServer.PagingOperations.InMemoryProcessing;

namespace HotChocolate.ResolverProcessingExtensions
{
    public static class IEnumerableOffsetPagingCustomExtensions
    {
        /// <summary>
        /// Implement Linq in-memory slicing as described by Relay spec here:
        /// https://relay.dev/graphql/connections.htm#sec-Pagination-algorithm
        /// NOTE: This is primarily used for Unit Testing of in-memory data sets and is generally not recommended for production
        ///     use unless you always have 100% of all your data in-memory; this is because sorting must be done on a pre-filtered and/or
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="items"></param>
        /// <param name="graphqlPagingArgs"></param>
        /// <param name="includeTotalCount"></param>
        /// <returns></returns>
        public static IOffsetPageResults<T> SliceAsOffsetPage<T>(this IEnumerable<T> items, OffsetPagingArguments graphqlPagingArgs, bool includeTotalCount = true)
            where T : class
        {
            return items.SliceAsOffsetPage(
                skip: graphqlPagingArgs.Skip,
                take: graphqlPagingArgs.Take,
                includeTotalCount: includeTotalCount
            );
        }
    }
}
