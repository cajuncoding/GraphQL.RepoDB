using System.Collections.Generic;
using System.Linq;
using HotChocolate.Types.Pagination;
using RepoDb.PagingPrimitives.CursorPaging;

namespace HotChocolate.ResolverProcessingExtensions
{
    /// <summary>
    /// Custom Extensions using Cursor Pagination process with GraphQL Arguments.
    /// </summary>
    public static class CursorPageSliceGraphQLExtensions
    {
        /// <summary>
        /// Convenience method to convert the current cursor based page slice to a GraphQL Connection result to return from the Resolver;
        /// Connection results will not be post-processed since it's already paginated!
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <returns></returns>
        public static Connection<TEntity> ToGraphQLConnection<TEntity>(this ICursorPageResults<TEntity> cursorPage)
        {
            var edges = cursorPage.ToEdgeResults().ToList();

            var connectionPageInfo = new ConnectionPageInfo(
                hasNextPage: cursorPage.HasNextPage,
                hasPreviousPage: cursorPage.HasPreviousPage,
                startCursor: edges.FirstOrDefault()?.Cursor,
                endCursor: edges.LastOrDefault()?.Cursor
            );

            var graphqlConnection = new Connection<TEntity>(
                edges,
                connectionPageInfo,
                cursorPage.TotalCount ?? 0
            );

            return graphqlConnection;
        }

        private static IEnumerable<Edge<TEntity>> ToEdgeResults<TEntity>(this ICursorPageResults<TEntity> cursorPage)
        {
            //Ensure we are null safe and return a valid empty list by default.
            //Note: We intentionally do NOT call ToList() here so that consuming classes may provide additional filtering...
            var results = cursorPage.CursorResults
                ?.Where(cr => cr != null)
                //.Select(cr => IndexEdge<TEntity>.Create(cr.Entity, cr.CursorIndex))
                .Select(cr => new Edge<TEntity>(cr.Entity, cr.Cursor))
                ?? Enumerable.Empty<Edge<TEntity>>();

            return results;
        }
    }
}
