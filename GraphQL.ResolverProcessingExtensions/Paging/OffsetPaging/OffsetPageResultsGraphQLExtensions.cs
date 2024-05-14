using System.Linq;
using HotChocolate.Types.Pagination;
using RepoDb.OffsetPaging;

namespace HotChocolate.ResolverProcessingExtensions
{
    /// <summary>
    /// Custom Extensions using Offset Pagination process with GraphQL Arguments.
    /// </summary>
    public static class OffsetPageResultsGraphQLExtensions
    {
        /// <summary>
        /// Convenience method to convert the current offset based paginated results to a GraphQL CollectionSegment result to return from the Resolver;
        /// CollectionSegment results will not be post-processed as since it's already paginated!
        /// </summary>
        /// <returns></returns>
        public static CollectionSegment<TEntity> ToGraphQLCollectionSegment<TEntity>(this IOffsetPageResults<TEntity> offsetPage)
        {
            //Ensure we are null safe and return a valid empty list by default.
            var segmentResults = (offsetPage.Results ?? Enumerable.Empty<TEntity>()).ToList().AsReadOnly();

            var collectionSegmentInfo = new CollectionSegmentInfo(
                hasNextPage: offsetPage.HasNextPage,
                hasPreviousPage: offsetPage.HasPreviousPage
            );

            var graphqlCollectionSegment = new CollectionSegment<TEntity>(
                segmentResults,
                collectionSegmentInfo,
                offsetPage.TotalCount ?? 0
            );

            return graphqlCollectionSegment;
        }
    }
}
