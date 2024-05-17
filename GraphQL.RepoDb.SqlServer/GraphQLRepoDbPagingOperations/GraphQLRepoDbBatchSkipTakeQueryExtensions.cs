using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using RepoDb;
using RepoDb.PagingPrimitives.OffsetPaging;
using RepoDb.SqlServer.PagingOperations;

namespace HotChocolate.RepoDb
{
    public static class GraphQLRepoDbBatchSkipTakeQueryExtensions
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
        [Obsolete("NOTE: This Method remains for backwards compatibility since the underlying RepoDb.SqlServer.PagingOperations package was released with renamed methods for better clarity.")]
        public static Task<IOffsetPageResults<TEntity>> GraphQLBatchSkipTakeQueryAsync<TEntity, TDbConnection>(
            this BaseRepository<TEntity, TDbConnection> baseRepo,
            IEnumerable<OrderField> orderBy,
            //NOTE: Expression is required to prevent Ambiguous Signatures
            Expression<Func<TEntity, bool>> whereExpression,
            IRepoDbOffsetPagingParams pagingParams = default,
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
            => baseRepo.PagingOffsetQueryAsync<TEntity, TDbConnection>(
                orderBy: orderBy,
                whereExpression: whereExpression,
                pagingParams: pagingParams,
                tableName: tableName,
                hints: hints,
                fields: fields,
                commandTimeout: commandTimeout,
                transaction: transaction,
                logTrace: logTrace,
                cancellationToken: cancellationToken
            );

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
        [Obsolete("NOTE: This Method remains for backwards compatibility since the underlying RepoDb.SqlServer.PagingOperations package was released with renamed methods for better clarity.")]
        public static Task<IOffsetPageResults<TEntity>> GraphQLBatchSkipTakeQueryAsync<TEntity, TDbConnection>(
            this BaseRepository<TEntity, TDbConnection> baseRepo,
            IEnumerable<OrderField> orderBy,
            QueryGroup whereQueryGroup = null,
            IRepoDbOffsetPagingParams pagingParams = default,
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
            => baseRepo.PagingOffsetQueryAsync<TEntity, TDbConnection>(
                orderBy: orderBy,
                whereQueryGroup: whereQueryGroup,
                pagingParams: pagingParams,
                tableName: tableName,
                hints: hints,
                fields: fields,
                commandTimeout: commandTimeout,
                transaction: transaction,
                logTrace: logTrace,
                cancellationToken: cancellationToken
            );

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
        [Obsolete("NOTE: This Method remains for backwards compatibility since the underlying RepoDb.SqlServer.PagingOperations package was released with renamed methods for better clarity.")]
        public static Task<IOffsetPageResults<TEntity>> GraphQLBatchSkipTakeQueryAsync<TEntity, TDbConnection>(
                this BaseRepository<TEntity, TDbConnection> baseRepo,
                IEnumerable<OrderField> orderBy,
                RawSqlWhere whereRawSql = null, //NOTE: This Overload allows cases where NO WHERE Filter is needed...
                IRepoDbOffsetPagingParams pagingParams = default,
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
            => baseRepo.PagingOffsetQueryAsync<TEntity, TDbConnection>(
                orderBy: orderBy,
                whereRawSql: whereRawSql,
                pagingParams: pagingParams,
                tableName: tableName,
                hints: hints,
                fields: fields,
                commandTimeout: commandTimeout,
                transaction: transaction,
                logTrace: logTrace,
                cancellationToken: cancellationToken
            );

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
        [Obsolete("NOTE: This Method remains for backwards compatibility since the underlying RepoDb.SqlServer.PagingOperations package was released with renamed methods for better clarity.")]
        public static async Task<IOffsetPageResults<TEntity>> GraphQLBatchSkipTakeQueryAsync<TEntity>(
            this DbConnection dbConnection,
            IEnumerable<OrderField> orderBy,
            Expression<Func<TEntity, bool>> whereExpression,
            IRepoDbOffsetPagingParams pagingParams = default,
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
            =>await dbConnection.PagingOffsetQueryAsync<TEntity>(
                orderBy: orderBy,
                whereExpression: whereExpression,
                pagingParams: pagingParams,
                tableName: tableName,
                hints: hints,
                fields: fields,
                commandTimeout: commandTimeout,
                transaction: transaction,
                logTrace: logTrace,
                cancellationToken: cancellationToken
            );

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
        [Obsolete("NOTE: This Method remains for backwards compatibility since the underlying RepoDb.SqlServer.PagingOperations package was released with renamed methods for better clarity.")]
        public static Task<IOffsetPageResults<TEntity>> GraphQLBatchSkipTakeQueryAsync<TEntity>(
            this DbConnection dbConnection,
            IEnumerable<OrderField> orderBy,
            QueryGroup whereQueryGroup,
            IRepoDbOffsetPagingParams pagingParams = default,
            string tableName = null,
            string hints = null,
            IEnumerable<Field> fields = null,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            Action<string> logTrace = null,
            CancellationToken cancellationToken = default
        ) where TEntity : class
            => dbConnection.PagingOffsetQueryAsync<TEntity>(
                orderBy: orderBy,
                whereQueryGroup: whereQueryGroup,
                pagingParams: pagingParams,
                tableName: tableName,
                hints: hints,
                fields: fields,
                commandTimeout: commandTimeout,
                transaction: transaction,
                logTrace: logTrace,
                cancellationToken: cancellationToken
            );

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
        [Obsolete("NOTE: This Method remains for backwards compatibility since the underlying RepoDb.SqlServer.PagingOperations package was released with renamed methods for better clarity.")]
        public static Task<IOffsetPageResults<TEntity>> GraphQLBatchSkipTakeQueryAsync<TEntity>(
            this DbConnection dbConnection,
            IEnumerable<OrderField> orderBy,
            RawSqlWhere whereRawSql = null, //NOTE: This Overload allows cases where NO WHERE Filter is needed...
            IRepoDbOffsetPagingParams pagingParams = default,
            string tableName = null,
            string hints = null,
            IEnumerable<Field> fields = null,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            Action<string> logTrace = null,
            CancellationToken cancellationToken = default
        ) where TEntity : class
            => dbConnection.PagingOffsetQueryAsync<TEntity>(
                    orderBy: orderBy,
                    whereRawSql: whereRawSql,
                    pagingParams: pagingParams,
                    tableName: tableName,
                    hints: hints,
                    fields: fields,
                    commandTimeout: commandTimeout,
                    transaction: transaction,
                    logTrace: logTrace,
                    cancellationToken: cancellationToken
                );
    }
}
