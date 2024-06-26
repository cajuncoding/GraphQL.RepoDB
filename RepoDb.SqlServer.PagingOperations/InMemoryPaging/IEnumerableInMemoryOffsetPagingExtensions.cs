﻿using System;
using System.Collections.Generic;
using System.Linq;
using RepoDb.PagingPrimitives.OffsetPaging;

namespace RepoDb.SqlServer.PagingOperations.InMemoryPaging
{
    public static class IEnumerableInMemoryOffsetPagingExtensions
    {
        /// <summary>
        /// Implement Linq in-memory slicing as described by Relay spec here:
        /// https://relay.dev/graphql/connections.htm#sec-Pagination-algorithm
        /// NOTE: This is primarily used for Unit Testing of in-memory data sets and is generally not recommended for production
        ///     use unless you always have 100% of all your data in-memory. But there are valid use-cases such as if data is archived
        ///     in compressed format and is always retrieved into memory, etc.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="items"></param>
        /// <param name="skip"></param>
        /// <param name="take"></param>
        /// <param name="includeTotalCount"></param>
        /// <returns></returns>
        public static IOffsetPageResults<T> SliceAsOffsetPage<T>(this IEnumerable<T> items, int? skip, int? take, bool includeTotalCount = true)
        {
            //Do nothing if there are no results...
            if (items?.Any() != true)
                return new OffsetPageResults<T>(Enumerable.Empty<T>(), false, false, 0, 0, 0);

            //NOTE: Implemented similar algorithm that would be used in a SQL Query; also similar to what the default
            //      HotChocolate QueryableCursorPagingHandler does...
            const int maxTakeValue = int.MaxValue - 1;
            var skipPast = Math.Max(skip ?? 0, 0);
            var takeSome = Math.Min(take ?? 0, maxTakeValue);

            var pagedResults = items.Skip(skipPast).Take(takeSome + 1).ToList();

            var hasPreviousPage = pagedResults.Count > 0 && skipPast > 0;
            var hasNextPage = pagedResults.Count > takeSome;

            if (hasNextPage) pagedResults.Remove(pagedResults.Last());

            //NOTE: We only materialize the FULL Count if actually requested to do so...
            var totalCount = includeTotalCount ? items.Count() : (int?)null;

            //Wrap all results into a Offset Page Slice result with Total Count...
            return new OffsetPageResults<T>(
                pagedResults, 
                hasNextPage, 
                hasPreviousPage, 
                skipPast + 1, 
                skipPast + takeSome, 
                totalCount
            );
        }
    }
}
