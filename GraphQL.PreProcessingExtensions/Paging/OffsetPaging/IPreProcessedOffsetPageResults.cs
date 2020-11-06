using System.Collections.Generic;

namespace HotChocolate.PreProcessingExtensions.Pagination
{
    public interface IPreProcessedOffsetPageResults<TEntity> : IEnumerable<TEntity>
    {
        bool HasNextPage { get; }
        bool HasPreviousPage { get; }
        int? TotalCount { get; }
    }
}