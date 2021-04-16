using System;
using System.Collections.Generic;

namespace HotChocolate.PreProcessingExtensions.Pagination
{

    /// <summary>
    /// Interface representing a Decorator class for a Page/Set/Slice of results any TEntity model provided.  
    /// This class represents a set of results/nodes of an edge/slice/page.
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    public interface ICursorPageSlice<TEntity> : IList<ICursorResult<TEntity>>, IHavePreProcessedPagingInfo
    {
        /// <summary>
        /// An enumerable list of the base non-decorated TEntity values.
        /// </summary>
        IEnumerable<TEntity> Results { get; }

        /// <summary>
        /// Support safe (deferred) casting to the specified Entity Type.
        /// </summary>
        /// <typeparam name="TTargetType"></typeparam>
        /// <returns></returns>
        CursorPageSlice<TTargetType> OfType<TTargetType>();

        /// <summary>
        /// Convenience method to easily map/convert/project all types in the current page to a different object type
        /// altogether, without affecting the cursor indexes, etc. Provide deferred execution via Linq Select().
        /// </summary>
        /// <typeparam name="TTargetType"></typeparam>
        /// <param name="mappingFunc"></param>
        /// <returns></returns>
        CursorPageSlice<TTargetType> AsMappedType<TTargetType>(Func<TEntity, TTargetType> mappingFunc);

        /// <summary>
        /// Convenience method to Wrap the current Page Slice as PreProcessedCursorSliceResults; to eliminate
        /// ceremonial code for new-ing up the results.
        /// </summary>
        /// <returns></returns>
        PreProcessedCursorSlice<TEntity> AsPreProcessedCursorSlice();
    }
}