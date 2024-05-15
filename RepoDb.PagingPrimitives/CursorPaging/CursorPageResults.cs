using System;
using System.Collections.Generic;
using System.Linq;

namespace RepoDb.PagingPrimitives.CursorPaging
{
    /// <summary>
    /// Model class for representing a paging result set that was computed using Cursor Pagination process by offering
    /// a default implementation of the ICursorPageResult interface.
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    public class CursorPageResults<TEntity> : ICursorPageResults<TEntity>
    {
        public CursorPageResults(IEnumerable<ICursorResult<TEntity>> results, int? totalCount, bool hasPreviousPage, bool hasNextPage)
        {
            this.CursorResults = results ?? throw new ArgumentNullException(nameof(results));
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
        public virtual CursorPageResults<TTargetType> OfType<TTargetType>()
        {
            var enumerableResults = this.CursorResults
                .Select(r =>
                {
                    // ReSharper disable once ConvertToLambdaExpression
                    return r.Entity is TTargetType entityAsTargetType
                        ? new CursorResult<TTargetType>(entityAsTargetType, r.Cursor)
                        : null;
                })
                .Where(c => c != null);

            return new CursorPageResults<TTargetType>(enumerableResults, this.TotalCount, this.HasPreviousPage, this.HasNextPage);
        }

        /// <summary>
        /// Convenience method to easily map/convert/project all types in the current page to a different object type
        /// altogether, without affecting the cursor indexes, etc. Provide deferred execution via Linq Select().
        /// </summary>
        /// <typeparam name="TTargetType"></typeparam>
        /// <param name="mappingFunc">Specify the Func that takes the current type in and returns the target type.</param>
        /// <returns></returns>
        public virtual CursorPageResults<TTargetType> AsMappedType<TTargetType>(Func<TEntity, TTargetType> mappingFunc)
        {
            if (mappingFunc == null) 
                throw new ArgumentException(nameof(mappingFunc));

            var results = this.CursorResults?.Select(r =>
            {
                var mappedEntity = mappingFunc(r.Entity);
                return new CursorResult<TTargetType>(mappedEntity, r.Cursor);
            });

            return new CursorPageResults<TTargetType>(results, this.TotalCount, this.HasPreviousPage, this.HasNextPage);
        }
    }
}
