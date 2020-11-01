using HotChocolate.PreProcessingExtensions.Pagination;
using HotChocolate.RepoDb;
using HotChocolate.RepoDb.SqlServer.Reflection;
using RepoDb;
using RepoDb.CustomExtensions;
using RepoDb.Enumerations;
using RepoDb.Extensions;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace RepoDb.CursorPagination
{
    public static class BaseRepositoryCursorPaginationCustomExtensions
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
        /// <param name="hints"></param>
        /// <param name="fields"></param>
        /// <param name="transaction"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public static async Task<CursorPageSlice<TEntity>> GraphQLBatchSliceQueryAsync<TEntity, TDbConnection>(
            this BaseRepository<TEntity, TDbConnection> baseRepo,
            IEnumerable<OrderField> orderBy,
            Expression<Func<TEntity, bool>> where,
            int? afterCursor = null, int? firstTake = null,
            int? beforeCursor = null, int? lastTake = null,
            string tableName = null,
            string hints = null,
            IEnumerable<Field> fields = null,
            IDbTransaction transaction = null,
            CancellationToken cancellationToken = default
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
                    transaction: transaction,
                    cancellationToken: cancellationToken
                );
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
        /// <param name="transaction"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public static async Task<CursorPageSlice<TEntity>> GraphQLBatchSliceQueryAsync<TEntity, TDbConnection>(
            this BaseRepository<TEntity, TDbConnection> baseRepo,
            IEnumerable<OrderField> orderBy,
            QueryGroup where = null,
            int? afterCursor = null, int? firstTake = null,
            int? beforeCursor = null, int? lastTake = null,
            string hints = null,
            IEnumerable<Field> fields = null,
            string tableName = null,
            IDbTransaction transaction = null,
            CancellationToken cancellationToken = default
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
                    transaction: transaction,
                    cancellationToken: cancellationToken
                );

                return cursorPageResult;
            }
            catch
            {
                // Throw back the error
                throw;
            }
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
        /// <param name="transaction"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public static async Task<CursorPageSlice<TEntity>> GraphQLBatchSliceQueryAsync<TEntity>(
            this DbConnection dbConnection,
            IEnumerable<OrderField> orderBy,
            Expression<Func<TEntity, bool>> where,
            int? afterCursor = null, int? firstTake = null,
            int? beforeCursor = null, int? lastTake = null,
            string hints = null,
            IEnumerable<Field> fields = null,
            IDbTransaction transaction = null,
            CancellationToken cancellationToken = default
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
                transaction: transaction,
                cancellationToken: cancellationToken
            );
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
        /// <param name="transaction"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public static async Task<CursorPageSlice<TEntity>> GraphQLBatchSliceQueryAsync<TEntity>(
            this DbConnection dbConnection,
            IEnumerable<OrderField> orderBy,
            QueryGroup where = null, 
            int? afterCursor = null, int? firstTake = null,
            int? beforeCursor = null, int? lastTake = null,
            string hints = null,
            IEnumerable<Field> fields = null,
            string tableName = null,
            IDbTransaction transaction = null,
            CancellationToken cancellationToken = default
        )
        //ALL entities retrieved and Mapped for Cursor Pagination must support IHaveCursor interface.
        where TEntity : class
        {
            if (orderBy == null)
                throw new ArgumentNullException("A sort order must be specified to provide valid cursor paging results.", nameof(orderBy));

            var dbTableName = string.IsNullOrWhiteSpace(tableName)
                                ? ClassMappedNameCache.Get<TEntity>()
                                : tableName;

            //Ensure we have default fields; default is to include All Fields...
            var selectFields = fields?.Any() == true
                ? fields
                : FieldCache.Get<TEntity>();

            //Retrieve only the select fields that are valid for the Database query!
            //NOTE: We guard against duplicate values as a convenience.
            var validSelectFields = await dbConnection.GetValidatedDbFields(dbTableName, selectFields.Distinct());

            //Dynamically hanlde RepoDb where filters (QueryGroup)...
            object whereParams = where != null
                ? RepoDbQueryGroupProxy.GetMappedParamsObject<TEntity>(where)
                : null;

            //Build the Cursor Paging query...
            var query = RepoDbCursorPagingQueryBuilder.BuildSqlServerBatchSliceQuery<TEntity>(
                tableName: dbTableName,
                fields: validSelectFields,
                orderBy: orderBy,
                where: where,
                hints: hints,
                afterCursorIndex: afterCursor,
                firstTake: firstTake,
                beforeCursorIndex: beforeCursor,
                lastTake: lastTake,
                //Currently we MUST include the Total Count because it's required to tell if there is a previous/next page
                includeTotalCountQuery: true
            );

            var cursorPageResult = await dbConnection.ExecuteBatchSliceQueryAsync<TEntity>(
                commandText: query,
                queryParams: whereParams,
                cancellationToken: cancellationToken,
                transaction: transaction
            );

            return cursorPageResult;
        }

        /// <summary>
        /// CLONED from RepoDb source code that is marked as 'internal' but needed to handle in a consistent way!
        /// Disposes an <see cref="IDbConnection"/> object if there is no <see cref="IDbTransaction"/> object connected
        /// and if the current <see cref="ConnectionPersistency"/> value is <see cref="ConnectionPersistency.PerCall"/>.
        /// </summary>
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
        ///     from RepoDb, with the caveat that some resources/utitlies are internal and requrie brute force via relfection
        ///     to access.
        ///     
        /// NOTE: We must manually construct our Reader as the Helpers from RepoDb are 'internal' scope
        ///      and not readily accessible, though for this it's important taht we have access to the reader
        ///      so that we can manually extract the CursorIndex that was dynamically added via the Query!
        /// NOTE: Since this doesn't have access to all RepoDb internals, there are some less optimal things such as:
        ///          - RepoDb CreateDbCommandForExecution() method isn't accessible and it performs greater validation.
        ///          - We use Reflection to get access to some internal elements via Brute Force, but cache the access
        ///              via proxy class that mitigates performance issues.
        ///
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="dbConn"></param>
        /// <param name="commandText"></param>
        /// <param name="queryParams"></param>
        /// <param name="commandTimeout"></param>
        /// <param name="transaction"></param>
        /// <param name="tableName"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [SuppressMessage("Style", "IDE0063:Use simple 'using' statement", Justification = "<Pending>")]
        private static async Task<CursorPageSlice<TEntity>> ExecuteBatchSliceQueryAsync<TEntity>(
            this DbConnection dbConn,
            string commandText,
            object queryParams = null,
            string tableName = null,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            CancellationToken cancellationToken = default
        ) where TEntity : class
        {

            //Get the Fields from Cache first (as this can't be done after Reader is opened...
            var tableNameForCache = tableName ?? ClassMappedNameCache.Get<TEntity>();
            var dbSetting = dbConn.GetDbSetting();
            var dbFieldsForCache = await DbFieldCache.GetAsync(dbConn, tableNameForCache, transaction, false, cancellationToken);

            //Ensure that the DB Connection is open (RepoDb provided extension)...
            await dbConn.EnsureOpenAsync();

            //Re-use the RepoDb Execute Reader method to get benefits of Command & Param caches, etc.
            using (var reader = (DbDataReader)await dbConn.ExecuteReaderAsync(
                commandText: commandText,
                param: queryParams,
                commandType: CommandType.Text,
                transaction: transaction,
                commandTimeout: commandTimeout,
                cancellationToken: cancellationToken
                ))
            {
                //BBernard
                //We NEED to manually process the Reader externally here!
                //Therefore, this had to be Code borrowed from RepoDb Source (DataReader.ToEnumerableAsync<TEntity>(...) 
                // core code so that we clone minimal amount of logic outside of RepoDb due to 'internal' scope.
                var results = new List<CursorResult<TEntity>>();
                int totalCount = 0;
                if (reader != null && !reader.IsClosed && reader.HasRows)
                {
                    string cursorIndexName = nameof(IHaveCursor.CursorIndex);

                    //Initialie the RepoDb compiled entity mapping function (via Brute Force Proxy class; it's marked 'internal'.
                    var functionCacheProxy = new RepoDbFunctionCacheProxy<TEntity>();
                    var repoDbMappingFunc = functionCacheProxy.GetDataReaderToDataEntityFunctionCompatible(
                        reader, dbConn, transaction, true, dbFieldsForCache, dbSetting
                    ) ?? throw new Exception($"Unable to retrieve the RepoDb entity mapping function for [{typeof(TEntity).Name}].");

                    while (await reader.ReadAsync(cancellationToken))
                    {
                        //Dynamically read the Entity from the Results...
                        TEntity entity = repoDbMappingFunc(reader);

                        //Manually Process the Cursor for each record...
                        var cursorIndex = Convert.ToInt32(reader.GetValue(cursorIndexName));

                        //This allows us to extract the CursorIndex field and return in a Decorator class 
                        //  so there's NO REQUIREMENT that the Model (TEntity) have any special fields/interfaces added.
                        var cursorResult = new CursorResult<TEntity>(entity, cursorIndex);
                        results.Add(cursorResult);
                    }

                    //Now attempt to step to the Total Count query result...
                    //Note: We know to attempt getting the TotalCount if there is a second result set avaialble.
                    if (await reader.NextResultAsync() && await reader.ReadAsync())
                    {
                        //This is a Scalar query so the first ordinal value is the Total Count!
                        totalCount = Convert.ToInt32(reader.GetValue(0));
                    }

                }

                //Return a CursorPagedResult decorator for the results along with the Total Count!
                var cursorPage = new CursorPageSlice<TEntity>(results, totalCount);
                return cursorPage;
            }
        }
    }

}
