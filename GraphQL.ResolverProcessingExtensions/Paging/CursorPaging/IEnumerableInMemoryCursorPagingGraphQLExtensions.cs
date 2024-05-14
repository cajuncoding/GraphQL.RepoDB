using HotChocolate.Types.Pagination;
using System.Collections.Generic;
using RepoDb.CursorPaging;
using RepoDb.CursorPaging.InMemoryProcessing;

namespace HotChocolate.ResolverProcessingExtensions
{
    public static class IEnumerableCursorPagingCustomExtensions
    {
        /// <summary>
        /// Provides Linq in-memory slicing as described by Relay spec here:
        /// https://relay.dev/graphql/connections.htm#sec-Pagination-algorithm
        /// NOTE: This is primarily used for Unit Testing of in-memory data sets and is generally not recommended for production
        ///     use unless you always have 100% of all your data in-memory; this is because sorting must be done on a pre-filtered and/or
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="items"></param>
        /// <param name="graphqlPagingArgs"></param>
        /// <returns></returns>
        public static ICursorPageSlice<T> SliceAsCursorPage<T>(this IEnumerable<T> items, CursorPagingArguments graphqlPagingArgs)
            where T : class
        {
            return items.SliceAsCursorPage(
                after: graphqlPagingArgs.After,
                first: graphqlPagingArgs.First,
                before: graphqlPagingArgs.Before,
                last: graphqlPagingArgs.Last
            );
        }
    }
}
