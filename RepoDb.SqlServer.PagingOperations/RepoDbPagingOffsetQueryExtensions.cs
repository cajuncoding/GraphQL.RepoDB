using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using RepoDb.PagingPrimitives;
using RepoDb.PagingPrimitives.CursorPaging;
using RepoDb.PagingPrimitives.OffsetPaging;

namespace RepoDb.SqlServer.PagingOperations
{
    public static class RepoDbPagingOffsetQueryExtensions
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
        /// <param name="whereExpression"></param>
        /// <param name="pagingParams"></param>
        /// <param name="tableName"></param>
        /// <param name="hints"></param>
        /// <param name="fields"></param>
        /// <param name="commandTimeout"></param>
        /// <param name="transaction"></param>
        /// <param name="logTrace"></param>
        /// <param name="cancellationToken"></param>
        /// <returns>OffsetPageResults&lt;TEntity&gt;</returns>
        public static async Task<IOffsetPageResults<TEntity>> PagingOffsetQueryAsync<TEntity, TDbConnection>(
            this BaseRepository<TEntity, TDbConnection> baseRepo,
            IEnumerable<OrderField> orderBy,
            //NOTE: Expression is required to prevent Ambiguous Signatures
            Expression<Func<TEntity, bool>> whereExpression,
            IOffsetPagingParams pagingParams = default,
            string tableName = null,
            string hints = null,
            IEnumerable<Field> fields = null,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            Action<string> logTrace = null,
            CancellationToken cancellationToken = default
        )
        //ALL entities retrieved and Mapped for Cursor Pagination must support IHaveCursor interface.
        where TEntity : class
        where TDbConnection : DbConnection, new()
        {
            //Slice Querying is more flexible and works perfectly for Offset Based processing also so there is no
            //  need to maintain duplicated code for the less flexible paging approach since we can provide
            //  the simplified Offset Paging facade on top of the existing Slice Queries!
            var cursorPageResults = await baseRepo.PagingCursorQueryAsync(
                orderBy: orderBy,
                whereExpression: whereExpression,
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
            return cursorPageResults.ToOffsetPageResults();
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
        /// <param name="whereQueryGroup"></param>
        /// <param name="pagingParams"></param>
        /// <param name="tableName"></param>
        /// <param name="hints"></param>
        /// <param name="fields"></param>
        /// <param name="commandTimeout"></param>
        /// <param name="transaction"></param>
        /// <param name="logTrace"></param>
        /// <param name="cancellationToken"></param>
        /// <returns>OffsetPageResults&lt;TEntity&gt;</returns>
        public static async Task<IOffsetPageResults<TEntity>> PagingOffsetQueryAsync<TEntity, TDbConnection>(
            this BaseRepository<TEntity, TDbConnection> baseRepo,
            IEnumerable<OrderField> orderBy,
            QueryGroup whereQueryGroup = null,
            IOffsetPagingParams pagingParams = default,
            string tableName = null,
            string hints = null,
            IEnumerable<Field> fields = null,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            Action<string> logTrace = null,
            CancellationToken cancellationToken = default
        )
        //ALL entities retrieved and Mapped for Cursor Pagination must support IHaveCursor interface.
        where TEntity : class
        where TDbConnection : DbConnection, new()
        {
            //Slice Querying is more flexible and works perfectly for Offset Based processing also so there is no
            //  need to maintain duplicated code for the less flexible paging approach since we can provide
            //  the simplified Offset Paging facade on top of the existing Slice Queries!
            var cursorPageResults = await baseRepo.PagingCursorQueryAsync(
                orderBy: orderBy,
                whereQueryGroup: whereQueryGroup,
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
            return cursorPageResults.ToOffsetPageResults();
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
        /// <param name="whereRawSql"></param>
        /// <param name="pagingParams"></param>
        /// <param name="tableName"></param>
        /// <param name="hints"></param>
        /// <param name="fields"></param>
        /// <param name="commandTimeout"></param>
        /// <param name="transaction"></param>
        /// <param name="logTrace"></param>
        /// <param name="cancellationToken"></param>
        /// <returns>OffsetPageResults&lt;TEntity&gt;</returns>
        public static async Task<IOffsetPageResults<TEntity>> PagingOffsetQueryAsync<TEntity, TDbConnection>(
            this BaseRepository<TEntity, TDbConnection> baseRepo,
            IEnumerable<OrderField> orderBy,
            //NOTE: This Overload allows cases where NO WHERE Filter is needed...
            RawSqlWhere whereRawSql = null,
            IOffsetPagingParams pagingParams = default,
            string tableName = null,
            string hints = null,
            IEnumerable<Field> fields = null,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            Action<string> logTrace = null,
            CancellationToken cancellationToken = default
        )
        //ALL entities retrieved and Mapped for Cursor Pagination must support IHaveCursor interface.
        where TEntity : class
        where TDbConnection : DbConnection, new()
        {
            //Slice Querying is more flexible and works perfectly for Offset Based processing also so there is no
            //  need to maintain duplicated code for the less flexible paging approach since we can provide
            //  the simplified Offset Paging facade on top of the existing Slice Queries!
            var cursorPageResults = await baseRepo.PagingCursorQueryAsync(
                orderBy: orderBy,
                whereRawSql: whereRawSql,
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
            return cursorPageResults.ToOffsetPageResults();
        }

        /// <summary>
        /// Base DbConnection (SqlConnection) extension for Offset Paginated Batch Query capability.
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
        /// <param name="whereExpression"></param>
        /// <param name="pagingParams"></param>
        /// <param name="tableName"></param>
        /// <param name="hints"></param>
        /// <param name="fields"></param>
        /// <param name="commandTimeout"></param>
        /// <param name="transaction"></param>
        /// <param name="logTrace"></param>
        /// <param name="cancellationToken"></param>
        /// <returns>OffsetPageResults&lt;TEntity&gt;</returns>
        public static async Task<IOffsetPageResults<TEntity>> PagingOffsetQueryAsync<TEntity>(
            this DbConnection dbConnection,
            IEnumerable<OrderField> orderBy,
            Expression<Func<TEntity, bool>> whereExpression,
            IOffsetPagingParams pagingParams = default,
            string tableName = null,
            string hints = null,
            IEnumerable<Field> fields = null,
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
            var cursorPageResults = await dbConnection.PagingCursorQueryAsync(
                orderBy: orderBy,
                whereExpression: whereExpression,
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
            return cursorPageResults.ToOffsetPageResults(); ;
        }

        /// <summary>
        /// Base DbConnection (SqlConnection) extension for Offset Paginated Batch Query capability.
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
        /// <param name="whereQueryGroup"></param>
        /// <param name="pagingParams"></param>
        /// <param name="tableName"></param>
        /// <param name="hints"></param>
        /// <param name="fields"></param>
        /// <param name="commandTimeout"></param>
        /// <param name="transaction"></param>
        /// <param name="logTrace"></param>
        /// <param name="cancellationToken"></param>
        /// <returns>OffsetPageResults&lt;TEntity&gt;</returns>
        public static async Task<IOffsetPageResults<TEntity>> PagingOffsetQueryAsync<TEntity>(
            this DbConnection dbConnection,
            IEnumerable<OrderField> orderBy,
            QueryGroup whereQueryGroup,
            IOffsetPagingParams pagingParams = default,
            string tableName = null,
            string hints = null,
            IEnumerable<Field> fields = null,
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
            var cursorPageResults = await dbConnection.PagingCursorQueryAsync<TEntity>(
                orderBy: orderBy,
                whereQueryGroup: whereQueryGroup,
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
            return cursorPageResults.ToOffsetPageResults(); ;
        }

        /// <summary>
        /// Base DbConnection (SqlConnection) extension for Offset Paginated Batch Query capability.
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
        /// <param name="whereRawSql"></param>
        /// <param name="pagingParams"></param>
        /// <param name="tableName"></param>
        /// <param name="hints"></param>
        /// <param name="fields"></param>
        /// <param name="commandTimeout"></param>
        /// <param name="transaction"></param>
        /// <param name="logTrace"></param>
        /// <param name="cancellationToken"></param>
        /// <returns>OffsetPageResults&lt;TEntity&gt;</returns>
        public static async Task<IOffsetPageResults<TEntity>> PagingOffsetQueryAsync<TEntity>(
            this DbConnection dbConnection,
            IEnumerable<OrderField> orderBy,
            //NOTE: This Overload allows cases where NO WHERE Filter is needed...
            RawSqlWhere whereRawSql = null,
            IOffsetPagingParams pagingParams = default,
            string tableName = null,
            string hints = null,
            IEnumerable<Field> fields = null,
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
            var cursorPageResults = await dbConnection.PagingCursorQueryAsync<TEntity>(
                orderBy: orderBy,
                whereRawSql: whereRawSql,
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
            return cursorPageResults.ToOffsetPageResults(); ;
        }

        /// <summary>
        /// Base DbConnection (SqlConnection) extension for Offset Paginated Batch Query capability.
        /// 
        /// Public Facade method to provide dynamically paginated results using Offset based paging/slicing.
        /// 
        /// NOTE: Since RepoDb supports only Batch querying using Page Number and Page Size -- it's less flexible
        ///     than pure Offset based paging which uses Skip/Take. Therefore, this logic provides an extension
        ///     of RepoDb core functionality; and if this is ever provided by the Core functionality
        ///     this facade will remain as a proxy to the core feature.
        /// 
        /// NOTE: Cursor Slice Querying is more flexible and works perfectly for Offset Based processing also so this
        ///      represents a facade around the Cursor Page slicing that maps between Skip/Take and the Cursor paging paradigm.
        /// 
        /// NOTE: If the implementor needs further optimization then it's recommended to implement the optimized query
        ///      exactly as required; though this should work well for many common use cases.
        /// 
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="dbConnection">Extends the RepoDb BaseRepository abstraction</param>
        /// <param name="commandText">The raw SQL to be executed; must be a simple SELECT without any CTE or ORDER BY clause.</param>
        /// <param name="orderBy"></param>
        /// <param name="sqlParams"></param>
        /// <param name="take"></param>
        /// <param name="retrieveTotalCount"></param>
        /// <param name="commandTimeout"></param>
        /// <param name="transaction"></param>
        /// <param name="logTrace"></param>
        /// <param name="cancellationToken"></param>
        /// <param name="skip"></param>
        /// <returns>OffsetPageResults&lt;TEntity&gt;</returns>
        public static async Task<IOffsetPageResults<TEntity>> ExecutePagingOffsetQueryAsync<TEntity>(
            this DbConnection dbConnection,
            string commandText,
            IEnumerable<OrderField> orderBy,
            object sqlParams = null,
            int? skip = 0,
            int? take = 0,
            bool retrieveTotalCount = false,
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
            var cursorPageResults = await dbConnection.ExecutePagingCursorQueryAsync<TEntity>(
                commandText: commandText,
                orderBy: orderBy,
                sqlParams: sqlParams,
                pagingParams: ConvertOffsetParamsToCursorParams(OffsetPagingParams.ForSkipTake(skip, take, retrieveTotalCount)),
                commandTimeout: commandTimeout,
                transaction: transaction,
                logTrace: logTrace,
                cancellationToken: cancellationToken
            ).ConfigureAwait(false);

            //Map the Slice into the OffsetPageResults for simplified processing by calling code...
            return cursorPageResults.ToOffsetPageResults(); ;
        }

        /// <summary>
        /// Base DbConnection (SqlConnection) extension for Offset Paginated Batch Query capability.
        /// 
        /// Public Facade method to provide dynamically paginated results using Offset based paging/slicing.
        /// 
        /// NOTE: Since RepoDb supports only Batch querying using Page Number and Page Size -- it's less flexible
        ///     than pure Offset based paging which uses Skip/Take. Therefore, this logic provides an extension
        ///     of RepoDb core functionality; and if this is ever provided by the Core functionality
        ///     this facade will remain as a proxy to the core feature.
        /// 
        /// NOTE: Cursor Slice Querying is more flexible and works perfectly for Offset Based processing also so this
        ///      represents a facade around the Cursor Page slicing that maps between Skip/Take and the Cursor paging paradigm.
        /// 
        /// NOTE: If the implementor needs further optimization then it's recommended to implement the optimized query
        ///      exactly as required; though this should work well for many common use cases.
        /// 
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="dbConnection">Extends the RepoDb BaseRepository abstraction</param>
        /// <param name="commandText">The raw SQL to be executed; must be a simple SELECT without any CTE or ORDER BY clause.</param>
        /// <param name="orderBy"></param>
        /// <param name="sqlParams"></param>
        /// <param name="pagingParams"></param>
        /// <param name="commandTimeout"></param>
        /// <param name="transaction"></param>
        /// <param name="logTrace"></param>
        /// <param name="cancellationToken"></param>
        /// <returns>OffsetPageResults&lt;TEntity&gt;</returns>
        public static async Task<IOffsetPageResults<TEntity>> ExecutePagingOffsetQueryAsync<TEntity>(
            this DbConnection dbConnection,
            string commandText,
            IEnumerable<OrderField> orderBy,
            object sqlParams = null,
            IOffsetPagingParams pagingParams = default,
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
            var cursorPageResults = await dbConnection.ExecutePagingCursorQueryAsync<TEntity>(
                commandText: commandText,
                orderBy: orderBy,
                sqlParams: sqlParams,
                pagingParams: ConvertOffsetParamsToCursorParams(pagingParams),
                commandTimeout: commandTimeout,
                transaction: transaction,
                logTrace: logTrace,
                cancellationToken: cancellationToken
            ).ConfigureAwait(false);

            //Map the Slice into the OffsetPageResults for simplified processing by calling code...
            return cursorPageResults.ToOffsetPageResults(); ;
        }

        /// <summary>
        /// Helper method for converting Cursor Slice to OffsetPageResults for easier processing by calling code.
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="cursorPageResults"></param>
        /// <returns></returns>
        public static OffsetPageResults<TEntity> ToOffsetPageResults<TEntity>(this ICursorPageResults<TEntity> cursorPageResults) where TEntity : class
            => new OffsetPageResults<TEntity>(
                cursorPageResults.Results,
                cursorPageResults.HasNextPage,
                cursorPageResults.HasPreviousPage,
                CursorFactory.ParseCursor(cursorPageResults.StartCursor),
                CursorFactory.ParseCursor(cursorPageResults.EndCursor),
                cursorPageResults.TotalCount
            );

        private static ICursorPagingParams ConvertOffsetParamsToCursorParams(IOffsetPagingParams offsetParams)
            => CursorPagingParams.ForIndexes(
                first: offsetParams?.Take,
                afterIndex: offsetParams?.Skip,
                retrieveTotalCount: offsetParams?.IsTotalCountRequested ?? false
            );
    }

}
