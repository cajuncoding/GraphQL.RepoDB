﻿using HotChocolate.Types.Pagination;
using System;
using System.Collections.Generic;

namespace HotChocolate.ResolverProcessingExtensions.Pagination
{

    /// <summary>
    /// Interface representing a Decorator class for a Page/Set/Slice of results any TEntity model provided.  
    /// This class represents a set of results/nodes of an edge/slice/page.
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    public interface ICursorPageSlice<TEntity> : IHaveResolverProcessedPagingInfo
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
        /// Convenience method to convert the current cursor based page slice to a GraphQL Connection result to return from the Resolver;
        /// Connection results will not be post-processed since it's already paginated!
        /// </summary>
        /// <returns></returns>
        Connection<TEntity> ToGraphQLConnection();
    }
}