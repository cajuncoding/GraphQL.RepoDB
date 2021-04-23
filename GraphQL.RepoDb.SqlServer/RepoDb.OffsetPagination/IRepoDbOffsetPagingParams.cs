namespace RepoDb.CursorPagination
{
    public interface IRepoDbOffsetPagingParams
    {
        int Page { get; }
        int RowsPerBatch { get; }
        bool IsTotalCountEnabled { get; }
    }
}