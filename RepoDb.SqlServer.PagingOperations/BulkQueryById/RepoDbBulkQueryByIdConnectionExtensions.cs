using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using HotChocolate.RepoDb;
using Microsoft.Data.SqlClient;
using RepoDb.Interfaces;
using RepoDb.SqlServer.PagingOperations.QueryBuilders;

namespace RepoDb.SqlServer.PagingOperations.BulkQueryById
{
    public static class RepoDbBulkQueryByIdConnectionExtensions
    {
        #region Query By (Bulk) ID Methods

        /// <summary>
        /// Selecting data from Sql with Sql IN clause usually requires 1 Parameter for every value, and this result in
        /// safe Sql Queries, but there is a limit of 2100 parameters on a Sql Command.  This method provides a safe
        /// alternative implementation that is highly performant for large data sets using a list of int values (e.g Ids);
        /// which is very useful when you have Identity ID columns.
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="sqlConnection"></param>
        /// <param name="idList"></param>
        /// <param name="filterFieldName"></param>
        /// <param name="tableName"></param>
        /// <param name="fields"></param>
        /// <param name="orderBy"></param>
        /// <param name="hints"></param>
        /// <param name="cacheKey"></param>
        /// <param name="cacheItemExpiration"></param>
        /// <param name="commandTimeout"></param>
        /// <param name="transaction"></param>
        /// <param name="logTrace"></param>
        /// <param name="cancellationToken"></param>
        /// <param name="cache"></param>
        /// <returns></returns>
        public static async Task<IEnumerable<TEntity>> QueryBulkResultsByIdAsync<TEntity>(
            this SqlConnection sqlConnection,
            IEnumerable<int> idList,
            string filterFieldName = null,
            string tableName = null,
            IEnumerable<Field> fields = null,
            IEnumerable<OrderField> orderBy = null,
            string hints = null,
            string cacheKey = null,
            int? cacheItemExpiration = null,
            int? commandTimeout = null,
            IDbTransaction transaction = null,
            ICache cache = null,
            Action<string> logTrace = null,
            CancellationToken cancellationToken = default
        ) where TEntity : class
        {
            var connection = sqlConnection ?? throw new ArgumentNullException(nameof(sqlConnection));

            var timer = Stopwatch.StartNew();

            Field filterField;
            if (string.IsNullOrWhiteSpace(filterFieldName))
            {
                //Attempt to dynamically resolve the Filter Field as the Identity or Primary Key field (if the field is a Numeric Type)!
                var classProp = IdentityCache.Get<TEntity>() ?? PrimaryCache.Get<TEntity>();
                if (classProp == null || !classProp.PropertyInfo.PropertyType.IsNumericType())
                {
                    throw new ArgumentException(
                        $"The filter field name was not specified and an Int Id could not be dynamically resolved from the Identity or Primary Key properties for the type [{typeof(TEntity).Name}]",
                        nameof(filterFieldName)
                    );
                }

                filterField = new Field(classProp.GetMappedName());
            }
            else
            {
                //If Specified then we use the Filter Field Name specified and attempt to resolve it on the Model!
                filterField = new Field(PropertyMappedNameCache.Get<TEntity>(filterFieldName) ?? filterFieldName);
            }

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
            var validSelectFields = await connection
                .GetValidatedDbFieldsAsync(dbTableName, selectFields.Distinct())
                .ConfigureAwait(false);

            var dbSetting = connection.GetDbSetting();

            var query = new QueryBuilder()
                .Clear()
                .Select().FieldsFrom(validSelectFields, dbSetting)
                .From().TableNameFrom(dbTableName, dbSetting).WriteText("data")
                .WriteText("INNER JOIN STRING_SPLIT(@StringSplitCsvValues, ',') split")
                .On().WriteText("(data.").FieldFrom(filterField).WriteText("= split.value)")
                .OrderByFrom(orderBy, dbSetting)
                .HintsFrom(hints)
                .End();

            var commandText = query.GetString();
            var commandParams = new { StringSplitCsvValues = idList.ToCsvString(false) };

            logTrace?.Invoke($"Query: {commandText}");
            logTrace?.Invoke($"Query Param @StringSplitCsvValues: {commandParams.StringSplitCsvValues}");

            await connection.EnsureOpenAsync(cancellationToken: cancellationToken);
            logTrace?.Invoke($"DB Connection Established in: {timer.ToElapsedTimeDescriptiveFormat()}");

            //By creating a View Model of the data we are interested in we can easily query the View
            //  and teh complex many-to-many join is now encapsulated for us in the SQL View...
            var results = await connection.ExecuteQueryAsync<TEntity>(
                commandText,
                commandParams,
                commandType: CommandType.Text,
                commandTimeout: commandTimeout,
                transaction: transaction,
                cancellationToken: cancellationToken,
                cacheKey: cacheKey,
                cacheItemExpiration: cacheItemExpiration,
                cache: cache
            ).ConfigureAwait(false);

            logTrace?.Invoke($"Query Execution Completed in: {timer.ToElapsedTimeDescriptiveFormat()}");

            return results;
        }

        #endregion
    }
}
