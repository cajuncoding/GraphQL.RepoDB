using HotChocolate.PreProcessingExtensions.Pagination;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using RepoDb.CursorPagination;

namespace RepoDb.OffsetPagination
{
    public static class RepoDbBatchSkipTakeQueryExtensions
    {
        /// <summary>
        /// Base Repository extension for Offset Paginated Batch Query capability.
        /// 
        /// Public Facade method to provide dynamically paginated results using Offset based paging/slicing.
        /// 
        /// NOTE: Since RepoDb supports only Batch querying using Page Number and Page Size -- it's less flexible
        ///     than pure Offset based paging which uses Skip/Take.  Therefore, this logic provides an extension
        ///     of RepoDb core functionality; and if this is ever provided by the Core functionality
        ///     this facade will remain as a proxy to the core feature.
        /// 
        /// NOTE: Cursor Slice Querying is more flexible and works perfectly for Offset Based processing also so this
        ///      represents a facade around the Cursor Page slicing that maps between Skip/Take and Cursor paging paradigm.
        /// 
        /// NOTE: If the implementor needs further optimization then it's recommended to implement the optimized query
        ///      exactly as required; this should work well for many common use cases.
        /// 
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <typeparam name="TDbConnection"></typeparam>
        /// <param name="baseRepo">Extends the RepoDb BaseRepository abstraction</param>
        /// <param name="orderBy"></param>
        /// <param name="where"></param>
        /// <param name="pagingParams"></param>
        /// <param name="tableName"></param>
        /// <param name="hints"></param>
        /// <param name="fields"></param>
        /// <param name="commandTimeout"></param>
        /// <param name="transaction"></param>
        /// <param name="logTrace"></param>
        /// <param name="cancellationToken"></param>
        /// <param name="computeTotalCount"></param>
        /// <returns>OffsetPageResults&lt;TEntity&gt;</returns>
        public static async Task<OffsetPageResults<TEntity>> GraphQLBatchSkipTakeQueryAsync<TEntity, TDbConnection>(
            this BaseRepository<TEntity, TDbConnection> baseRepo,
            IEnumerable<OrderField> orderBy,
            //NOTE: Expression is required to prevent Ambiguous Signatures
            Expression<Func<TEntity, bool>> where,
            IRepoDbOffsetPagingParams pagingParams = default,
            string tableName = null,
            string hints = null,
            IEnumerable<Field> fields = null,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            Action<string> logTrace = null,
            CancellationToken cancellationToken = default,
            bool computeTotalCount = false
        )
        //ALL entities retrieved and Mapped for Cursor Pagination must support IHaveCursor interface.
        where TEntity : class
        where TDbConnection : DbConnection
        {
            //Slice Querying is more flexible and works perfectly for Offset Based processing also so there is no
            //  need to maintain duplicated code for the less flexible paging approach since we can provide
            //  the simplified Offset Paging facade on top of the existing Slice Queries!
            var sliceResults = await baseRepo.GraphQLBatchSliceQueryAsync<TEntity, TDbConnection>(
                orderBy: orderBy,
                //NOTE: Expression is required to prevent Ambiguous Signatures
                where: where,
                pagingParams: ConvertOffsetParamsToCursorParams(pagingParams),
                tableName: tableName,
                hints: hints,
                fields: fields,
                commandTimeout: commandTimeout,
                transaction: transaction,
                logTrace: logTrace,
                cancellationToken: cancellationToken
            ).ConfigureAwait(false);

            //Map the Slice into the OffsetPageResults for simplified processing by calling code...
            return sliceResults.ToOffsetPageResults();
        }

