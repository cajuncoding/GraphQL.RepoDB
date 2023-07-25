﻿using System;
using System.Collections.Generic;
using System.Linq;
using HotChocolate.Types.Pagination;

namespace HotChocolate.PreProcessingExtensions.Pagination
{
    /// <summary>
    /// Model class for representing a paging result set that was computed using Cursor Pagination process by offering
    /// a default implementation of the ICursorPageSlice interface which de-couples the code that executes queries 
    /// from the actual PreProcessing extension classes used for the HotChocolate.
    /// This class generally to be used by libraries and/or lower level code that executes queries and renders page results.
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    public class CursorPageSlice<TEntity> : ICursorPageSlice<TEntity>
    {
        public CursorPageSlice(IEnumerable<ICursorResult<TEntity>> results, int? totalCount, bool hasPreviousPage, bool hasNextPage)
        {
            this.CursorResults = results ?? throw new ArgumentException(nameof(results));
            this.TotalCount = totalCount;
            this.HasPreviousPage = hasPreviousPage;
            this.HasNextPage = hasNextPage;
        }

        public IEnumerable<ICursorResult<TEntity>> CursorResults { get; protected set; }

        public IEnumerable<TEntity> Results => CursorResults?.Select(cr => cr.Entity);

        public int? TotalCount { get; }

        public bool HasNextPage { get; protected set; }

        public bool HasPreviousPage { get; protected set; }
        /// <summary>
        /// Convenience method to easily cast all types in the current page to a garget compatible type
        /// without affecting the cursor indexes, etc. Provide deferred execution via Linq Select(). Type mismatches
        /// will be ignored and not returned for behaviour matching Linq OfType().
        /// </summary>
        /// <typeparam name="TTargetType"></typeparam>
        /// <returns></returns>
        public CursorPageSlice<TTargetType> OfType<TTargetType>()
        {
            var enumerableResults = this.CursorResults
                .Select(r =>
                {
                    // ReSharper disable once ConvertToLambdaExpression
                    return r.Entity is TTargetType entityAsTargetType
                        ? new CursorResult<TTargetType>(entityAsTargetType, r.CursorIndex)
                        : null;
                })
                .Where(c => c != null);

            return new CursorPageSlice<TTargetType>(enumerableResults, this.TotalCount, this.HasPreviousPage, this.HasNextPage);
        }

        /// <summary>
        /// Convenience method to easily map/convert/project all types in the current page to a different object type
        /// altogether, without affecting the cursor indexes, etc. Provide deferred execution via Linq Select().
        /// </summary>
        /// <typeparam name="TTargetType"></typeparam>
        /// <param name="mappingFunc">Specify the Func that takes the current type in and returns the target type.</param>
        /// <returns></returns>
        public CursorPageSlice<TTargetType> AsMappedType<TTargetType>(Func<TEntity, TTargetType> mappingFunc)
        {
            if (mappingFunc == null) throw new ArgumentException(nameof(mappingFunc));
            var results = this.CursorResults?.Select(r =>
            {
                var mappedEntity = mappingFunc(r.Entity);
                return new CursorResult<TTargetType>(mappedEntity, r.CursorIndex);
            });

            return new CursorPageSlice<TTargetType>(results, this.TotalCount, this.HasPreviousPage, this.HasNextPage);
        }

        /// <summary>
        /// Convenience method to Wrap the current Page Slice as PreProcessedCursorSliceResults; to eliminate
        /// ceremonial code for new-ing up the results.
        /// </summary>
        /// <returns></returns>
        [Obsolete("It is now Recommended to simply use ToGraphQLConnection() for returning data from Hot Chocolate Resolvers instead;"
                    + " since HC has resolved internal bug(s), a Connection result will offer improved performance.")]
        public PreProcessedCursorSlice<TEntity> AsPreProcessedCursorSlice() => new(this);

        public Connection<TEntity> ToGraphQLConnection()
        {
            var edges = this.ToEdgeResults().ToList();

            var connectionPageInfo = new ConnectionPageInfo(
                hasNextPage: this.HasNextPage,
                hasPreviousPage: this.HasPreviousPage,
                startCursor: edges.FirstOrDefault()?.Cursor,
                endCursor: edges.LastOrDefault()?.Cursor
            );

            var graphQLConnection = new Connection<TEntity>(
                edges,
                connectionPageInfo,
                this.TotalCount ?? 0
            );

            return graphQLConnection;
        }

        private IEnumerable<IndexEdge<TEntity>> ToEdgeResults()
        {
            //Ensure we are null safe and return a valid empty list by default.
            //Note: We intentionally do NOT call ToList() here so that consuming classes may provide additional filtering...
            var results = this.CursorResults
                ?.Where(cr => cr != null)
                .Select(cr => IndexEdge<TEntity>.Create(cr.Entity, cr.CursorIndex))
            ?? Enumerable.Empty<IndexEdge<TEntity>>();

            return results;
        }
    }
}
