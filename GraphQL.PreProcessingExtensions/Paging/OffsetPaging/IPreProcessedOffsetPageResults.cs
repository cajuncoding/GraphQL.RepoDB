using System.Collections.Generic;
using HotChocolate.Types.Pagination;

namespace HotChocolate.PreProcessingExtensions.Pagination
{
    public interface IPreProcessedOffsetPageResults<TEntity> : IList<TEntity>, IHavePreProcessedPagingInfo, IAmPreProcessedResult
    {
    }
}