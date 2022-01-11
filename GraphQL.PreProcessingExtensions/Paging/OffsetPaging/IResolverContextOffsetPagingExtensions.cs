# nullable enable

using HotChocolate.Resolvers;
using HotChocolate.Types.Pagination;
using System;
using GraphQL.PreProcessingExtensions.Paging;
using HotChocolate.PreProcessingExtensions.Arguments;


namespace HotChocolate.PreProcessingExtensions.Pagination
{
    public static class IResolverContextOffsetPagingExtensions
    {
        /// <summary>
        /// Safely process the GraphQL context to retrieve the Offset Paging arguments;
        /// matches the default names used by HotChocolate Paging middleware (skip: int, take: int).
        /// Will return null property values for any arguments/param that is not available in the query
        ///
        /// NOTE: As of 11/15/2021 this is now consistent with the behavior of CursorPaging and makes it easier
        ///         to control default values within your resolvers (e.g. via null coalesce operators).
        /// </summary>
        /// <param name="context"></param>
        /// <param name="defaultSkip"></param>
        /// <param name="defaultTake"></param>
        /// <returns></returns>
        public static OffsetPagingArguments GetOffsetPagingArgsSafely(this IResolverContext context, int? defaultSkip = null, int? defaultTake = null)
        {
            int? skip = context?.ArgumentValueSafely<int?>(OffsetPagingArgNames.Skip);
            int? take = context?.ArgumentValueSafely<int?>(OffsetPagingArgNames.Take);

            //Initialize with default values that enable default behaviour to retrieve all results anytime
            //  the values are not specified; consistent with Cursor based paging where all params are optional.
            var pagingArgs = new OffsetPagingArguments(skip ?? defaultSkip, take ?? defaultTake);

            return pagingArgs;
        }
    }
}
