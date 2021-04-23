//using HotChocolate.PreProcessingExtensions.Pagination;
//using HotChocolate.RepoDb.SqlServer.Reflection;
//using RepoDb.Enumerations;
//using System;
//using System.Collections.Generic;
//using System.Data;
//using System.Data.Common;
//using System.Linq;
//using System.Linq.Expressions;
//using System.Threading;
//using System.Threading.Tasks;
//using RepoDb.Interfaces;

//namespace RepoDb.OffsetPagination
//{
//    public static class RepoDbBatchQueryExtensions
//    {
//        /// <summary>
//        /// Base Repository extension for Offset Paginated Batch Query capability.
//        /// </summary>
//        /// <typeparam name="TEntity"></typeparam>
//        /// <typeparam name="TDbConnection"></typeparam>
//        /// <param name="baseRepo"></param>
//        /// <param name="orderBy"></param>
//        /// <param name="where"></param>
//        /// <param name="page"></param>
//        /// <param name="rowsPerBatch"></param>
//        /// <param name="fetchTotalCount"></param>
//        /// <param name="tableName"></param>
//        /// <param name="hints"></param>
//        /// <param name="fields"></param>
//        /// <param name="commandTimeout"></param>
//        /// <param name="transaction"></param>
//        /// <param name="trace"></param>
//        /// <param name="cancellationToken"></param>
//        /// <returns></returns>
//        public static async Task<OffsetPageResults<TEntity>> GraphQLBatchOffsetPagingQueryAsync<TEntity, TDbConnection>(
//            this BaseRepository<TEntity, TDbConnection> baseRepo,
//            IEnumerable<OrderField> orderBy,
//            //NOTE: Expression is required to prevent Ambiguous Signatures
//            Expression<Func<TEntity, bool>> where,
//            int? page = null,
//            int? rowsPerBatch = null,
//            bool fetchTotalCount = false,
//            string tableName = null,
//            string hints = null,
//            IEnumerable<Field> fields = null,
//            int? commandTimeout = null,
//            IDbTransaction transaction = null,
//            ITrace trace = null,
//            CancellationToken cancellationToken = default
//        )
//        //ALL entities retrieved and Mapped for Cursor Pagination must support IHaveCursor interface.
//        where TEntity : class
//        where TDbConnection : DbConnection
//        {
//            return await baseRepo.GraphQLBatchOffsetPagingQueryAsync<TEntity, TDbConnection>(
//                    page: page,
//                    rowsPerBatch: rowsPerBatch,
//                    fetchTotalCount: fetchTotalCount,
//                    orderBy: orderBy,
//                    where: where != null ? QueryGroup.Parse<TEntity>(where) : (QueryGroup)null,
//                    hints: hints,
//                    fields: fields,
//                    tableName: tableName,
//                    commandTimeout: commandTimeout,
//                    transaction: transaction,
//                    trace: trace,
//                    cancellationToken: cancellationToken
//                ).ConfigureAwait(false);
//        }

//        /// <summary>
//        /// Base Repository extension for Offset Paginated Batch Query capability.
//        /// </summary>
//        /// <typeparam name="TEntity"></typeparam>
//        /// <typeparam name="TDbConnection"></typeparam>
//        /// <param name="baseRepo"></param>
//        /// <param name="orderBy"></param>
//        /// <param name="where"></param>
//        /// <param name="page"></param>
//        /// <param name="rowsPerBatch"></param>
//        /// <param name="tableName"></param>
//        /// <param name="fetchTotalCount"></param>
//        /// <param name="hints"></param>
//        /// <param name="fields"></param>
//        /// <param name="commandTimeout"></param>
//        /// <param name="transaction"></param>
//        /// <param name="trace"></param>
//        /// <param name="cancellationToken"></param>
//        /// <returns></returns>
//        public static async Task<OffsetPageResults<TEntity>> GraphQLBatchOffsetPagingQueryAsync<TEntity, TDbConnection>(
//            this BaseRepository<TEntity, TDbConnection> baseRepo,
//            IEnumerable<OrderField> orderBy,
//            QueryGroup where = null,
//            int? page = null,
//            int? rowsPerBatch = null,
//            bool fetchTotalCount = false,
//            string hints = null,
//            IEnumerable<Field> fields = null,
//            string tableName = null,
//            int? commandTimeout = null,
//            IDbTransaction transaction = null,
//            ITrace trace = null,
//            CancellationToken cancellationToken = default
//        )
//        //ALL entities retrieved and Mapped for Cursor Pagination must support IHaveCursor interface.
//        where TEntity : class
//        where TDbConnection : DbConnection
//        {
//            //Below Logic mirrors that of RepoDb Source for managing the Connection (PerInstance or PerCall)!
//            var connection = (DbConnection)(transaction?.Connection ?? baseRepo.CreateConnection());

