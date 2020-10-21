using HotChocolate.Types.Pagination;
using System;
using System.Collections.Generic;

namespace HotChocolate.PreProcessedExtensions.Pagination
{
    public interface IPreProcessedCursorSliceResults<TEntity> : IHavePagingInfo, IAmPreProcessedResult
    {
        public IEnumerable<IndexEdge<TEntity>> ToEdgeResults();
    }
}
