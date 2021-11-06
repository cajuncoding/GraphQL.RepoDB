# nullable enable

using HotChocolate.Types.Pagination;
using System;
using System.Collections.Generic;
using System.Linq;
using HotChocolate.PreProcessingExtensions.Pagination;

namespace HotChocolate.PreProcessingExtensions
{
    public static class IEnumerableOffsetPagingCustomExtensions
    {
        /// <summary>
        /// Implement Linq in-memory slicing as described by Relay spec here:
        /// https://relay.dev/graphql/connections.htm#sec-Pagination-algorithm
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="items"></param>
        /// <param name="graphQLPagingArgs"></param>
        /// <param name="includeTotalCount"></param>
        /// <returns></returns>
        public static IOffsetPageResults<T> SliceAsOffsetPage<T>(this IEnumerable<T> items, OffsetPagingArguments graphQLPagingArgs, bool includeTotalCount = true)
            where T : class
        {
            return items.SliceAsOffsetPage(
                skip: graphQLPagingArgs.Skip,
                take: graphQLPagingArgs.Take,
                includeTotalCount: includeTotalCount
            );
        }

        /// <summary>
        /// Implement Linq in-memory slicing as described by Relay spec here:
        /// https://relay.dev/graphql/connections.htm#sec-Pagination-algorithm
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="items"></param>
        /// <param name="skip"></param>
        /// <param name="take"></param>
        /// <param name="includeTotalCount"></param>
        /// <returns></returns>
        public static IOffsetPageResults<T> SliceAsOffsetPage<T>(this IEnumerable<T> items, int? skip, int? take, bool includeTotalCount = true)
            where T : class
        {
            //Do nothing if there are no results...
            if (!items.Any())
                return new OffsetPageResults<T>(Enumerable.Empty<T>(), false, false, 0);

            //NOTE: Implemented similar algorithm that would be used in a SQL Query; also similar to what the default
            //      HotChocolate QueryableCursorPagingHandler does...
            //NOTE: to ensure our pagination is complete, we materialize the Results!
            var skipPast = skip ?? 0;
            var takeSome = take ?? 0;
            var pagedResults = items.Skip(skipPast).Take(takeSome + 1).ToList();

            var hasPreviousPage = pagedResults.Count > 0 && (skipPast > 0);
            var hasNextPage = pagedResults.Count > takeSome;

            if (hasNextPage) pagedResults.Remove(pagedResults.Last());

            //NOTE: We only materialize the FULL Count if actually requested to do so...
            int? totalCount = includeTotalCount ? (int?)items.Count() : null;

            //Wrap all results into a Offset Page Slice result with Total Count...
            var offsetPageResults = new OffsetPageResults<T>(pagedResults, hasNextPage, hasPreviousPage, totalCount);
            return offsetPageResults;
        }
    }
}
