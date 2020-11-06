# nullable enable

using HotChocolate.Types.Pagination;
using System;

namespace RepoDb.CursorPagination
{
    /// <summary>
    /// RepoDb specific class representing Cursor Paging parameteres. This provides a specific class for RepoDb
    /// that is isolated from HotChocolate and other libraries, similar to Field, OrderField, QueryField provide
    /// RepoDb specific context for those elements of a query.
    /// </summary>
    public class RepoDbCursorPagingParams : IRepoDbCursorPagingParams
    {
        public RepoDbCursorPagingParams(int? first = null, int? last = null, string? after = null, string? before = null)
        {
            this.First = first;
            this.Last = last;
            this.After = after;
            this.Before = before;
            this.AfterIndex = DeserializeCursor(after);
            this.BeforeIndex = DeserializeCursor(before);
        }

        public static string? SerializeCursor(int? index)
        {
            return index != null
                ? IndexEdge<string>.Create(String.Empty, (int)index)?.Cursor
                : default;
        }

        public static int? DeserializeCursor(string? cursor)
        {
            return cursor != null
                ? IndexEdge<string>.DeserializeCursor(cursor)
                : (int?)null;
        }

        public int? First { get; }
        public int? Last { get; }

        public string? After { get; }
        public int? AfterIndex { get; }
        public string? Before { get; }
        public int? BeforeIndex { get; }
    }
}
