using System;

namespace HotChocolate.PreProcessedExtensions.Pagination
{
    public interface IHavePreProcessedPagingInfo
    {
        public int? TotalCount { get; }
        public bool HasNextPage { get; }
        public bool HasPreviousPage { get; }
    }
}
