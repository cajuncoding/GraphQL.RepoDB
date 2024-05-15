using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using RepoDb;
using RepoDb.CursorPaging;
using RepoDb.SqlServer.PagingOperations;
using RepoDb.SqlServer.PagingOperations.CommonPrimitives;

namespace HotChocolate.RepoDb
{
    public static class GraphQLRepoDbBatchSliceQueryExtensions
    {
        /// <summary>
        /// Base Repository extension for Relay Cursor Paginated Batch Query capability.
        /// 
        /// Public Facade method to provide dynamically paginated results using Relay Cursor slicing.
        /// Relay spec cursor algorithm is implemented for Sql Server on top of RepoDb.
        /// 
        /// NOTE: Since RepoDb supports only Offset Batch querying, this logic provided as an extension
        ///     of RepoDb core functionality; and if this is ever provided by the Core functionality
        ///     this facade will remain as a proxy to core feature.
        ///     
        /// NOTE: For Relay Spec details and Cursor Algorithm see:
        ///     https://relay.dev/graphql/connections.htm#sec-Pagination-algorithm
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
        /// <returns>CursorPageSlice&lt;TEntity&gt;</returns>
        [Obsolete("NOTE: This Method remains for backwards compatibility since the underlying RepoDb.SqlServer.PagingOperations package was released with renamed methods for better clarity.")]
        public static Task<CursorPageResults<TEntity>> GraphQLBatchSliceQueryAsync<TEntity, TDbConnection>(
            this BaseRepository<TEntity, TDbConnection> baseRepo,
            IEnumerable<OrderField> orderBy,
            Expression<Func<TEntity, bool>> whereExpression,
            IRepoDbCursorPagingParams pagingParams = default,
            string tableName = null,
            string hints = null,
            IEnumerable<Field> fields = null,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            Action<string> logTrace = null,
            CancellationToken cancellationToken = default
        )
        where TEntity : class
        where TDbConnection : DbConnection, new()
            => baseRepo.PagingCursorQueryAsync<TEntity, TDbConnection>(
                orderBy: orderBy,
                whereQueryGroup: whereExpression != null ? QueryGroup.Parse<TEntity>(whereExpression) : (QueryGroup)null,
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
        /// Base Repository extension for Relay Cursor Paginated Batch Query capability.
        /// 
        /// Public Facade method to provide dynamically paginated results using Relay Cursor slicing.
        /// Relay spec cursor algorithm is implemented for Sql Server on top of RepoDb.
        /// 
        /// NOTE: Since RepoDb supports only Offset Batch querying, this logic provided as an extension
        ///     of RepoDb core functionality; and if this is ever provided by the Core functionality
        ///     this facade will remain as a proxy to core feature.
        ///     
        /// NOTE: For Relay Spec details and Cursor Algorithm see:
        ///     https://relay.dev/graphql/connections.htm#sec-Pagination-algorithm
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
        /// <returns>CursorPageSlice&lt;TEntity&gt;</returns>
        [Obsolete("NOTE: This Method remains for backwards compatibility since the underlying RepoDb.SqlServer.PagingOperations package was released with renamed methods for better clarity.")]
        public static Task<CursorPageResults<TEntity>> GraphQLBatchSliceQueryAsync<TEntity, TDbConnection>(
            this BaseRepository<TEntity, TDbConnection> baseRepo,
            IEnumerable<OrderField> orderBy,
            QueryGroup whereQueryGroup,
            IRepoDbCursorPagingParams pagingParams = default,
            string tableName = null,
            string hints = null,
            IEnumerable<Field> fields = null,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            Action<string> logTrace = null,
            CancellationToken cancellationToken = default
        ) 
        where TEntity : class 
        where TDbConnection : DbConnection, new()
            => baseRepo.PagingCursorQueryAsync<TEntity, TDbConnection>(
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
        /// Base Repository extension for Relay Cursor Paginated Batch Query capability.
        /// 
        /// Public Facade method to provide dynamically paginated results using Relay Cursor slicing.
        /// Relay spec cursor algorithm is implemented for Sql Server on top of RepoDb.
        /// 
        /// NOTE: Since RepoDb supports only Offset Batch querying, this logic provided as an extension
        ///     of RepoDb core functionality; and if this is ever provided by the Core functionality
        ///     this facade will remain as a proxy to core feature.
        ///     
        /// NOTE: For Relay Spec details and Cursor Algorithm see:
        ///     https://relay.dev/graphql/connections.htm#sec-Pagination-algorithm
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
        /// <returns>CursorPageSlice&lt;TEntity&gt;</returns>
        [Obsolete("NOTE: This Method remains for backwards compatibility since the underlying RepoDb.SqlServer.PagingOperations package was released with renamed methods for better clarity.")]
        public static Task<CursorPageResults<TEntity>> GraphQLBatchSliceQueryAsync<TEntity, TDbConnection>(
            this BaseRepository<TEntity, TDbConnection> baseRepo,
            IEnumerable<OrderField> orderBy,
            RawSqlWhere whereRawSql = null, //NOTE: This Overload allows cases where NO WHERE Filter is needed...
            IRepoDbCursorPagingParams pagingParams = default,
            string tableName = null,
            string hints = null,
            IEnumerable<Field> fields = null,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            Action<string> logTrace = null,
            CancellationToken cancellationToken = default
        )
        where TEntity : class
        where TDbConnection : DbConnection, new()
            => baseRepo.PagingCursorQueryAsync<TEntity, TDbConnection>(
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
        /// Base DbConnection (SqlConnection) extension for Relay Cursor Paginated Batch Query capability.
        /// 
        /// Public Facade method to provide dynamically paginated results using Relay Cursor slicing.
        /// Relay spec cursor algorithm is implemented for Sql Server on top of RepoDb.
        /// 
        /// NOTE: Since RepoDb supports only Offset Batch querying, this logic provided as an extension
        ///     of RepoDb core functionality; and if this is ever provided by the Core functionality
        ///     this facade will remain as a proxy to core feature.
        ///     
        /// NOTE: For Relay Spec details and Cursor Algorithm see:
        ///     https://relay.dev/graphql/connections.htm#sec-Pagination-algorithm
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="dbConnection">Extends DbConnection directly</param>
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
        /// <returns>CursorPageSlice&lt;TEntity&gt;</returns>
        [Obsolete("NOTE: This Method remains for backwards compatibility since the underlying RepoDb.SqlServer.PagingOperations package was released with renamed methods for better clarity.")]
        public static Task<CursorPageResults<TEntity>> GraphQLBatchSliceQueryAsync<TEntity>(
            this DbConnection dbConnection,
            IEnumerable<OrderField> orderBy,
            //NOTE: Expression is required to prevent Ambiguous Signatures
            Expression<Func<TEntity, bool>> whereExpression,
            IRepoDbCursorPagingParams pagingParams = default,
            string tableName = null,
            string hints = null,
            IEnumerable<Field> fields = null,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            Action<string> logTrace = null,
            CancellationToken cancellationToken = default
        ) where TEntity : class
            => dbConnection.PagingCursorQueryAsync<TEntity>(
                orderBy: orderBy,
                whereQueryGroup: whereExpression != null ? QueryGroup.Parse<TEntity>(whereExpression) : (QueryGroup)null,
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
        /// Base DbConnection (SqlConnection) extension for Relay Cursor Paginated Batch Query capability.
        /// 
        /// Public Facade method to provide dynamically paginated results using Relay Cursor slicing.
        /// Relay spec cursor algorithm is implemented for Sql Server on top of RepoDb.
        /// 
        /// NOTE: Since RepoDb supports only Offset Batch querying, this logic provided as an extension
        ///     of RepoDb core functionality; and if this is ever provided by the Core functionality
        ///     this facade will remain as a proxy to core feature.
        ///     
        /// NOTE: For Relay Spec details and Cursor Algorithm see:
        ///     https://relay.dev/graphql/connections.htm#sec-Pagination-algorithm
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="dbConnection">Extends DbConnection directly</param>
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
        /// <returns>CursorPageSlice&lt;TEntity&gt;</returns>
        [Obsolete("NOTE: This Method remains for backwards compatibility since the underlying RepoDb.SqlServer.PagingOperations package was released with renamed methods for better clarity.")]
        public static Task<CursorPageResults<TEntity>> GraphQLBatchSliceQueryAsync<TEntity>(
            this DbConnection dbConnection,
            IEnumerable<OrderField> orderBy,
            //NOTE: RawSql Where is required to prevent Ambiguous Signatures
            QueryGroup whereQueryGroup,
            IRepoDbCursorPagingParams pagingParams = default,
            string hints = null,
            IEnumerable<Field> fields = null,
            string tableName = null,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            Action<string> logTrace = null,
            CancellationToken cancellationToken = default
        ) where TEntity : class
            => dbConnection.PagingCursorQueryAsync<TEntity>(
                orderBy: orderBy,
                //NOTE: Must cast to raw object to prevent Recursive execution with our catch-all overload...
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
        /// Base DbConnection (SqlConnection) extension for Relay Cursor Paginated Batch Query capability.
        /// 
        /// Public Facade method to provide dynamically paginated results using Relay Cursor slicing.
        /// Relay spec cursor algorithm is implemented for Sql Server on top of RepoDb.
        /// 
        /// NOTE: Since RepoDb supports only Offset Batch querying, this logic provided as an extension
        ///     of RepoDb core functionality; and if this is ever provided by the Core functionality
        ///     this facade will remain as a proxy to core feature.
        ///     
        /// NOTE: For Relay Spec details and Cursor Algorithm see:
        ///     https://relay.dev/graphql/connections.htm#sec-Pagination-algorithm
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="dbConnection">Extends DbConnection directly</param>
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
        /// <returns>CursorPageSlice&lt;TEntity&gt;</returns>
        [Obsolete("NOTE: This Method remains for backwards compatibility since the underlying RepoDb.SqlServer.PagingOperations package was released with renamed methods for better clarity.")]
        public static Task<CursorPageResults<TEntity>> GraphQLBatchSliceQueryAsync<TEntity>(
            this DbConnection dbConnection,
            IEnumerable<OrderField> orderBy,
            RawSqlWhere whereRawSql = null, //NOTE: This Overload allows cases where NO WHERE Filter is needed...
            IRepoDbCursorPagingParams pagingParams = default,
            string tableName = null,
            string hints = null,
            IEnumerable<Field> fields = null,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            Action<string> logTrace = null,
            CancellationToken cancellationToken = default
        ) where TEntity : class
            => dbConnection.PagingCursorQueryAsync<TEntity>(
                orderBy: orderBy,
                //NOTE: Must cast to raw object to prevent Recursive execution with our catch-all overload...
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
