using System;
using System.Collections.Generic;
using System.Linq;
using HotChocolate.Types.Pagination;

namespace HotChocolate.PreProcessingExtensions.Pagination
{
    /// <summary>
    /// Model class for representing a paging result set that was computed using Cursor Pagination process by offering
    /// a default implementation of the IOffsetPageResults interface which de-couples the code that executes queries 
    /// from the actual PreProcessing extension classes used for the HotChocolate.
    /// This class generally to be used by libraries and/or lower level code that executes queries and renders page results.
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    public class OffsetPageResults<TEntity> : IOffsetPageResults<TEntity>
    {
        public OffsetPageResults(IEnumerable<TEntity> results, bool hasNextPage, bool hasPreviousPage, int? totalCount)
        {
            this.Results = results ?? throw new ArgumentNullException(nameof(results));
            this.HasNextPage = hasNextPage;
            this.HasPreviousPage = hasPreviousPage;
            this.TotalCount = totalCount;
        }

        public IEnumerable<TEntity> Results { get; protected set; }

        public int? TotalCount { get; protected set; }

        public bool HasNextPage { get; protected set; }

        public bool HasPreviousPage { get; protected set; }


        public virtual OffsetPageResults<TTargetType> OfType<TTargetType>() 
        {
            var results = this.Results?.OfType<TTargetType>();
            return new OffsetPageResults<TTargetType>(results, this.HasNextPage, this.HasPreviousPage, this.TotalCount);
        }

        public virtual OffsetPageResults<TTargetType> AsMappedType<TTargetType>(Func<TEntity, TTargetType> mappingFunc)
        {
            if (mappingFunc == null)
                throw new ArgumentException(nameof(mappingFunc));

            var results = this.Results?.Select(mappingFunc);
            return new OffsetPageResults<TTargetType>(results, this.HasNextPage, this.HasPreviousPage, this.TotalCount);
        }

        /// <summary>
        /// Convenience method to convert the current offset based paginated results to a GraphQL CollectionSegment result to return from the Resolver;
        /// CollectionSegment results will not be post-processed as since it's already paginated!
        /// </summary>
        /// <returns></returns>
        public virtual CollectionSegment<TEntity> ToGraphQLCollectionSegment()
        {
            //Ensure we are null safe and return a valid empty list by default.
            var segmentResults = (this.Results ?? Enumerable.Empty<TEntity>()).ToList().AsReadOnly();

            var collectionSegmentInfo = new CollectionSegmentInfo(
                hasNextPage: this.HasNextPage,
                hasPreviousPage: this.HasPreviousPage
            );

            var graphqlCollectionSegment = new CollectionSegment<TEntity>(
                segmentResults,
                collectionSegmentInfo,
                this.TotalCount ?? 0
            );

            return graphqlCollectionSegment;
        }
    }
}
