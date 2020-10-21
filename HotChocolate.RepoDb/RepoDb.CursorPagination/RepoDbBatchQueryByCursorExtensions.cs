using HotChocolate.PreProcessedExtensions.Pagination;
using RepoDb;
using RepoDb.CustomExtensions;
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
            int afterCursor = 0, int firstTake = 0,
            int beforeCursor = 0, int lastTake = 0,
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
            //Validate arguments...
            AsserteCursorPagingArgsAreValid(firstTake, lastTake, orderBy);

            var tableName = ClassMappedNameCache.Get<TEntity>();

            //Ensure we have default fields; default is to include All Fields...
            var selectFields = fields?.Count() > 0
                ? fields
                : FieldCache.Get<TEntity>();

            //Convert enumerable Where fields to QueryGroup...
            var whereGroup = where != null 
                ? new QueryGroup(where) 
                : null;

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
                fields: selectFields,
                orderBy: orderBy,
                //TODO: Where is NOT IMPLEMENTED YET due to the utilties to easily map the QueryGroup to a query param object 
                //  being 'internal' scoped; that we will need to access....
                //whereGroup: whereGroup,
                hints: hints,
                afterCursor: afterCursor,
                firstTake: firstTake, 
                beforeCursor: beforeCursor,
                lastTake: lastTake,
                //Currently we MUST include the Total Count because it's required to tell if there is a previous/next page
                includeTotalCountQuery: true
            );

            //var results = await baseRepo.BatchQueryAsync(page, rowsPerBatch, orderBy, fields, hints, transaction, cancellationToken);
            using (var sqlConn = (DbConnection)transaction?.Connection ?? baseRepo.CreateConnection())
            {
                var cursorPageResult = await sqlConn.ExecuteBatchSliceQueryAsync<TEntity>(
                    commandText: query,
                    cancellationToken: cancellationToken
                );
                return cursorPageResult;
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
        ) where TEntity: class
        {

            //Get the Fields from Cache first (as this can't be done after Reader is opened...
            var tableNameForCache = tableName ?? ClassMappedNameCache.Get<TEntity>();
            var dbSetting = dbConn.GetDbSetting();
            var dbFieldsForCache = await DbFieldCache.GetAsync(dbConn, tableNameForCache, transaction, false, cancellationToken);

             //Ensure that the DB Connection is open (RepoDb provided extension)...
            await dbConn.EnsureOpenAsync();

            using (var dbCommand = (DbCommand)dbConn.CreateCommand(commandText, CommandType.Text, commandTimeout, transaction))
            using (var reader = await dbCommand.ExecuteReaderAsync())
            {
                //BBernard
                //NOTE: This Code Taken from RepoDb Source (DataReader.ToEnumerableAsync<TEntity>(...) core code
                //  so that we clone minimal amount of logic outside of RepoDb due to 'internal' scope.
                var results = new List<CursorResult<TEntity>>();
                int totalCount = 0;
                if (reader != null && !reader.IsClosed && reader.HasRows)
                {
                    var functionCacheProxy = new RepoDbFunctionCacheProxy<TEntity>();
                    var entityMappingFunc = functionCacheProxy.GetDataReaderToDataEntityFunctionCompatible(reader, dbConn, transaction, true, dbFieldsForCache, dbSetting);
                    string cursorIndexName = nameof(IHaveCursor.CursorIndex);

                    while (await reader.ReadAsync())
                    {
                        //Dynamically read the Entity from the Results...
                        TEntity entity = entityMappingFunc(reader);

                        //Manually Process the Cursor for each record...
                        //NOTE: we want to extract the values of the CursorIndex field and return in a Decorator class so so that 
                        //  there is NO REQUIREMENT that the original business Model (TEntity) have any special fields/interfaces added.
                        var cursorIndex = Convert.ToInt32(reader.GetValue(cursorIndexName));

                        var cursorResult = new CursorResult<TEntity>(entity, cursorIndex);
                        results.Add(cursorResult);
                    }

                    //Now attempt to step to the Total Count query result...
                    //Note: We know to attempt getting the TotalCount if there is a second result set avaialble.
                    if(await reader.NextResultAsync() && await reader.ReadAsync())
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


        public static void AsserteCursorPagingArgsAreValid(
            int firstTake = 0,
            int lastTake = 0,
            IEnumerable<OrderField> orderBy = null
        )
        {
            if (firstTake <= 0 && lastTake <= 0)
                throw new ArgumentException(
                    "At least one take size is required; either 'firstTake' or 'lastTake' size must be specified.",
                    nameof(firstTake)
                );

            if (orderBy?.Count() <= 0)
                throw new ArgumentException(
                    "A valid Order By field must be specified to ensure consistent ordering of data.",
                    nameof(orderBy)
                );
        }

 


    }

}
