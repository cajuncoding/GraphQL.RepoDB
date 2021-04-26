# nullable enable

using HotChocolate.Resolvers;
using HotChocolate.Types.Pagination;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GraphQL.PreProcessingExtensions.Paging;
using HotChocolate.Utilities;

namespace HotChocolate.PreProcessingExtensions.Pagination
{
    public class PreProcessedCursorPagingHandler<TEntity> : CursorPagingHandler
    {
        public PagingOptions PagingOptions { get; protected set; }

        public PreProcessedCursorPagingHandler(PagingOptions pagingOptions)
            : base(pagingOptions)
        {
            this.PagingOptions = pagingOptions.ClonePagingOptions();
        }

        /// <summary>
        /// Provides a No-Op (no post-processing) implementation so that results are unchanged from
        /// what was returned by the Resolver (or lower layers); assumes that the results are correctly
        /// pre-processed as a IPreProcessedPagedResult
        /// </summary>
        /// <param name="context"></param>
        /// <param name="source"></param>
        /// <param name="arguments"></param>
        /// <returns></returns>
        protected override ValueTask<Connection> SliceAsync(IResolverContext context, object source, CursorPagingArguments arguments)
        {
            //If Appropriate we handle the values here to ensure that no post-processing is done other than
            //  correctly mapping the results into a GraphQL Connection as Edges with Cursors...
            if (source is IPreProcessedCursorSlice<TEntity> pagedResults)
            {
                bool includeTotalCountEnabled = this.PagingOptions.IncludeTotalCount ?? PagingDefaults.IncludeTotalCount;
                var graphQLParamsContext = new GraphQLParamsContext(context);

                //Optimized to only require TotalCount value if the query actually requested it!
                if (includeTotalCountEnabled && graphQLParamsContext.IsTotalCountRequested && pagedResults.TotalCount == null)
                    throw new InvalidOperationException($"Total Count is requested in the query, but was not provided with the results [{this.GetType().GetTypeName()}] from the resolvers pre-processing logic; TotalCount is null.");

                int? totalCount = pagedResults.TotalCount;

                //Ensure we are null safe and return a valid empty list by default.
                IReadOnlyList<IndexEdge<TEntity>> selectedEdges = 
                    pagedResults?.ToEdgeResults().ToList() ?? new List<IndexEdge<TEntity>>(); ;

                IndexEdge<TEntity>? firstEdge = selectedEdges.FirstOrDefault();
                IndexEdge<TEntity>? lastEdge = selectedEdges.LastOrDefault();

                var connectionPageInfo = new ConnectionPageInfo(
                    hasNextPage: pagedResults?.HasNextPage ?? false,
                    hasPreviousPage: pagedResults?.HasPreviousPage ?? false,
                    startCursor: firstEdge?.Cursor,
                    endCursor: lastEdge?.Cursor,
                    totalCount: totalCount ?? 0
                );

                var graphQLConnection = new Connection<TEntity>(
                    selectedEdges,
                    connectionPageInfo,
                    ct => new ValueTask<int>(connectionPageInfo.TotalCount ?? 0)
                );

                return new ValueTask<Connection>(graphQLConnection);
            }

            throw new GraphQLException($"[{nameof(PreProcessedCursorPagingHandler<TEntity>)}] cannot handle the specified data source of type [{source.GetType().Name}].");
        }


    }
}
