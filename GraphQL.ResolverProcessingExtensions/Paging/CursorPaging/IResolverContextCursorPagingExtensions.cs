# nullable enable

using HotChocolate.Resolvers;
using HotChocolate.Types.Pagination;
using HotChocolate.ResolverProcessingExtensions.Arguments;
using RepoDb.PagingPrimitives.CursorPaging;

namespace HotChocolate.ResolverProcessingExtensions.Pagination
{
    public static class IResolverContextCursorPagingExtensions
    {
        /// <summary>
        /// Safely process the GraphQL context to retrieve the Cursor Paging arguments;
        /// matches the default names used by HotChocolate Paging middleware (first: int, after: "", last: int, before: "").
        /// Will return null property values for any arguments/param that is not available in the query.
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public static CursorPagingArguments GetCursorPagingArgsSafely(this IResolverContext context)
        {
            var pagingArgs = new CursorPagingArguments(
                first: context?.ArgumentValueSafely<int?>(CursorPagingArgNames.First),
                after: context?.ArgumentValueSafely<string>(CursorPagingArgNames.After),
                last: context?.ArgumentValueSafely<int?>(CursorPagingArgNames.Last),
                before: context?.ArgumentValueSafely<string>(CursorPagingArgNames.Before)
            );

            return pagingArgs;
        }

    }
}
