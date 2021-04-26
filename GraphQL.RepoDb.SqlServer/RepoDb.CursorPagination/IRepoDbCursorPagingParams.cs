using System;

namespace RepoDb.CursorPagination
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