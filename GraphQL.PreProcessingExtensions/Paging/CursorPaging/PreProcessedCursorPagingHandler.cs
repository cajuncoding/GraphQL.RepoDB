# nullable enable

using HotChocolate.Resolvers;
using HotChocolate.Types.Pagination;
using System;
using System.Threading.Tasks;
using GraphQL.PreProcessingExtensions.Paging;
using HotChocolate.Utilities;

namespace HotChocolate.PreProcessingExtensions.Pagination
{
    [Obsolete("It is now Recommended to simply use ToGraphQLConnection() for directly returning a GraphQL Connection from Hot Chocolate Resolvers instead;"
              + " since HC has resolved internal bug(s), a Connection result will offer improved performance. This will likely be removed in future release"
              + " (especially once the new Paging features are available in a later version of v13.")]
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
                var includeTotalCountEnabled = this.PagingOptions.IncludeTotalCount ?? PagingDefaults.IncludeTotalCount;
                var graphQLParamsContext = new GraphQLParamsContext(context);

                //Optimized to only require TotalCount value if the query actually requested it!
                if (includeTotalCountEnabled && graphQLParamsContext.IsTotalCountRequested && pagedResults.TotalCount == null)
                    throw new InvalidOperationException($"Total Count is requested in the query, but was not provided with the results [{this.GetType().GetTypeName()}]"
                                                                + $" from the resolvers pre-processing logic; TotalCount is null.");

                var graphQLConnection = pagedResults.ToGraphQLConnection();
                return new ValueTask<Connection>(graphQLConnection);
            }

            throw new GraphQLException($"[{nameof(PreProcessedCursorPagingHandler<TEntity>)}] cannot handle the specified data source of type [{source.GetType().Name}].");
        }


    }
}
