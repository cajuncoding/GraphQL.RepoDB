using System;
using System.Collections.Generic;
using System.Linq;

namespace HotChocolate.PreProcessedExtensions.Pagination
{
    public class CursorPageSlice<TEntity> : ICursorPageSlice<TEntity> where TEntity : class
    {
        public CursorPageSlice(IEnumerable<ICursorResult<TEntity>> results, int totalCount)
        {
            this.CursorResults = results;
            this.TotalCount = totalCount;

            var firstCursor = results?.FirstOrDefault();
            var lastCursor = results?.LastOrDefault();

            //Now we can deduce if there are results before or after this slice based on the total count
            //  and the ordinal index of the first and last cursors.
            this.HasNextPage = lastCursor?.CursorIndex < this.TotalCount; //Cursor Index is 1 Based; the Count will match the Last Item
            this.HasPreviousPage = firstCursor?.CursorIndex > 1; //Cursor Index is 1 Based; 0 would be the Cursor before the First
        }

        public IEnumerable<ICursorResult<TEntity>> CursorResults { get; protected set; }

        public IEnumerable<TEntity> Results => CursorResults?.Select(cr => cr?.Entity);

        public CursorPageSlice<TTargetType> OfType<TTargetType>() where TTargetType : class
        {
            var results = this.CursorResults?.Select(cr => {
                if (cr?.Entity is TTargetType)
                    return new CursorResult<TTargetType>(cr.Entity as TTargetType, cr.CursorIndex);
                else
                    return null;
            })
            .Where(cr => cr != null);
            
            return new CursorPageSlice<TTargetType>(results, (int)this.TotalCount);
        }

        public int? TotalCount { get; protected set; }

        public bool HasNextPage { get; protected set; }

        public bool HasPreviousPage { get; protected set; }
    }
}
