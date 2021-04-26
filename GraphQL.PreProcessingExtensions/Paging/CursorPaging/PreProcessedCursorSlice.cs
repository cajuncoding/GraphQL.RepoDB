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
    public class PreProcessedCursorSlice<TEntity> : List<TEntity>, IPreProcessedCursorSlice<TEntity>
    {
        public PreProcessedCursorSlice(ICursorPageSlice<TEntity> pageSlice)
        {
            this.CursorPage = pageSlice ?? throw new ArgumentNullException(nameof(pageSlice));
            this.TotalCount = pageSlice.TotalCount;
            this.HasNextPage = pageSlice.HasNextPage;
            this.HasPreviousPage = pageSlice.HasPreviousPage;

            if(pageSlice.Results != null)
                this.AddRange(pageSlice.Results);
        }

        public ICursorPageSlice<TEntity> CursorPage { get; protected set; }
        
        public int? TotalCount { get; protected set; }

        public bool HasNextPage { get; protected set; }

        public bool HasPreviousPage { get; protected set; }

        public IEnumerable<IndexEdge<TEntity>> ToEdgeResults()
        {
            //The Linq Selection provides IEnumerable for us...
            //Note: that's why we do NOT call ToList() here so that consuming classes may provide additional filtering...
            var results = this.CursorPage?.CursorResults
                .Where(cr => cr != null)
                .Select(cr => IndexEdge<TEntity>.Create(cr.Entity, cr.CursorIndex));

            return results;
        }
    }
}
