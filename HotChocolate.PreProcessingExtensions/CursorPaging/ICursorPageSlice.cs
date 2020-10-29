using System;
using System.Collections.Generic;

namespace HotChocolate.PreProcessingExtensions.Pagination
{

    /// <summary>
    /// Interface representing a Decorator class for a Page/Set/Slice of results any TEntity model provided.  
    /// This class represents a set of results/nodes of an edge/slice/page.
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    public interface ICursorPageSlice<TEntity> : IHavePreProcessedPagingInfo where TEntity : class
    {
        /// <summary>
        /// The set of all CursorResults decorator classes for TEntity model with Cursor Index values.
        /// </summary>
        IEnumerable<ICursorResult<TEntity>> CursorResults { get; }
        /// <summary>
        /// An enumerable list of the base non-decorated TEntity values.
        /// </summary>
        IEnumerable<TEntity> Results { get; }

        /// <summary>
        /// Support safe (deferred) casting to the specified Entity Type.
        /// </summary>
        /// <typeparam name="TTargetType"></typeparam>
        /// <returns></returns>
        CursorPageSlice<TTargetType> OfType<TTargetType>() where TTargetType : class;

        /// <summary>
        /// Convenience method to easily map/convert/project all types in the current page to a different object type
        /// altogether, without affecting the cursor indexes, etc. Provide deffered execution via Linq Select().
        /// </summary>
        /// <typeparam name="TTargetType"></typeparam>
        /// <param name="mappingFunc"></param>
        /// <returns></returns>
        CursorPageSlice<TTargetType> AsMappedType<TTargetType>(Func<TEntity, TTargetType> mappingFunc) where TTargetType : class;

        /// <summary>
        /// Conveniene method to Wrap the current Page Slice as PreProcessedCursorSliceResults; to eliminate
        /// cermenonial code for new'ing up the results.
        /// </summary>
        /// <returns></returns>
        IPreProcessedCursorSlice<TEntity> AsPreProcessedCursorSlice();
    }
}