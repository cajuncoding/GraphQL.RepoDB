using HotChocolate.Types.Pagination;
using System;
using System.Collections.Generic;

namespace HotChocolate.PreProcessingExtensions.Pagination
{

    /// <summary>
    /// Interface representing a Decorator class for a Page/Set/Slice of results any TEntity model provided.  
    /// This class represents a set of results/nodes of an edge/slice/page.
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    public interface ICursorPageSlice<TEntity> : IHavePreProcessedPagingInfo
    {
        /// <summary>
        /// The set of all CursorResults decorator classes for TEntity model with Cursor Index values.
        /// </summary>
        IEnumerable<ICursorResult<TEntity>> CursorResults { get; }
        /// <summary>
        /// An enumerable list of the base non-decorated TEntity values.
        /// </summary>
        IEnumerable<TEntity> Results { get; }

        /// <summary>
        /// Support safe (deferred) casting to the specified Entity Type.
        /// </summary>
        /// <typeparam name="TTargetType"></typeparam>
        /// <returns></returns>
        CursorPageSlice<TTargetType> OfType<TTargetType>();

        /// <summary>
        /// Convenience method to easily map/convert/project all types in the current page to a different object type
        /// altogether, without affecting the cursor indexes, etc. Provide deferred execution via Linq Select().
        /// </summary>
        /// <typeparam name="TTargetType"></typeparam>
        /// <param name="mappingFunc"></param>
        /// <returns></returns>
        CursorPageSlice<TTargetType> AsMappedType<TTargetType>(Func<TEntity, TTargetType> mappingFunc);

        /// <summary>
        /// Convenience method to Wrap the current Page Slice as PreProcessedCursorSliceResults; to eliminate
        /// ceremonial code for new-ing up the results.
        /// </summary>
        /// <returns></returns>
        [Obsolete("It is Strongly Recommended to now simply use ToGraphQLConnection() for returning data from Hot Chocolate Resolvers instead; " +
                  "since HC has resolved internal bug(s), a Connection result will offer improved performance. This will be removed in a future release to simplify the code," +
                  "and improve use of OOTB HC features (esp. when the new Pagination features are available in a later release of v13).")]
        PreProcessedCursorSlice<TEntity> AsPreProcessedCursorSlice();

        /// <summary>
        /// Convenience method to Convert the current Page Slice to a GraphQL Connection result to return from the Resolver;
        /// Connection results will not be processed as a Page since it's already paginated!
        /// </summary>
        /// <returns></returns>
        Connection<TEntity> ToGraphQLConnection();
    }
}