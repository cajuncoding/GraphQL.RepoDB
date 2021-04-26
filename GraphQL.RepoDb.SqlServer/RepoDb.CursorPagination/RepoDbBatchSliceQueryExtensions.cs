using HotChocolate.PreProcessingExtensions.Pagination;
using HotChocolate.RepoDb.SqlServer.Reflection;
using RepoDb.CustomExtensions;
using RepoDb.Enumerations;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using HotChocolate.RepoDb;

namespace RepoDb.CursorPagination
{
    public static class RepoDbBatchSliceQueryExtensions
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
        /// <param name="afterCursor"></param>
        /// <param name="firstTake"></param>
        /// <param name="beforeCursor"></param>
        /// <param name="lastTake"></param>
        /// <param name="orderBy"></param>
        /// <param name="where"></param>
        /// <param name="tableName"></param>
        /// <param name="hints"></param>
        /// <param name="fields"></param>
        /// <param name="commandTimeout"></param>
        /// <param name="transaction"></param>
        /// <param name="logTrace"></param>
        /// <param name="cancellationToken"></param>
        /// <param name="computeTotalCount"></param>
        /// <returns>CursorPageSlice&lt;TEntity&gt;</returns>
        public static async Task<CursorPageSlice<TEntity>> GraphQLBatchSliceQueryAsync<TEntity, TDbConnection>(
            this BaseRepository<TEntity, TDbConnection> baseRepo,
            IEnumerable<OrderField> orderBy,
            //NOTE: Expression is required to prevent Ambiguous Signatures
            Expression<Func<TEntity, bool>> where,
            int? afterCursor = null, int? firstTake = null,
            int? beforeCursor = null, int? lastTake = null,
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
            return await baseRepo.GraphQLBatchSliceQueryAsync<TEntity, TDbConnection>(
                    afterCursor: afterCursor,
                    firstTake: firstTake,
                    beforeCursor: beforeCursor,
                    lastTake: lastTake,
                    orderBy: orderBy,
                    where: where != null ? QueryGroup.Parse<TEntity>(where) : (QueryGroup)null,
                    hints: hints,
                    fields: fields,
                    tableName: tableName,
                    commandTimeout: commandTimeout,
                    transaction: transaction,
                    logTrace: logTrace,
                    cancellationToken: cancellationToken,
                    computeTotalCount: computeTotalCount
            ).ConfigureAwait(false);
        }

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
        /// <param name="afterCursor"></param>
        /// <param name="firstTake"></param>
        /// <param name="beforeCursor"></param>
        /// <param name="lastTake"></param>
        /// <param name="orderBy"></param>
        /// <param name="where"></param>
        /// <param name="hints"></param>
        /// <param name="fields"></param>
        /// <param name="tableName"></param>
        /// <param name="commandTimeout"></param>
        /// <param name="transaction"></param>
        /// <param name="logTrace"></param>
        /// <param name="cancellationToken"></param>
        /// <param name="computeTotalCount"></param>
        /// <returns>CursorPageSlice&lt;TEntity&gt;</returns>
        public static async Task<CursorPageSlice<TEntity>> GraphQLBatchSliceQueryAsync<TEntity, TDbConnection>(
            this BaseRepository<TEntity, TDbConnection> baseRepo,
            IEnumerable<OrderField> orderBy,
            QueryGroup where = null,
            int? afterCursor = null, int? firstTake = null,
            int? beforeCursor = null, int? lastTake = null,
            string hints = null,
            IEnumerable<Field> fields = null,
            string tableName = null,
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
            //Below Logic mirrors that of RepoDb Source for managing the Connection (PerInstance or PerCall)!
            var connection = (DbConnection)(transaction?.Connection ?? baseRepo.CreateConnection());

            try
            {
                var cursorPageResult = await connection.GraphQLBatchSliceQueryAsync<TEntity>(
                    afterCursor: afterCursor,
                    firstTake: firstTake,
                    beforeCursor: beforeCursor,
                    lastTake: lastTake,
                    orderBy: orderBy,
                    where: where,
                    hints: hints,
                    fields: fields,
                    tableName: tableName,
                    commandTimeout: commandTimeout,
                    transaction: transaction,
                    logTrace: logTrace,
                    cancellationToken: cancellationToken,
                    computeTotalCount: computeTotalCount
                ).ConfigureAwait(false);

                return cursorPageResult;
            }
            //catch
            //{
            //    // Throw back the error
            //    throw;
            //}
            finally
            {
                // Dispose the connection
                baseRepo.DisposeConnectionForPerCallExtension(connection, transaction);
            }
        }

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
        /// <param name="afterCursor"></param>
        /// <param name="firstTake"></param>
        /// <param name="beforeCursor"></param>
        /// <param name="lastTake"></param>
        /// <param name="orderBy"></param>
        /// <param name="where"></param>
        /// <param name="hints"></param>
        /// <param name="fields"></param>
        /// <param name="commandTimeout"></param>
        /// <param name="transaction"></param>
        /// <param name="logTrace"></param>
        /// <param name="cancellationToken"></param>
        /// <param name="computeTotalCount"></param>
        /// <returns>CursorPageSlice&lt;TEntity&gt;</returns>
        public static async Task<CursorPageSlice<TEntity>> GraphQLBatchSliceQueryAsync<TEntity>(
            this DbConnection dbConnection,
            IEnumerable<OrderField> orderBy,
            //NOTE: Expression is required to prevent Ambiguous Signatures
            Expression<Func<TEntity, bool>> where,
            int? afterCursor = null, int? firstTake = null,
            int? beforeCursor = null, int? lastTake = null,
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
            return await dbConnection.GraphQLBatchSliceQueryAsync<TEntity>(
                afterCursor: afterCursor,
                firstTake: firstTake,
                beforeCursor: beforeCursor,
                lastTake: lastTake,
                orderBy: orderBy,
                where: where != null ? QueryGroup.Parse<TEntity>(where) : (QueryGroup)null,
                hints: hints,
                fields: fields,
                commandTimeout: commandTimeout,
                transaction: transaction,
                logTrace: logTrace,
                cancellationToken: cancellationToken,
                computeTotalCount: computeTotalCount
            ).ConfigureAwait(false);
        }

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
        /// <param name="afterCursor"></param>
        /// <param name="firstTake"></param>
        /// <param name="beforeCursor"></param>
        /// <param name="lastTake"></param>
        /// <param name="orderBy"></param>
        /// <param name="where"></param>
        /// <param name="hints"></param>
        /// <param name="fields"></param>
        /// <param name="tableName"></param>
        /// <param name="commandTimeout"></param>
        /// <param name="transaction"></param>
        /// <param name="logTrace"></param>
        /// <param name="cancellationToken"></param>
        /// <param name="computeTotalCount"></param>
        /// <returns>CursorPageSlice&lt;TEntity&gt;</returns>
        public static async Task<CursorPageSlice<TEntity>> GraphQLBatchSliceQueryAsync<TEntity>(
            this DbConnection dbConnection,
            IEnumerable<OrderField> orderBy,
            QueryGroup where = null, 
            int? afterCursor = null, int? firstTake = null,
            int? beforeCursor = null, int? lastTake = null,
            string hints = null,
            IEnumerable<Field> fields = null,
            string tableName = null,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            Action<string> logTrace = null,
            CancellationToken cancellationToken = default,
            bool computeTotalCount = false
        )
        //ALL entities retrieved and Mapped for Cursor Pagination must support IHaveCursor interface.
        where TEntity : class
        {
            if (orderBy == null)
                throw new ArgumentNullException(nameof(orderBy), "A sort order must be specified to provide valid cursor paging results.");

            var dbTableName = string.IsNullOrWhiteSpace(tableName)
                ? ClassMappedNameCache.Get<TEntity>()
                : tableName;

            //Ensure we have default fields; default is to include All Fields...
            var fieldsList = fields?.ToList();
            
            var selectFields = fieldsList?.Any() == true
                ? fieldsList
                : FieldCache.Get<TEntity>();

            //Retrieve only the select fields that are valid for the Database query!
            //NOTE: We guard against duplicate values as a convenience.
            var validSelectFields = await dbConnection
                .GetValidatedDbFields(dbTableName, selectFields.Distinct())
                .ConfigureAwait(false);

            //Dynamically handle RepoDb where filters (QueryGroup)...
            var whereParams = where != null
                ? RepoDbQueryGroupProxy.GetMappedParamsObject<TEntity>(where)
                : null;

            //Build the Cursor Paging query...
            var querySliceInfo = RepoDbBatchSliceQueryBuilder.BuildSqlServerBatchSliceQuery<TEntity>(
                tableName: dbTableName,
                fields: validSelectFields,
                orderBy: orderBy,
                where: where,
                hints: hints,
                afterCursorIndex: afterCursor,
                firstTake: firstTake,
                beforeCursorIndex: beforeCursor,
                lastTake: lastTake,
                //Optionally we compute the Total Count only when requested!
                includeTotalCountQuery: computeTotalCount
            );

            //Now we can execute the process and get the results!
            var cursorPageResult = await dbConnection.ExecuteBatchSliceQueryAsync<TEntity>(
                sqlQuerySliceInfo: querySliceInfo,
                queryParams: whereParams,
                tableName: dbTableName,
                commandTimeout: commandTimeout,
                transaction: transaction,
                logTrace: logTrace,
                cancellationToken: cancellationToken
            ).ConfigureAwait(false);

            return cursorPageResult;
        }

