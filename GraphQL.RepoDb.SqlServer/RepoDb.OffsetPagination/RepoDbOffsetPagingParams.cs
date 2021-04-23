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
        /// <param name="rowsPerBatch">Is Optional and may be null to get all results; will default to getting all data (int.MaxValue) if null or is not a valid positive value.</param>
        /// <param name="page">Is Optional and will default to the first page (zero-based) if null or is not a valid positive value.</param>
        /// <param name="fetchTotalCount">Enable the retrieval of the Total Count; for OffsetPaging this is optional and enabling it may impact performance.</param>
        public RepoDbOffsetPagingParams(int? rowsPerBatch = null, int? page = null, bool fetchTotalCount = false)
        {
            if (rowsPerBatch.HasValue && rowsPerBatch <= 0)
                throw new ArgumentException("A valid number of rows per batch must be specified (>0).", nameof(rowsPerBatch));

            this.RowsPerBatch = rowsPerBatch.HasValue && rowsPerBatch > 0 ? (int)rowsPerBatch : int.MaxValue;
            this.Page = page.HasValue && page > 0 ? (int)page : 0;
            this.IsTotalCountEnabled = fetchTotalCount;
        }

        /// <summary>
        /// Initialize Batch paging parameters safely; if null values are specified the default behaviour
        ///     will be to retrieve all results by initializing Page to 0 and batch size to int.MaxValue.
        /// </summary>
        /// <param name="skip">Is Optional and may be null to get all results; will default to skipping none (0) if null or is not a valid positive value.</param>
        /// <param name="take">Is Optional and may be null to get all results; will default to getting all data (int.MaxValue) if null or is not a valid positive value.</param>
        /// <param name="fetchTotalCount">Enable the retrieval of the Total Count; for OffsetPaging this is optional and enabling it may impact performance.</param>
        /// <returns></returns>
        public static RepoDbOffsetPagingParams FromSkipTake(int? skip = null, int? take = null, bool fetchTotalCount = false)
        {
            if (take.HasValue && take <= 0)
                throw new ArgumentException("A valid number of items to take (rows-per-page) must be specified (e.g. greater than 0).", nameof(take));

            //The RepoDb specific rows-per-batch is the same value as the Take value; but we allow null values which
            //  default to retrieving all results by using int.MaxValue; this matches the default behavior of
            //  Cursor based paging which retrieves all results if not specified.
            var rowsPerBatch = take.HasValue && take > 0 ? (int)take : int.MaxValue;

            //Dynamically derive a Page number from the Skip & Take values as RepoDb specific 
            //  Page value (which is zero-based).
            var skipOver = Math.Max((skip ?? 0), 0);
            var page = (int)(skipOver / rowsPerBatch);

            return new RepoDbOffsetPagingParams(rowsPerBatch, page, fetchTotalCount);
        }

        public int Page { get; }
        public int RowsPerBatch { get; }
        public bool IsTotalCountEnabled { get; }
    }
}
