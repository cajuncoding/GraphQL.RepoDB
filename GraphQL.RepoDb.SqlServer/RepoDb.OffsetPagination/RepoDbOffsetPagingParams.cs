# nullable enable

using System;

namespace RepoDb.OffsetPagination
{
    /// <summary>
    /// RepoDb specific class representing Offset Paging parameters. This provides a specific class for RepoDb
    /// that is isolated from HotChocolate and other libraries, similar to Field, OrderField, QueryField provide
    /// RepoDb specific context for those elements of a query.
    /// </summary>
    public class RepoDbOffsetPagingParams : IRepoDbOffsetPagingParams
    {
        /// <summary>
        /// Initialize Offset paging parameters safely; if null values are specified the default behaviour
        ///     will be to retrieve all results which is consistent with behavior of Cursor paging when no
        ///     paging filters are specified.
        /// </summary>
        /// <param name="skip">Is Optional and will default to the first page (or potentially all results based on Take) if null; but otherwise must be a valid positive value or an ArgumentException will occur.</param>
        /// <param name="take">Is Optional and may be null to get all results; but otherwise must be a valid positive value or an ArgumentException will occur.</param>
        public RepoDbOffsetPagingParams(int? skip = null, int? take = null)
        {
            //For RepoDb both Skip & Take may actually be optional so that would result in default behaviour to retrieve all results
            //  which is consistent with Cursor Paging as not specifying any filtering for Cursor Pagination would also get all results.
            //  Cursor based paging which retrieves all results if not specified.
            if (skip.HasValue && skip < 0)
                throw new ArgumentException("If specified (e.g. not null) then a valid number of items to skip-over must be specified (e.g. greater than or equal to 0).", nameof(skip));

            if (take.HasValue && take <= 0)
                throw new ArgumentException("If specified (e.g. not null) then A valid number of items to take (rows-per-page) must be specified (e.g. greater than 0).", nameof(take));

            this.Skip = skip;
            this.Take = take;
        }

        public int? Skip { get; }
        public int? Take { get; }
    }
}