//            try
//            {
//                var cursorPageResult = await connection.GraphQLBatchOffsetPagingQueryAsync<TEntity>(
//                    page: page,
//                    rowsPerBatch: rowsPerBatch,
//                    fetchTotalCount: fetchTotalCount,
//                    orderBy: orderBy,
//                    where: where,
//                    hints: hints,
//                    fields: fields,
//                    tableName: tableName,
//                    commandTimeout: commandTimeout,
//                    transaction: transaction,
//                    trace: trace,
//                    cancellationToken: cancellationToken
//                ).ConfigureAwait(false);

//                return cursorPageResult;
//            }
//            catch
//            {
//                // Throw back the error
//                throw;
//            }
//            finally
//            {
//                // Dispose the connection
//                baseRepo.DisposeConnectionForPerCallExtension(connection, transaction);
//            }
//        }

//        /// <summary>
//        /// Base DbConnection (SqlConnection) extension for Offset Batch Paginated Batch Query capability.
//        /// </summary>
//        /// <typeparam name="TEntity"></typeparam>
//        /// <param name="dbConnection">Extends DbConnection directly</param>
//        /// <param name="orderBy"></param>
//        /// <param name="where"></param>
//        /// <param name="page"></param>
//        /// <param name="rowsPerBatch"></param>
//        /// <param name="fetchTotalCount"></param>
//        /// <param name="hints"></param>
//        /// <param name="fields"></param>
//        /// <param name="commandTimeout"></param>
//        /// <param name="transaction"></param>
//        /// <param name="trace"></param>
//        /// <param name="cancellationToken"></param>
//        /// <returns></returns>
//        public static async Task<OffsetPageResults<TEntity>> GraphQLBatchOffsetPagingQueryAsync<TEntity>(
//            this DbConnection dbConnection,
//            IEnumerable<OrderField> orderBy,
//            //NOTE: Expression is required to prevent Ambiguous Signatures
//            Expression<Func<TEntity, bool>> where,
//            int? page = null,
//            int? rowsPerBatch = null,
//            bool fetchTotalCount = false,
//            string hints = null,
//            IEnumerable<Field> fields = null,
//            int? commandTimeout = null,
//            IDbTransaction transaction = null,
//            ITrace trace = null,
//            CancellationToken cancellationToken = default
//        )
//        //ALL entities retrieved and Mapped for Cursor Pagination must support IHaveCursor interface.
//        where TEntity : class
//        {
//            return await dbConnection.GraphQLBatchOffsetPagingQueryAsync<TEntity>(
//                page: page,
//                rowsPerBatch: rowsPerBatch,
//                fetchTotalCount: fetchTotalCount,
//                orderBy: orderBy,
//                where: where != null ? QueryGroup.Parse<TEntity>(where) : (QueryGroup)null,
//                hints: hints,
//                fields: fields,
//                commandTimeout: commandTimeout,
//                transaction: transaction,
//                trace: trace,
//                cancellationToken: cancellationToken
//            ).ConfigureAwait(false);
//        }

//        /// <summary>
//        /// Base Repository extension for Offset Paginated Batch Query capability.
//        /// </summary>
//        /// <typeparam name="TEntity"></typeparam>
//        /// <param name="dbConnection">Extends DbConnection directly</param>
//        /// <param name="orderBy"></param>
//        /// <param name="where"></param>
//        /// <param name="page"></param>
//        /// <param name="rowsPerBatch"></param>
//        /// <param name="tableName"></param>
//        /// <param name="fetchTotalCount"></param>
//        /// <param name="hints"></param>
//        /// <param name="fields"></param>
//        /// <param name="commandTimeout"></param>
//        /// <param name="transaction"></param>
//        /// <param name="trace"></param>
//        /// <param name="cancellationToken"></param>
//        /// <returns></returns>
//        public static async Task<OffsetPageResults<TEntity>> GraphQLBatchOffsetPagingQueryAsync<TEntity>(
//            this DbConnection dbConnection,
//            IEnumerable<OrderField> orderBy,
//            QueryGroup where = null, 
//            int? page = null, 
//            int? rowsPerBatch = null,
//            bool fetchTotalCount = false,
//            string hints = null,
//            IEnumerable<Field> fields = null,
//            string tableName = null,
//            int? commandTimeout = null,
//            IDbTransaction transaction = null,
//            ITrace trace = null,
//            CancellationToken cancellationToken = default
//        )
//        //ALL entities retrieved and Mapped for Cursor Pagination must support IHaveCursor interface.
//        where TEntity : class
//        {
//            if (orderBy == null)
//                throw new ArgumentNullException(nameof(orderBy), "A sort order must be specified to provide valid paging results.");

