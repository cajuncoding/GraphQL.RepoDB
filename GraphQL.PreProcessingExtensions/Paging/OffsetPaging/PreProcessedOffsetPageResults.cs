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
    public class PreProcessedOffsetPageResults<TEntity> : List<TEntity>, IPreProcessedOffsetPageResults<TEntity>
        where TEntity : class
    {
        public PreProcessedOffsetPageResults(IOffsetPageResults<TEntity> pageResults)
        {
            if (pageResults == null)
                throw new ArgumentNullException(nameof(pageResults));

            this.TotalCount = pageResults.TotalCount;
            this.HasNextPage = pageResults.HasNextPage;
            this.HasPreviousPage = pageResults.HasPreviousPage;

            if (pageResults.Results.Any())
                this.AddRange(pageResults.Results);
        }

        public int? TotalCount { get; protected set; }

        public bool HasNextPage { get; protected set; }

        public bool HasPreviousPage { get; protected set; }
    }
}
