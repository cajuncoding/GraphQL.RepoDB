using System;

namespace HotChocolate.PreProcessingExtensions.Pagination
{
    public interface IHavePreProcessedPagingInfo : IAmPreProcessedResult
    {
        public int? TotalCount { get; }
        public bool HasNextPage { get; }
        public bool HasPreviousPage { get; }
    }
}
