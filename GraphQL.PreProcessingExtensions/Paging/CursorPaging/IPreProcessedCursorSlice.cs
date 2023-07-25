    using HotChocolate.Types.Pagination;
using System;
using System.Collections.Generic;

namespace HotChocolate.PreProcessingExtensions.Pagination
{
    [Obsolete("It is now Recommended to simply use ToGraphQLConnection() for directly returning a GraphQL Connection from Hot Chocolate Resolvers instead;"
              + " since HC has resolved internal bug(s), a Connection result will offer improved performance. This will likely be removed in future release"
              + " (especially once the new Paging features are available in a later version of v13.")]
    public interface IPreProcessedCursorSlice<TEntity> : IList<TEntity>, IHavePreProcessedPagingInfo, IAmPreProcessedResult
    {
        /// <summary>
        /// Convenience method to Convert the current Page Slice to a GraphQL Connection result to return from the Resolver;
        /// Connection results will not be processed as a Page since it's already paginated!
        /// </summary>
        /// <returns></returns>
        Connection<TEntity> ToGraphQLConnection();
    }
}
