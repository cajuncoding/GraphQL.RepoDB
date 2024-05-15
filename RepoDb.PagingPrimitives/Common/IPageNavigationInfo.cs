namespace RepoDb.PagingPrimitives.Common
{

    /// <summary>
    /// Interface representing a Decorator class for common Paging navigation info.
    /// </summary>
    public interface IPageNavigationInfo
    {
        /// <summary>
        /// Optional value denoting the Total count of the full data-set for which this Page was retrieved.
        /// </summary>
        int? TotalCount { get; }

        /// <summary>
        /// Denotes if there are any Cursor pages after this current page within the full data-set for which this Page was retrieved.
        /// </summary>
        bool HasNextPage { get; }

        /// <summary>
        /// Denotes if there are any Cursor pages before this current page within the full data-set for which this Page was retrieved.
        /// </summary>
        bool HasPreviousPage { get; }
    }
}