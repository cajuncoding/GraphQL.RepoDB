# nullable enable

using HotChocolate.Resolvers;
using HotChocolate.Types.Pagination;
using System;
using HotChocolate.PreProcessingExtensions.Arguments;


namespace HotChocolate.PreProcessingExtensions.Pagination
{
    public static class IResolverContextOffsetPagingExtensions
    {
        /// <summary>
        /// Safely process the GraphQL context to retrieve the Cursor Paging arguments;
        /// matches the default names used by HotChocolate Paging middleware (first: int, after: "", last: int, before: "").
        /// Will return null if the order arguments/info is not available.
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public static OffsetPagingArguments GetOffsetPagingArgsSafely(this IResolverContext context)
        {
            int? skip = context?.ArgumentValueSafely<int?>(OffsetPagingArgNames.SkipDescription);
            int? take = context?.ArgumentValueSafely<int?>(OffsetPagingArgNames.TakeDescription);

            //Initialize with default values that enable default behaviour to retrieve all results anytime
            //  the values are not specified; consistent with Cursor based paging where all params are optional.
            var pagingArgs = new OffsetPagingArguments(skip ?? 0, take ?? int.MaxValue);

            return pagingArgs;
        }
    }
}