        /// <summary>
        /// Base Repository extension for Offset Paginated Batch Query capability.
        /// 
        /// Public Facade method to provide dynamically paginated results using Offset based paging/slicing.
        /// 
        /// NOTE: Since RepoDb supports only Batch querying using Page Number and Page Size -- it's less flexible
        ///     than pure Offset based paging which uses Skip/Take.  Therefore, this logic provides an extension
        ///     of RepoDb core functionality; and if this is ever provided by the Core functionality
        ///     this facade will remain as a proxy to the core feature.
        /// 
        /// NOTE: Cursor Slice Querying is more flexible and works perfectly for Offset Based processing also so this
        ///      represents a facade around the Cursor Page slicing that maps between Skip/Take and Cursor paging paradigm.
        /// 
        /// NOTE: If the implementor needs further optimization then it's recommended to implement the optimized query
        ///      exactly as required; this should work well for many common use cases.
        /// 
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <typeparam name="TDbConnection"></typeparam>
        /// <param name="baseRepo">Extends the RepoDb BaseRepository abstraction</param>
        /// <param name="orderBy"></param>
        /// <param name="where"></param>
        /// <param name="tableName"></param>
        /// <param name="pagingParams"></param>
        /// <param name="hints"></param>
        /// <param name="fields"></param>
        /// <param name="commandTimeout"></param>
        /// <param name="transaction"></param>
        /// <param name="logTrace"></param>
        /// <param name="cancellationToken"></param>
        /// <returns>OffsetPageResults&lt;TEntity&gt;</returns>
        public static async Task<OffsetPageResults<TEntity>> GraphQLBatchSkipTakeQueryAsync<TEntity, TDbConnection>(
            this BaseRepository<TEntity, TDbConnection> baseRepo,
            IEnumerable<OrderField> orderBy,
            QueryGroup where = null,
            IRepoDbOffsetPagingParams pagingParams = default,
            string hints = null,
            IEnumerable<Field> fields = null,
            string tableName = null,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            Action<string> logTrace = null,
            CancellationToken cancellationToken = default
            )
        //ALL entities retrieved and Mapped for Cursor Pagination must support IHaveCursor interface.
        where TEntity : class
        where TDbConnection : DbConnection
        {
            //Slice Querying is more flexible and works perfectly for Offset Based processing also so there is no
            //  need to maintain duplicated code for the less flexible paging approach since we can provide
            //  the simplified Offset Paging facade on top of the existing Slice Queries!
            var sliceResults = await baseRepo.GraphQLBatchSliceQueryAsync<TEntity, TDbConnection>(
                orderBy: orderBy,
                //NOTE: Expression is required to prevent Ambiguous Signatures
                where: where,
                pagingParams: ConvertOffsetParamsToCursorParams(pagingParams),
                tableName: tableName,
                hints: hints,
                fields: fields,
                commandTimeout: commandTimeout,
                transaction: transaction,
                logTrace: logTrace,
                cancellationToken: cancellationToken
            ).ConfigureAwait(false);

            //Map the Slice into the OffsetPageResults for simplified processing by calling code...
            return sliceResults.ToOffsetPageResults();
        }

        /// <summary>
        /// Base Repository extension for Offset Paginated Batch Query capability.
        /// 
        /// Public Facade method to provide dynamically paginated results using Offset based paging/slicing.
        /// 
        /// NOTE: Since RepoDb supports only Batch querying using Page Number and Page Size -- it's less flexible
        ///     than pure Offset based paging which uses Skip/Take.  Therefore, this logic provides an extension
        ///     of RepoDb core functionality; and if this is ever provided by the Core functionality
        ///     this facade will remain as a proxy to the core feature.
        /// 
        /// NOTE: Cursor Slice Querying is more flexible and works perfectly for Offset Based processing also so this
        ///      represents a facade around the Cursor Page slicing that maps between Skip/Take and Cursor paging paradigm.
        /// 
        /// NOTE: If the implementor needs further optimization then it's recommended to implement the optimized query
        ///      exactly as required; this should work well for many common use cases.
        /// 
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="dbConnection">Extends the RepoDb BaseRepository abstraction</param>
        /// <param name="orderBy"></param>
        /// <param name="where"></param>
        /// <param name="pagingParams"></param>
        /// <param name="hints"></param>
        /// <param name="fields"></param>
        /// <param name="commandTimeout"></param>
        /// <param name="transaction"></param>
        /// <param name="logTrace"></param>
        /// <param name="cancellationToken"></param>
        /// <returns>OffsetPageResults&lt;TEntity&gt;</returns>
        public static async Task<OffsetPageResults<TEntity>> GraphQLBatchSkipTakeQueryAsync<TEntity>(
            this DbConnection dbConnection,
            IEnumerable<OrderField> orderBy,
            //NOTE: Expression is required to prevent Ambiguous Signatures
            Expression<Func<TEntity, bool>> where,
            IRepoDbOffsetPagingParams pagingParams = default,
            string hints = null,
            IEnumerable<Field> fields = null,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            Action<string> logTrace = null,
            CancellationToken cancellationToken = default,
            bool computeTotalCount = false
            )
        //ALL entities retrieved and Mapped for Cursor Pagination must support IHaveCursor interface.
        where TEntity : class
        {
            //Slice Querying is more flexible and works perfectly for Offset Based processing also so there is no
            //  need to maintain duplicated code for the less flexible paging approach since we can provide
            //  the simplified Offset Paging facade on top of the existing Slice Queries!
            var sliceResults = await dbConnection.GraphQLBatchSliceQueryAsync<TEntity>(
                orderBy: orderBy,
                //NOTE: Expression is required to prevent Ambiguous Signatures
                where: where,
                pagingParams: ConvertOffsetParamsToCursorParams(pagingParams),
                hints: hints,
                fields: fields,
                commandTimeout: commandTimeout,
                transaction: transaction,
                logTrace: logTrace,
                cancellationToken: cancellationToken
            ).ConfigureAwait(false); 

            //Map the Slice into the OffsetPageResults for simplified processing by calling code...
            return sliceResults.ToOffsetPageResults(); ;
        }

