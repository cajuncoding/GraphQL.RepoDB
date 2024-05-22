namespace RepoDb.PagingPrimitives.Common
{

    /// <summary>
    /// Interface representing a Decorator class for common Paging navigation info.
    /// </summary>
    public interface ICursorPageNavigationInfo : IPageNavigationInfo
    {
        /// <summary>
        /// The Cursor for the first item in the results of this page; can be used for forward or backward cursor navigation via first/after or last/before.
        /// </summary>
        string StartCursor { get; }

        /// <summary>
        /// The Cursor for the last item in the results of this page; can be used for forward or backward cursor navigation via first/after or last/before.
        /// </summary>
        string EndCursor { get; }
    }
}