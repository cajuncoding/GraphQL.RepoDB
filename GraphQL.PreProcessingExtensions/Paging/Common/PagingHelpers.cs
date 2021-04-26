using System;
using HotChocolate.Types.Pagination;

namespace GraphQL.PreProcessingExtensions.Paging
{
    public static class PagingHelpers
    {
        /// <summary>
        /// Simplified helper to get the initialize a PagingOptions with all Default values from HC existing constants.
        /// </summary>
        /// <returns></returns>
        public static PagingOptions GetDefaultPagingOptions()
        {
            return new PagingOptions()
            {
                DefaultPageSize = PagingDefaults.DefaultPageSize,
                IncludeTotalCount = PagingDefaults.IncludeTotalCount,
                MaxPageSize = PagingDefaults.MaxPageSize
            };
        }

        /// <summary>
        /// Custom Extension to easily clone existing PagingOptions (helps to keep things immutable)
        /// </summary>
        /// <param name="pagingOptions"></param>
        /// <returns></returns>
        public static PagingOptions ClonePagingOptions(this PagingOptions pagingOptions)
        {
            return new PagingOptions()
            {
                DefaultPageSize = pagingOptions.DefaultPageSize,
                IncludeTotalCount = pagingOptions.IncludeTotalCount,
                MaxPageSize = pagingOptions.MaxPageSize
            };
        }
    }
}
