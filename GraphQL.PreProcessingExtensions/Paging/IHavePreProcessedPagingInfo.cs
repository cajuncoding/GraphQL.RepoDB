using HotChocolate.Types.Pagination;
using System;

namespace HotChocolate.PreProcessingExtensions.Pagination
{
    public interface IHavePreProcessedPagingInfo
    {
        public int? TotalCount { get; }
        public bool HasNextPage { get; }
        public bool HasPreviousPage { get; }
    }
}
