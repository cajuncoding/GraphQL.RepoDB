using System;
using System.Collections.Generic;
using RepoDb.CommonPaging;

namespace RepoDb.OffsetPaging

{
    /// <summary>
    /// Interface representing a Decorator class for a set of Page of results any TEntity model provided.  
    /// This class represents a set of results/nodes of offset pagination.
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    public interface IOffsetPageResults<out TEntity> : IPageNavigationInfo
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
        OffsetPageResults<TTargetType> OfType<TTargetType>();

        /// <summary>
        /// Convenience method to easily map/convert/project all types in the current page to a different object type
        /// altogether, without losing the decorator paging details; Provide deferred execution via Linq Select().
        /// </summary>
        /// <typeparam name="TTargetType"></typeparam>
        /// <param name="mappingFunc"></param>
        /// <returns></returns>
        OffsetPageResults<TTargetType> AsMappedType<TTargetType>(Func<TEntity, TTargetType> mappingFunc);
    }
}