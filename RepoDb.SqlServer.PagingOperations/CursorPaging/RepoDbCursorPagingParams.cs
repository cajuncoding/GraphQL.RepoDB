namespace RepoDb.CursorPaging
{
    /// <summary>
    /// RepoDb specific class representing Cursor Paging parameters. This provides a specific class for RepoDb
    /// that is isolated from HotChocolate and other libraries, similar to Field, OrderField, QueryField provide
    /// RepoDb specific context for those elements of a query.
    /// </summary>
    public class RepoDbCursorPagingParams : IRepoDbCursorPagingParams
    {
        public RepoDbCursorPagingParams(int? first = null, int? last = null, string afterCursor = null, string beforeCursor = null, bool isTotalCountRequested = false)
        {
            this.First = first;
            this.Last = last;
            this.After = afterCursor;
            this.Before = beforeCursor;
            this.AfterIndex = DeserializeCursor(afterCursor);
            this.BeforeIndex = DeserializeCursor(beforeCursor);
            this.IsTotalCountRequested = isTotalCountRequested;
        }

        public RepoDbCursorPagingParams(int? after = null, int? first = null, int? before = null, int ? last = null, bool isTotalCountRequested = false)
        {
            this.First = first;
            this.Last = last;
            this.AfterIndex = after;
            this.BeforeIndex = before;
            this.After = SerializeCursor(after);
            this.Before = SerializeCursor(before);
            this.IsTotalCountRequested = isTotalCountRequested;
        }

        public static string SerializeCursor(int? index)
        {
            return index != null
                ? RepoDbCursorHelper.CreateCursor((int)index) //IndexEdge<string>.Create(String.Empty, (int)index)?.Cursor
                : default;
        }

        public static int? DeserializeCursor(string cursor)
        {
            return cursor != null
                ? RepoDbCursorHelper.ParseCursor(cursor) //IndexEdge<string>.DeserializeCursor(cursor)
                : (int?)null;
        }

        public int? First { get; }
        public int? Last { get; }

        public string After { get; }
        public int? AfterIndex { get; }
        public string Before { get; }
        public int? BeforeIndex { get; }
        public bool IsTotalCountRequested { get; }
    }
}
