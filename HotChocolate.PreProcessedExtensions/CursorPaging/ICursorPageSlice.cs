using System;
using System.Collections.Generic;

namespace HotChocolate.PreProcessedExtensions.Pagination
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
    }
}