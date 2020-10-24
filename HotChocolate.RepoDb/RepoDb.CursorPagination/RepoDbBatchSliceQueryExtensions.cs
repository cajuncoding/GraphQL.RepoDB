using HotChocolate.PreProcessedExtensions.Pagination;
using RepoDb;
using RepoDb.CustomExtensions;
using RepoDb.Enumerations;
using RepoDb.Extensions;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace RepoDb.CursorPagination
{
    public static class BaseRepositoryCustomExtensions
    {
        public static async Task<CursorPageSlice<TEntity>> BatchSliceQueryAsync<TEntity, TDbConnection>(
            this BaseRepository<TEntity, TDbConnection> baseRepo,
            int? afterCursor = null, int? firstTake = null,
            int? beforeCursor = null, int? lastTake = null,
            IEnumerable<OrderField> orderBy = null,
            IEnumerable<QueryField> where = null,
            string hints = null,
            IEnumerable<Field> fields = null,
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
                var cursorPageResult = await connection.BatchSliceQueryAsync<TEntity>(
                    afterCursor: afterCursor,
                    firstTake: firstTake,
                    beforeCursor: beforeCursor,
                    lastTake: lastTake,
                    orderBy: orderBy,
                    where: where,
                    hints: hints,
                    fields: fields,
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


        public static async Task<CursorPageSlice<TEntity>> BatchSliceQueryAsync<TEntity>(
            this DbConnection dbConnection,
            int? afterCursor = null, int? firstTake = null,
            int? beforeCursor = null, int? lastTake = null,
            IEnumerable<OrderField> orderBy = null,
            IEnumerable<QueryField> where = null,
            string hints = null,
            IEnumerable<Field> fields = null,
            IDbTransaction transaction = null,
            CancellationToken cancellationToken = default
        )
        //ALL entities retrieved and Mapped for Cursor Pagination must support IHaveCursor interface.
        where TEntity : class
        {
            var tableName = ClassMappedNameCache.Get<TEntity>();

            //Ensure we have default fields; default is to include All Fields...
            var selectFields = fields?.Count() > 0
                ? fields
                : FieldCache.Get<TEntity>();

            //Convert enumerable Where fields to QueryGroup...
            var whereGroup = where != null
                ? new QueryGroup(where)
                : null;

            //FILTER for only VALID fields from the seleciton by safely comparing to the valid fields from the DB Schema!
            //NOTE: Per RepoDb source we need to compare unquoated names to get pure matches...
            var dbSetting = dbConnection.GetDbSetting();
            var dbFields = await DbFieldCache.GetAsync(dbConnection, tableName, transaction, false, cancellationToken);
            var dbFieldLookup = dbFields.ToLookup(f => f.Name.AsUnquoted(dbSetting).ToLower());
            var validSelectFields = selectFields.Where(s => dbFieldLookup[s.Name.AsUnquoted(dbSetting).ToLower()].Any());

            //TODO: Where Filters NOT IMPLEMENTED YET due to the utilties to easily map the QueryGroup to a query param object 
            //  being 'internal' scoped; that we will need to access....
            //// Converts to propery mapped object
            //if (where != null)
            //{
            //    param = QueryGroup.AsMappedObject(new[] { where.MapTo<TEntity>() });
            //}

            //Build the Cursor Paging query...
            var query = RepoDbCursorPagingQueryBuilder.BuildSqlServerBatchSliceQuery<TEntity>(
                tableName: tableName,
                fields: validSelectFields,
                orderBy: orderBy,
                //TODO: Where is NOT IMPLEMENTED YET due to the utilties to easily map the QueryGroup to a query param object 
                //  being 'internal' scoped; that we will need to access....
                //whereGroup: whereGroup,
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
                cancellationToken: cancellationToken
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
                    connection.Dispose();
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
        private static async Task<CursorPageSlice<TEntity>> ExecuteBatchSliceQueryAsync<TEntity>(
            this DbConnection dbConn,
            string commandText,
            object queryParams = null,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            string tableName = null,
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