//            var dbTableName = string.IsNullOrWhiteSpace(tableName)
//                                ? ClassMappedNameCache.Get<TEntity>()
//                                : tableName;

//            var fieldsList = fields.ToList();

//            //Ensure we have default fields; default is to include All Fields...
//            var selectFields = fieldsList?.Any() == true
//                ? fieldsList
//                : FieldCache.Get<TEntity>();

//            //Retrieve only the select fields that are valid for the Database query!
//            //NOTE: We guard against duplicate values as a convenience.
//            var validSelectFields = await dbConnection
//                .GetValidatedDbFields(dbTableName, selectFields.Distinct())
//                .ConfigureAwait(false);

//            //Dynamically handle RepoDb where filters (QueryGroup)...
//            object whereParams = where != null
//                ? RepoDbQueryGroupProxy.GetMappedParamsObject<TEntity>(where)
//                : null;

//            //We increment Rows count to actually be fetched by One to help determine if there is any data after the current page...
//            var rowsPerPage = rowsPerBatch ?? int.MaxValue;
//            var rowsToFetch = rowsPerPage < int.MaxValue ? rowsPerPage + 1 : rowsPerPage;

//            var batchResults = (await dbConnection.BatchQueryAsync<TEntity>(
//                tableName: dbTableName,
//                page: page ?? 0,
//                rowsPerBatch: rowsToFetch,
//                fields: validSelectFields,
//                orderBy: orderBy,
//                where: whereParams,
//                hints: hints,
//                commandTimeout: commandTimeout,
//                transaction: transaction,
//                trace: trace,
//                cancellationToken: cancellationToken
//            ).ConfigureAwait(false))
//            ?.ToList() ?? new List<TEntity>();

//            //If specified we need to get the Total Count...
//            int totalCount = 0;
//            if (fetchTotalCount)
//            {
//                var repoDbTotalCount = await dbConnection.CountAsync(
//                    tableName: dbTableName,
//                    where: whereParams,
//                    hints: hints,
//                    commandTimeout: commandTimeout,
//                    transaction: transaction,
//                    trace: trace,
//                    cancellationToken: cancellationToken
//                ).ConfigureAwait(false);

//                totalCount = Convert.ToInt32(repoDbTotalCount);
//            }

//            //TODO: Implement Logic to determine HasNextPage, HasPreviousPage, and get Total Count or Not!
//            var hasPreviousPage = page > 0;
//            var hasNextPage = batchResults.Count > rowsPerPage;

//            //Trim the results to the exact page size (removing potential additional item).
//            var pageResults = batchResults.Take(rowsPerPage);

//            var offsetPageResults = new OffsetPageResults<TEntity>(pageResults, hasNextPage, hasPreviousPage, totalCount);
//            return offsetPageResults;
//        }

//        /// <summary>
//        /// CLONED from RepoDb source code that is marked as 'internal' but needed to handle in a consistent way!
//        /// Disposes an <see cref="IDbConnection"/> object if there is no <see cref="IDbTransaction"/> object connected
//        /// and if the current <see cref="ConnectionPersistency"/> value is <see cref="ConnectionPersistency.PerCall"/>.
//        /// </summary>
//        /// <typeparam name="TEntity"></typeparam>
//        /// <typeparam name="TDbConnection"></typeparam>
//        /// <param name="baseRepo"></param>
//        /// <param name="connection"></param>
//        /// <param name="transaction"></param>
//        private static void DisposeConnectionForPerCallExtension<TEntity, TDbConnection>(
//            this BaseRepository<TEntity, TDbConnection> baseRepo,
//            IDbConnection connection,
//            IDbTransaction transaction = null
//        )
//        where TEntity : class
//        where TDbConnection : DbConnection
//        {
//            if (baseRepo.ConnectionPersistency == ConnectionPersistency.PerCall)
//            {
//                if (transaction == null)
//                {
//                    connection?.Dispose();
//                }
//            }
//        }
  
//    }

//}