        /// <summary>
        /// Base Repository extension for Offset Paginated Batch Query capability.
        /// 
        /// Public Facade method to provide dynamically paginated results using Offset based paging/slicing.
        /// 
        /// NOTE: Since RepoDb supports only Batch querying using Page Number and Page Size -- it's less flexible
        ///     than pure Offset based paging which uses Skip/Take.  Therefore, this logic provides an extension
        ///     of RepoDb core functionality; and if this is ever provided by the Core functionality
        ///     this facade will remain as a proxy to the core feature.
        /// 
        /// NOTE: Cursor Slice Querying is more flexible and works perfectly for Offset Based processing also so this
        ///      represents a facade around the Cursor Page slicing that maps between Skip/Take and Cursor paging paradigm.
        /// 
        /// NOTE: If the implementor needs further optimization then it's recommended to implement the optimized query
        ///      exactly as required; this should work well for many common use cases.
        /// 
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="dbConnection">Extends the RepoDb BaseRepository abstraction</param>
        /// <param name="orderBy"></param>
        /// <param name="where"></param>
        /// <param name="pagingParams"></param>
        /// <param name="hints"></param>
        /// <param name="fields"></param>
        /// <param name="tableName"></param>
        /// <param name="commandTimeout"></param>
        /// <param name="transaction"></param>
        /// <param name="logTrace"></param>
        /// <param name="cancellationToken"></param>
        /// <returns>OffsetPageResults&lt;TEntity&gt;</returns>
        public static async Task<OffsetPageResults<TEntity>> GraphQLBatchSkipTakeQueryAsync<TEntity>(
            this DbConnection dbConnection,
            IEnumerable<OrderField> orderBy,
            QueryGroup where = null,
            IRepoDbOffsetPagingParams pagingParams = default,
            string hints = null,
            IEnumerable<Field> fields = null,
            string tableName = null,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            Action<string> logTrace = null,
            CancellationToken cancellationToken = default
        )
        //ALL entities retrieved and Mapped for Cursor Pagination must support IHaveCursor interface.
        where TEntity : class
        {
            //Slice Querying is more flexible and works perfectly for Offset Based processing also so there is no
            //  need to maintain duplicated code for the less flexible paging approach since we can provide
            //  the simplified Offset Paging facade on top of the existing Slice Queries!
            var sliceResults = await dbConnection.GraphQLBatchSliceQueryAsync<TEntity>(
                orderBy: orderBy,
                //NOTE: Expression is required to prevent Ambiguous Signatures
                where: where,
                pagingParams: ConvertOffsetParamsToCursorParams(pagingParams),
                hints: hints,
                fields: fields,
                tableName: tableName,
                commandTimeout: commandTimeout,
                transaction: transaction,
                logTrace: logTrace,
                cancellationToken: cancellationToken
            ).ConfigureAwait(false);

            //Map the Slice into the OffsetPageResults for simplified processing by calling code...
            return sliceResults.ToOffsetPageResults(); ;
        }

        /// <summary>
        /// Helper method for converting Cursor Slice to OffsetPageResults for easier processing by calling code.
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="cursorPageSlice"></param>
        /// <returns></returns>
        public static OffsetPageResults<TEntity> ToOffsetPageResults<TEntity>(this CursorPageSlice<TEntity> cursorPageSlice)
            //ALL entities retrieved and Mapped for Cursor Pagination must support IHaveCursor interface.
            where TEntity : class
        {
            return new OffsetPageResults<TEntity>(
                cursorPageSlice.Results,
                cursorPageSlice.HasNextPage,
                cursorPageSlice.HasPreviousPage,
                cursorPageSlice.TotalCount
            );
        }

        private static IRepoDbCursorPagingParams ConvertOffsetParamsToCursorParams(IRepoDbOffsetPagingParams offsetParams)
        {
            return new RepoDbCursorPagingParams(
                after: offsetParams?.Skip,
                first: offsetParams?.Take,
                isTotalCountRequested: offsetParams?.IsTotalCountRequested ?? false
            );
        }
    }

}
