namespace RepoDb.PagingPrimitives.CursorPaging
{
    public class CursorResult<TEntity> : ICursorResult<TEntity>
    {
        public CursorResult(TEntity entity, string cursor)
        {
            this.Cursor = cursor;
            this.Entity = entity;
        }

        public TEntity Entity { get; }
        public string Cursor { get; }

        internal int CursorIndex { get; private set; }

        internal static CursorResult<TEntity> CreateIndexedCursor(TEntity entity, string cursor, int cursorIndex)
        {
            var cursorResult = new CursorResult<TEntity>(entity, cursor)
            {
                CursorIndex = cursorIndex
            };
            return cursorResult;
        }
    }
}
