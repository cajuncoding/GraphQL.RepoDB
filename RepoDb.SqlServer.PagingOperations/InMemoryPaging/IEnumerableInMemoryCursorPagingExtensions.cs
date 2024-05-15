using System;
using System.Collections.Generic;
using System.Linq;
using RepoDb.PagingPrimitives.CursorPaging;

namespace RepoDb.SqlServer.PagingOperations.InMemoryPaging
{
    public static class IEnumerableInMemoryCursorPagingExtensions
    {
        /// <summary>
        /// Implement Linq in-memory slicing as described by Relay spec here:
        /// https://relay.dev/graphql/connections.htm#sec-Pagination-algorithm
        /// NOTE: This is primarily used for Unit Testing of in-memory data sets and is generally not recommended for production
        ///     use unless you always have 100% of all your data in-memory; this is because sorting must be done on a pre-filtered and/or
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="items"></param>
        /// <param name="pagingArgs"></param>
        /// <returns></returns>
        public static ICursorPageResults<T> SliceAsCursorPage<T>(this IEnumerable<T> items, string after, int? first, string before, int? last)
        where T : class
        {
            //Do nothing if there are no results...
            if (!items.Any())
                return new CursorPageResults<T>(Enumerable.Empty<ICursorResult<T>>(), 0, false, false);

            var afterIndex = after != null
                ? RepoDbCursorHelper.ParseCursor(after)
                : 0;

            var beforeIndex = before != null
                ? RepoDbCursorHelper.ParseCursor(before)
                : 0;

            //FIRST log the index of all items in the list BEFORE slicing, as these indexes are 
            //  the Cursor Indexes for paging up/down the entire list, & ICursorResult is the Decorator 
            //  around the Entity Models.

            //NOTE: We MUST materialize this after applying index values to prevent ongoing increments...
            IEnumerable<CursorResult<T>> slice = items
                .Select((item, index) => CursorResult<T>.CreateIndexedCursor(item, RepoDbCursorHelper.CreateCursor(index), index))
                .ToList();

            int totalCount = slice.Count();

            //If After specified, remove all before After (or skip past After)
            if (afterIndex > 0 && slice.Last().CursorIndex > afterIndex)
            {
                slice = slice.Skip(afterIndex);
            }

            //If Before is specified, remove all after Before (Skip Until Before is reached)
            if (beforeIndex > 0 && slice.Last().CursorIndex > beforeIndex)
            {
                slice = slice.SkipWhile(c => c.CursorIndex < beforeIndex);
            }

            //If First is specified, then take the first/top rows from the current Slice!
            if (first.HasValue && first > 0 && slice.Count() > first)
            {
                slice = slice.Take(first.Value);
            }

            //If First is specified, then take the first/top rows from the current Slice!
            if (last.HasValue && last > 0 && slice.Count() > last)
            {
                slice = slice.TakeLast(last.Value);
            }

            //Wrap all results into a PagedCursor Slice result wit Total Count...
            //NOTE: to ensure our pagination is complete, we materialize the Results!
            var results = slice.ToList();
            var firstCursor = results.FirstOrDefault();
            var lastCursor = results.LastOrDefault();

            var cursorPageSlice = new CursorPageResults<T>(
                results,
                totalCount,
                hasPreviousPage: firstCursor?.CursorIndex > 1,
                hasNextPage: lastCursor?.CursorIndex < totalCount
            );
            return cursorPageSlice;
        }

        #if NETSTANDARD2_0

        /// <summary>
        /// Adapted/inspired by Stack Overflow post here:
        /// https://stackoverflow.com/a/3453301/7293142
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public static IEnumerable<T> TakeLast<T>(this IEnumerable<T> source, int count)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));
            else if (count < 0) throw new ArgumentOutOfRangeException(nameof(count));
            else if (count == 0) return Enumerable.Empty<T>();

            var sourceCount = (source as ICollection<T>)?.Count
                                    ?? (source as IReadOnlyCollection<T>)?.Count
                                    // ReSharper disable once PossibleMultipleEnumeration
                                    ?? source.Count();

            // ReSharper disable once PossibleMultipleEnumeration
            return source.Skip(Math.Max(0, sourceCount - count));
        }
        #endif
    }
}
