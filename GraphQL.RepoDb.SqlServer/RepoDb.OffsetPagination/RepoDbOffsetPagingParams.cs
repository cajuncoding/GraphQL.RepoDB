# nullable enable

using System;

namespace RepoDb.CursorPagination
{
    /// <summary>
    /// RepoDb specific class representing Cursor Paging parameteres. This provides a specific class for RepoDb
    /// that is isolated from HotChocolate and other libraries, similar to Field, OrderField, QueryField provide
    /// RepoDb specific context for those elements of a query.
    /// </summary>
    public class RepoDbOffsetPagingParams : IRepoDbOffsetPagingParams
    {
        /// <summary>
        /// Initialize Batch paging parameters safely; if null values are specified the default behaviour
        ///     will be to retrieve all results by initializing Page to 0 and batch size to int.MaxValue.
        /// </summary>
        /// <param name="rowsPerBatch">May be null to get all results, otherwise must be a valid positive value</param>
        /// <param name="page">Is Optional and will default to the first page (zero-based)</param>
        public RepoDbOffsetPagingParams(int? rowsPerBatch = null, int? page = null)
        {
            if (rowsPerBatch.HasValue && rowsPerBatch <= 0)
                throw new ArgumentException("A valid number of rows per batch must be specified (>0).", nameof(rowsPerBatch));

            this.RowsPerBatch = rowsPerBatch.HasValue && rowsPerBatch > 0 ? (int)rowsPerBatch : int.MaxValue;
            this.Page = page.HasValue && page > 0 ? (int)page : 0;
        }

        /// <summary>
        /// Initialize Batch paging parameters safely; if null values are specified the default behaviour
        ///     will be to retrieve all results by initializing Page to 0 and batch size to int.MaxValue.
        /// </summary>
        /// <param name="take">May be null to get all results, otherwise must be a valid positive value</param>
        /// <param name="skip">Is Optional and will default to skipping none (0) for the first page.</param>
        /// <returns></returns>
        public static RepoDbOffsetPagingParams FromSkipTake(int? take = null, int? skip = null)
        {
            if (take.HasValue && take <= 0)
                throw new ArgumentException("A valid number of items to take (rows-per-page) must be specified (>0).", nameof(take));

            //The RepoDb specific rows-per-batch is the same value as the Take value; but we allow null values to
            //  default to retrieving all results by using int.MaxValue (this matches the default behavior of
            //  Cursor based paging which retrieves all results if not specified.
            var rowsPerBatch = take.HasValue && take > 0 ? (int)take : int.MaxValue;

            //Dynamically derive a Page number from the Skip & Take values as RepoDb specific 
            //  Page value (which is zero-based).
            var page = (int)(skip ?? 0) / take;

            return new RepoDbOffsetPagingParams(rowsPerBatch, page);
        }

        public int Page { get; }
        public int RowsPerBatch { get; }
    }
}
