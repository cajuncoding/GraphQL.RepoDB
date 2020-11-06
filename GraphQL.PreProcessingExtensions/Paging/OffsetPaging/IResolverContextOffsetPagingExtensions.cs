# nullable enable

using HotChocolate.Resolvers;
using HotChocolate.Types.Pagination;
using System;


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

            //Initialize with -1 values if the values are not specified; because the OffsetPagingArguments
            //  cannot be null itself, nor can the Take property be null as it's minimally required.
            //  so we use -1 as an indicator of a non-initialized OffsetPagingArguments.
            var pagingArgs = new OffsetPagingArguments(skip ?? -1, take ?? -1);

            return pagingArgs;
        }
    }
}
