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
    [Obsolete("It is now Recommended to simply use ToGraphQLConnection() for directly returning a GraphQL Connection from Hot Chocolate Resolvers instead;"
              + " since HC has resolved internal bug(s), a Connection result will offer improved performance. This will likely be removed in future release"
              + " (especially once the new Paging features are available in a later version of v13.")]
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

        public Connection<TEntity> ToGraphQLConnection()
            => this.CursorPage.ToGraphQLConnection();
    }
}