        /// <summary>
        /// CLONED from RepoDb source code that is marked as 'internal' but needed to handle in a consistent way!
        /// Disposes an <see cref="IDbConnection"/> object if there is no <see cref="IDbTransaction"/> object connected
        /// and if the current <see cref="ConnectionPersistency"/> value is <see cref="ConnectionPersistency.PerCall"/>.
        /// </summary>
        /// <param name="baseRepo"></param>
        /// <param name="connection">The instance of <see cref="IDbConnection"/> object.</param>
        /// <param name="transaction">The instance of <see cref="IDbTransaction"/> object.</param>
        private static void DisposeConnectionForPerCallExtension<TEntity, TDbConnection>(
            this BaseRepository<TEntity, TDbConnection> baseRepo,
            IDbConnection connection,
            IDbTransaction transaction = null
        )
        where TEntity : class
        where TDbConnection : DbConnection
        {
            if (baseRepo.ConnectionPersistency == ConnectionPersistency.PerCall)
            {
                if (transaction == null)
                {
                    connection?.Dispose();
                }
            }
        }


        /// <summary>
        /// Internal query execution method for Slice Queries based on TEntity model. We attempt to use as much as possible
        ///     from RepoDb, with the caveat that some resources/utilities are internal and require brute force via reflection
        ///     to access.
        ///     
        /// NOTE: We must manually construct our Reader as the Helpers from RepoDb are 'internal' scope
        ///      and not readily accessible, though for this it's important that we have access to the reader
        ///      so that we can manually extract the CursorIndex that was dynamically added via the Query!
        /// NOTE: Since this doesn't have access to all RepoDb internals, there are some less optimal things such as:
        ///          - RepoDb CreateDbCommandForExecution() method isn't accessible and it performs greater validation.
        ///          - We use Reflection to get access to some internal elements via Brute Force, but cache the access
        ///              via proxy class that mitigates performance issues.
        /// 
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="dbConn"></param>
        /// <param name="sqlQuerySliceInfo"></param>
        /// <param name="queryParams"></param>
        /// <param name="tableName"></param>
        /// <param name="commandTimeout"></param>
        /// <param name="transaction"></param>
        /// <param name="logTrace"></param>
        /// <param name="cancellationToken"></param>
        /// <returns>CursorPageSlice&lt;TEntity&gt;</returns>
        [SuppressMessage("Style", "IDE0063:Use simple 'using' statement", Justification = "<Pending>")]
        private static async Task<CursorPageSlice<TEntity>> ExecuteBatchSliceQueryAsync<TEntity>(
            this DbConnection dbConn,
            SqlQuerySliceInfo sqlQuerySliceInfo,
            object queryParams = null,
            string tableName = null,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            Action<string> logTrace = null,
            CancellationToken cancellationToken = default
        ) where TEntity : class
        {
            var commandText = sqlQuerySliceInfo.SQL;

            logTrace?.Invoke($"Query: {commandText}");

            var timer = Stopwatch.StartNew();

            //Ensure that the DB Connection is open (RepoDb provided extension)...
            await dbConn.EnsureOpenAsync(cancellationToken).ConfigureAwait(false);

            logTrace?.Invoke($"DB Connection Established in: {timer.ToElapsedTimeDescriptiveFormat()}");

            //Re-use the RepoDb Execute Reader method to get benefits of Command & Param caches, etc.
            await using var reader = (DbDataReader)await dbConn.ExecuteReaderAsync(
                commandText: commandText,
                param: queryParams,
                commandType: CommandType.Text,
                transaction: transaction,
                commandTimeout: commandTimeout,
                cancellationToken: cancellationToken
            ).ConfigureAwait(false);

            //BBernard
            //We NEED to manually process the Reader externally here!
            //Therefore, this had to be Code borrowed from RepoDb Source (DataReader.ToEnumerableAsync<TEntity>(...) 
            // core code so that we clone minimal amount of logic outside of RepoDb due to 'internal' scope.
            var results = new List<CursorResult<TEntity>>();
            int? totalCount = null;
            if (reader != null && !reader.IsClosed && reader.HasRows)
            {
                const string cursorIndexPropName = nameof(IHaveCursor.CursorIndex);

                //It's exposed for easy use by extensions so we use Brute Force Reflection to get the Model Mapping Function
                //  because RepoDb does this very well already!
                //NOTE: The complied Mapping Func (delegate) that is retrieved is lazy loaded into a static cache reference
                //      by Generic Type <TEntity> for extremely high performance once initialized!
                var repoDbModelMappingFunc = await GetRepoDbModelMappingFuncByBruteForce<TEntity>(dbConn, reader, tableName, transaction, cancellationToken);

                while (await reader.ReadAsync(cancellationToken).ConfigureAwait(false))
                {
                    //Dynamically read the Entity from the Results...
                    TEntity entity = repoDbModelMappingFunc(reader);

                    //Manually Process the Cursor for each record...
                    var cursorIndex = Convert.ToInt32(reader.GetValue(cursorIndexPropName));

                    //This allows us to extract the CursorIndex field and return in a Decorator class 
                    //  so there's NO REQUIREMENT that the Model (TEntity) have any special fields/interfaces added.
                    var cursorResult = new CursorResult<TEntity>(entity, cursorIndex);
                    results.Add(cursorResult);
                }

                //Now attempt to step to the Total Count query result...
                //Note: We know to attempt getting the TotalCount if there is a second result set available.
                if (await reader.NextResultAsync(cancellationToken).ConfigureAwait(false) 
                    && await reader.ReadAsync(cancellationToken).ConfigureAwait(false))
                {
                    //This is a Scalar query so the first ordinal value is the Total Count!
                    totalCount = Convert.ToInt32(reader.GetValue(0));
                }

            }

            timer.Stop();
            logTrace?.Invoke($"Query Execution Time: {timer.ToElapsedTimeDescriptiveFormat()}");

            //Process the Results and determine Pagination metadata, etc.
            var cursorPage = PostProcessResultsIntoCursorPageSlice(results, sqlQuerySliceInfo, totalCount);
            return cursorPage;
        }


