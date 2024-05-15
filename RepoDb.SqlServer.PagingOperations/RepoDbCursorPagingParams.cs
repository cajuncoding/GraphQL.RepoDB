using RepoDb.CursorPaging;

namespace RepoDb.SqlServer.PagingOperations
{
    /// <summary>
    /// RepoDb specific class representing Cursor Paging parameters. This provides a specific class for RepoDb
    /// that is isolated from HotChocolate and other libraries, similar to Field, OrderField, QueryField provide
    /// RepoDb specific context for those elements of a query.
    /// </summary>
    public class RepoDbCursorPagingParams : IRepoDbCursorPagingParams
    {
        private static readonly int? NullInt = (int?)null;
        private static readonly string NullString = (string)null;

        public RepoDbCursorPagingParams(int? first = null, int? last = null, string afterCursor = null, string beforeCursor = null, bool isTotalCountRequested = false)
        {
            First = first;
            Last = last;
            After = afterCursor;
            Before = beforeCursor;
            AfterIndex = DeserializeCursor(afterCursor);
            BeforeIndex = DeserializeCursor(beforeCursor);
            IsTotalCountRequested = isTotalCountRequested;
        }

        public RepoDbCursorPagingParams(int? after = null, int? first = null, int? before = null, int? last = null, bool isTotalCountRequested = false)
        {
            First = first;
            Last = last;
            AfterIndex = after;
            BeforeIndex = before;
            After = SerializeCursor(after);
            Before = SerializeCursor(before);
            IsTotalCountRequested = isTotalCountRequested;
        }

        public static string SerializeCursor(int? index) => index != null
            ? RepoDbCursorHelper.CreateCursor((int)index) //IndexEdge<string>.Create(String.Empty, (int)index)?.Cursor
            : NullString;

        public static int? DeserializeCursor(string cursor) => cursor != null
            ? RepoDbCursorHelper.ParseCursor(cursor) //IndexEdge<string>.DeserializeCursor(cursor)
            : NullInt;

        public int? First { get; }
        public int? Last { get; }

        public string After { get; }
        public int? AfterIndex { get; }
        public string Before { get; }
        public int? BeforeIndex { get; }
        public bool IsTotalCountRequested { get; }
    }
}
