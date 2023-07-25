using System;

namespace HotChocolate.PreProcessingExtensions.Pagination
{
    public class CursorResult<TEntity> : ICursorResult<TEntity>
    {
        public CursorResult(TEntity entity, int cursorIndex)
        {
            this.CursorIndex = cursorIndex;
            this.Entity = entity;
        }

        public TEntity Entity { get; }
        public int CursorIndex { get; }
    }
}
