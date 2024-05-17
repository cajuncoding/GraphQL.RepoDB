﻿using RepoDb.SqlServer.PagingOperations;

namespace RepoDb.PagingPrimitives.CursorPaging
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

        public RepoDbCursorPagingParams(int? first = null, int? last = null, string afterCursor = null, string beforeCursor = null, bool retrieveTotalCount = false)
        {
            First = first;
            Last = last;
            After = afterCursor;
            Before = beforeCursor;
            AfterIndex = DeserializeCursor(afterCursor);
            BeforeIndex = DeserializeCursor(beforeCursor);
            IsTotalCountRequested = retrieveTotalCount;
        }

        public RepoDbCursorPagingParams(int? firstTake = null, int? lastTake = null, int? afterIndex = null, int? beforeIndex = null, bool retrieveTotalCount = false)
        {
            First = firstTake;
            Last = lastTake;
            AfterIndex = afterIndex;
            BeforeIndex = beforeIndex;
            After = SerializeCursor(afterIndex);
            Before = SerializeCursor(beforeIndex);
            IsTotalCountRequested = retrieveTotalCount;
        }

        public static RepoDbCursorPagingParams ForCursors(int? first = null, int? last = null, string afterCursor = null, string beforeCursor = null, bool retrieveTotalCount = false)
            => new RepoDbCursorPagingParams(first, last, afterCursor, beforeCursor, retrieveTotalCount);

        public static RepoDbCursorPagingParams ForIndexes(int? first = null, int? last = null, int? afterIndex = null, int? beforeIndex = null, bool retrieveTotalCount = false)
            => new RepoDbCursorPagingParams(first, last, afterIndex, beforeIndex, retrieveTotalCount);

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
