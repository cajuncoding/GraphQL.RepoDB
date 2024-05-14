namespace RepoDb.CursorPaging
{
    public interface IRepoDbCursorPagingParams
    {
        string After { get; }
        int? AfterIndex { get; }
        string Before { get; }
        int? BeforeIndex { get; }
        int? First { get; }
        int? Last { get; }
        bool IsTotalCountRequested { get; }
    }
}