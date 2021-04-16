# nullable enable

using HotChocolate.Resolvers;
using HotChocolate.Types.Pagination;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HotChocolate.Utilities;

namespace HotChocolate.PreProcessingExtensions.Pagination
{
    public class PreProcessedCursorPagingHandler<TEntity> : CursorPagingHandler
    {
        public static CursorPagingArguments NoOpCursorPagingArguments = new CursorPagingArguments();

        public PagingOptions PagingOptions { get; protected set; }

        public PreProcessedCursorPagingHandler(PagingOptions pagingOptions)
            : base(pagingOptions)
        {
            this.PagingOptions = pagingOptions;
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
        #pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
        protected override async ValueTask<Connection> SliceAsync(IResolverContext context, object source, CursorPagingArguments arguments)
        #pragma warning restore CS1998
        {

            IPreProcessedCursorSlice<TEntity>? pagedSlice = null;

            //If Appropriate we handle the values here to ensure that no post-processing is done other than
            //  correctly mapping the results into a GraphQL Connection as Edges with Cursors...
            if (source is IPreProcessedCursorSlice<TEntity> cursorPageSlice)
            {
                bool includeTotalCountEnabled = this.PagingOptions.IncludeTotalCount ?? PagingDefaults.IncludeTotalCount;
                var graphQLParamsContext = new GraphQLParamsContext(context);

                //Optimized to only require TotalCount value if the query actually requested it!
                if (includeTotalCountEnabled && graphQLParamsContext.IsTotalCountRequested && cursorPageSlice.TotalCount == null)
                    throw new InvalidOperationException($"Total Count is requested in the query, but was not provided with the results [{this.GetType().GetTypeName()}] from the resolvers pre-processing logic; TotalCount is null.");

                pagedSlice = cursorPageSlice;
            }
            //IF provided (and enabled in the Paging Provider via CanHandle() checks), we attempt to gracefully handle IEnumerable even though TotalCount details are likely incorrect.
            else if (source is IEnumerable<TEntity> enumerableResults)
            {
                int index = 0;
                var cursorItems = enumerableResults.Select(c => new CursorResult<TEntity>(c, ++index));

                var totalCount = source is IHavePreProcessedPagingInfo pagingInfo
                    ? pagingInfo.TotalCount ?? 0
                    : 0;

                pagedSlice = new CursorPageSlice<TEntity>(cursorItems, totalCount).AsPreProcessedCursorSlice();
            }

            if (pagedSlice != null)
            {
                int? totalCount = pagedSlice.TotalCount;

                //Ensure we are null safe and return a valid empty list by default.
                IReadOnlyList<IndexEdge<TEntity>> selectedEdges =
                    pagedSlice?.ToEdgeResults().ToList() ?? new List<IndexEdge<TEntity>>();
                ;

                IndexEdge<TEntity>? firstEdge = selectedEdges.FirstOrDefault();
                IndexEdge<TEntity>? lastEdge = selectedEdges.LastOrDefault();

                var connectionPageInfo = new ConnectionPageInfo(
                    hasNextPage: pagedSlice?.HasNextPage ?? false,
                    hasPreviousPage: pagedSlice?.HasPreviousPage ?? false,
                    startCursor: firstEdge?.Cursor,
                    endCursor: lastEdge?.Cursor,
                    totalCount: totalCount ?? 0
                );

                var graphQLConnection = new Connection<TEntity>(
                    selectedEdges,
                    connectionPageInfo,
                    ct => new ValueTask<int>(connectionPageInfo.TotalCount ?? 0)
                );

                return graphQLConnection;
            }

            throw new GraphQLException($"[{nameof(PreProcessedCursorPagingHandler<TEntity>)}] cannot handle the specified data source of type [{source.GetType().Name}].");
        }


    }
}
