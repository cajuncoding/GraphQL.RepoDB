using HotChocolate.Types.Pagination;
using System;

namespace HotChocolate.ResolverProcessingExtensions.Pagination
{
    public interface IHaveResolverProcessedPagingInfo
    {
        public int? TotalCount { get; }
        public bool HasNextPage { get; }
        public bool HasPreviousPage { get; }
    }
}
