using System;
using System.Collections.Generic;
using System.Linq;

namespace HotChocolate.PreProcessingExtensions.Pagination
{
    /// <summary>
    /// Model class for representing a paging result set that was computed using Cursor Pagination process by offering
    /// a default implementation of the ICursorPageSlice interface which de-couples the code that executes queries 
    /// from the actual PreProcessing extension classes used for the HotChocolate.
    /// This class generally to be used by libraries and/or lower level code that executes queries and renders page results.
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    public class CursorPageSlice<TEntity> : ICursorPageSlice<TEntity> where TEntity : class
    {
        private int _totalCount;
        
        public CursorPageSlice(IEnumerable<ICursorResult<TEntity>> results, int totalCount)
        {
            this.CursorResults = (results ?? throw new ArgumentException(nameof(results))).ToList();
            _totalCount = totalCount;

            var firstCursor = this.CursorResults.FirstOrDefault();
            var lastCursor = this.CursorResults.LastOrDefault();

            //Now we can deduce if there are results before or after this slice based on the total count
            //  and the ordinal index of the first and last cursors.
            this.HasNextPage = lastCursor?.CursorIndex < _totalCount; //Cursor Index is 1 Based; the Count will match the Last Item
            this.HasPreviousPage = firstCursor?.CursorIndex > 1; //Cursor Index is 1 Based; 0 would be the Cursor before the First
        }

        public IEnumerable<ICursorResult<TEntity>> CursorResults { get; protected set; }

        public IEnumerable<TEntity> Results => CursorResults?.Select(cr => cr?.Entity);

        public int? TotalCount => _totalCount;

        public bool HasNextPage { get; protected set; }

        public bool HasPreviousPage { get; protected set; }
        /// <summary>
        /// Convenience method to easily cast all types in the current page to a garget compatible type
        /// without affecting the cursor indexes, etc. Provide deferred execution via Linq Select(). Type mismatches
        /// will be ignored and not returned for behaviour matching Linq OfType().
        /// </summary>
        /// <typeparam name="TTargetType"></typeparam>
        /// <returns></returns>
        public CursorPageSlice<TTargetType> OfType<TTargetType>() where TTargetType : class
        {
            var results = this.CursorResults?.Select(r => {
                    // ReSharper disable once ConvertToLambdaExpression
                    return r?.Entity is TTargetType targetType
                    ? new CursorResult<TTargetType>(targetType, r.CursorIndex)
                    : null;
            })
            .Where(cr => cr != null);
            
            return new CursorPageSlice<TTargetType>(results, _totalCount);
        }

        /// <summary>
        /// Convenience method to easily map/convert/project all types in the current page to a different object type
        /// altogether, without affecting the cursor indexes, etc. Provide deferred execution via Linq Select().
        /// </summary>
        /// <typeparam name="TTargetType"></typeparam>
        /// <param name="mappingFunc">Specify the Func that takes the current type in and returns the target type.</param>
        /// <returns></returns>
        public CursorPageSlice<TTargetType> AsMappedType<TTargetType>(Func<TEntity, TTargetType> mappingFunc) where TTargetType : class
        {
            var results = this.CursorResults?.Select(r =>
            {
                var mappedEntity = mappingFunc(r.Entity);
                return new CursorResult<TTargetType>(mappedEntity, r.CursorIndex);
            });

            return new CursorPageSlice<TTargetType>(results, _totalCount);
        }

        /// <summary>
        /// Convenience method to Wrap the current Page Slice as PreProcessedCursorSliceResults; to eliminate
        /// ceremonial code for new-ing up the results.
        /// </summary>
        /// <returns></returns>
        public PreProcessedCursorSlice<TEntity> AsPreProcessedCursorSlice()
        {
            return new PreProcessedCursorSlice<TEntity>(this);
        }
    }
}
