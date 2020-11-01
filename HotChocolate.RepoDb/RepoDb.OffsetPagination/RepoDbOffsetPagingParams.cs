# nullable enable

using HotChocolate.Types.Pagination;
using System;

namespace RepoDb.CursorPagination
{
    /// <summary>
    /// RepoDb specific class representing Cursor Paging parameteres. This provides a specific class for RepoDb
    /// that is isolated from HotChocolate and other libraries, similar to Field, OrderField, QueryField provide
    /// RepoDb specific context for those elements of a query.
    /// </summary>
    public class RepoDbOffsetPagingParams
    {
        public RepoDbOffsetPagingParams(int rowsPerBatch, int page=0)
        {
            this.RowsPerBatch = rowsPerBatch;
            this.Page = page;
        }

        public static RepoDbOffsetPagingParams FromSkipTake(int take, int? skip)
        {
            var rowsPerBatch = take;
            var page = (int)(skip ?? 0) / take;
            return new RepoDbOffsetPagingParams(rowsPerBatch, page);
        }

        public int? Page { get; }
        public int? RowsPerBatch { get; }
    }
}
