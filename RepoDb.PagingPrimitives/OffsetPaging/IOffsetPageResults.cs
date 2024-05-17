using System;
using System.Collections.Generic;
using RepoDb.PagingPrimitives.Common;

namespace RepoDb.PagingPrimitives.OffsetPaging

{
    /// <summary>
    /// Interface representing a Decorator class for a set of Page of results any TEntity model provided.  
    /// This class represents a set of results/nodes of offset pagination.
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    public interface IOffsetPageResults<out TEntity> : IPageNavigationInfo
    {
        /// <summary>
        /// A readonly enumerable list of the base non-decorated TEntity results for this page.
        /// </summary>
        IReadOnlyList<TEntity> Results { get; }

        /// <summary>
        /// The Ordinal position index for the first item in the results of this page; can be used for forward or backward offset navigation via skip/take.
        /// </summary>
        int StartIndex { get; }

        /// <summary>
        /// The Ordinal position index for the last item in the results of this page; can be used for forward or backward offset navigation via skip/take.
        /// </summary>
        int EndIndex { get; }

        /// <summary>
        /// Convenience method to easily cast all types in the current page to another compatible type.
        /// Type mismatches will be safely ignored and not returned for behaviour matching Linq OfType().
        /// </summary>
        /// <typeparam name="TTargetType"></typeparam>
        /// <returns></returns>
        OffsetPageResults<TTargetType> OfType<TTargetType>();

        /// <summary>
        /// Convenience method to easily map/convert/project all types in the current page to a different object type
        /// altogether, without losing the decorator paging details.
        /// </summary>
        /// <typeparam name="TTargetType"></typeparam>
        /// <param name="mappingFunc"></param>
        /// <returns></returns>
        OffsetPageResults<TTargetType> AsMappedType<TTargetType>(Func<TEntity, TTargetType> mappingFunc);
    }
}