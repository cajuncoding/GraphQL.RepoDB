using HotChocolate.Types.Pagination;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HotChocolate.PreProcessingExtensions.Pagination
{
    /// <summary>
    /// Payload response container for HotChocolate pipeline that contains all details needed for pre-processed
    /// results to integrate with the existing OOTB paging pipeline, but with results that are already completely
    /// processed by the Resolver (or lower layer).
    /// 
    /// We must inherit from List<TEntity> to ensure that HotChocolate can correctly Infer the proper Schema from 
    /// the base TEntity generic type; simply providing IEnumerable<TEntity> isn't enough.  
    /// As a real List<> the PureCode Schema inference works as expected!
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    public class PreProcessedCursorSlice<TEntity> : List<TEntity>, IEnumerable<TEntity>, IPreProcessedCursorSlice<TEntity>
        where TEntity : class
    {
        public PreProcessedCursorSlice(ICursorPageSlice<TEntity> pageSlice)
        {
            this.CursorPage = pageSlice ?? throw new ArgumentNullException(nameof(pageSlice));
            this.TotalCount = pageSlice?.TotalCount ?? 0;

            var firstCursor = pageSlice?.CursorResults?.FirstOrDefault();
            var lastCursor = pageSlice?.CursorResults?.LastOrDefault();

            //Now we can deduce if there are results before or after this slice based on the total count
            //  and the ordinal index of the first and last cursors.
            this.HasNextPage = lastCursor?.CursorIndex < this.TotalCount; //Cursor Index is 1 Based; the Count will match the Last Item
            this.HasPreviousPage = firstCursor?.CursorIndex > 1; //Cursor Index is 1 Based; 0 would be the Cursor before the First

            if(pageSlice?.Results != null)
                this.AddRange(pageSlice.Results);
        }

        public ICursorPageSlice<TEntity> CursorPage { get; protected set; }
        
        public int? TotalCount { get; protected set; }

        public bool HasNextPage { get; protected set; }

        public bool HasPreviousPage { get; protected set; }

        public IEnumerable<IndexEdge<TEntity>> ToEdgeResults()
        {
            //The Linq Selection provides IEnumerable for us...
            //Note: thats why we do NOT call ToList() here so that consuming classes may provide additional filtering...
            var results = this.CursorPage?.CursorResults
                .Where(cr => cr != null)
                .Select(cr => IndexEdge<TEntity>.Create(cr.Entity, cr.CursorIndex));

            return results;
        }
    }
}
