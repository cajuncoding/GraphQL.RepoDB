using System;

namespace HotChocolate.PreProcessingExtensions.Pagination
{
    public class CursorResult<TEntity> : ICursorResult<TEntity>
    {
        protected string _opaqueCursor;

        public TEntity Entity { get; }
        public int CursorIndex { get; }
        
        public CursorResult(TEntity entity, int cursorIndex)
        {
            this.CursorIndex = cursorIndex;
            this.Entity = entity;
        }
    }
}
