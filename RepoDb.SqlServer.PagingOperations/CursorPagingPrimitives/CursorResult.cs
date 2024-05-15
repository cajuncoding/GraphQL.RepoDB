﻿using RepoDb.SqlServer.PagingOperations;

namespace RepoDb.CursorPaging
{
    public class CursorResult<TEntity> : ICursorResult<TEntity>
    {
        public CursorResult(TEntity entity, int cursorIndex)
        {
            this.CursorIndex = cursorIndex;
            this.Cursor = RepoDbCursorHelper.CreateCursor(cursorIndex);
            this.Entity = entity;
        }

        public TEntity Entity { get; }
        public int CursorIndex { get; }
        public string Cursor { get; }
    }
}