        private static async Task<Func<DbDataReader, TEntity>> GetRepoDbModelMappingFuncByBruteForce<TEntity>(
            DbConnection dbConn,
            DbDataReader reader,
            string tableName = null,
            IDbTransaction transaction = null,
            CancellationToken cancellationToken = default
        )
        {
            //Get the Fields from Cache first (as this can't be done after Reader is opened...
            var tableNameForCache = string.IsNullOrWhiteSpace(tableName)
                ? ClassMappedNameCache.Get<TEntity>()
                : tableName;

            var dbSetting = dbConn.GetDbSetting();

            var dbFieldsForCache = await DbFieldCache
                .GetAsync(dbConn, tableNameForCache, transaction, false, cancellationToken)
                .ConfigureAwait(false);

            //Initialize the RepoDb compiled entity mapping function (via Brute Force Proxy class; it's marked 'internal'.
            var functionCacheProxy = new RepoDbFunctionCacheProxy<TEntity>();

            var repoDbMappingFunc = functionCacheProxy.GetDataReaderToDataEntityFunctionCompatible(
                reader, dbConn, transaction, true, dbFieldsForCache, dbSetting
            ) ?? throw new Exception($"Unable to retrieve the RepoDb entity mapping function for [{typeof(TEntity).Name}].");

            return repoDbMappingFunc;
        }

        private static CursorPageSlice<TEntity> PostProcessResultsIntoCursorPageSlice<TEntity>(
            List<CursorResult<TEntity>> results, 
            SqlQuerySliceInfo sqlQuerySliceInfo, 
            int? totalCount
        ) where TEntity : class
        {
            bool hasPreviousPage = false;
            bool hasNextPage = false;

            if (sqlQuerySliceInfo.IsPreviousPagePossible)
            {
                var firstCursor = results.FirstOrDefault();
                hasPreviousPage = firstCursor?.CursorIndex > 1; //Cursor Index is 1 Based; 0 would be the Cursor before the First
            }

            if (sqlQuerySliceInfo.IsNextPagePossible)
            {
                //GENERALLY This should Always Be True as we always increment the EndIndex if there is the Possibility that there might
                //  be a NEXT Page, and the ExpectedCount is always a value that should satisfy the processing
                //  (e.g. ExpectedCount might be int.MaxValue which would ensure our Take is always successful to get ALL Results).
                if (sqlQuerySliceInfo.IsEndIndexOverFetchedForNextPageCheck && sqlQuerySliceInfo.ExpectedCount < int.MaxValue)
                {
                    hasNextPage = results.Count > sqlQuerySliceInfo.ExpectedCount;
                    if (hasNextPage)
                    {
                        results.RemoveAt(results.Count - 1);
                    }

                }
            }

            //Return a CursorPagedResult decorator for the results along with the Total Count!
            var cursorPage = new CursorPageSlice<TEntity>(results, totalCount, hasPreviousPage, hasNextPage);
            return cursorPage;
        }

    }


}
