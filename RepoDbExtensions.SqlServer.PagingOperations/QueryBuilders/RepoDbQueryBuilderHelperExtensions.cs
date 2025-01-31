using RepoDb.Extensions;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;

namespace RepoDb.SqlServer.PagingOperations.QueryBuilders
{
    /// <summary>
    /// NOTE: These may be useful for consumers to help build valid SQL queries dynamically, so we share them as public methods.
    /// </summary>
    public static class RepoDbQueryBuilderHelperExtensions
    {
        public static async Task<IEnumerable<Field>> GetValidatedDbFieldsAsync<TEntity>(
            this DbConnection dbConnection,
            IEnumerable<Field> selectFields
        )
        {
            var tableName = ClassMappedNameCache.Get<TEntity>();
            var safeSelectFields = await dbConnection.GetValidatedDbFieldsAsync(tableName, selectFields).ConfigureAwait(false);
            return safeSelectFields;
        }

        public static async Task<IEnumerable<Field>> GetValidatedDbFieldsAsync(
            this DbConnection dbConnection,
            string tableName,
            IEnumerable<Field> selectFields
        )
        {
            var dbFieldCollection = await GetDbFieldCollectionForTableInternalAsync(dbConnection, tableName).ConfigureAwait(false);
            if (dbFieldCollection == null || selectFields == null)
                return null;

            var dbSetting = dbConnection.GetDbSetting();
            var safeSelectFields = selectFields.Where(s => !(dbFieldCollection.GetByUnquotedName(s.Name.AsUnquoted(dbSetting)) is null));
            
            return safeSelectFields;
        }

        public static async Task<IEnumerable<OrderField>> GetValidatedDbFieldsAsync<TEntity>(
            this DbConnection dbConnection,
            IEnumerable<OrderField> sortFields
        )
        {
            var tableName = ClassMappedNameCache.Get<TEntity>();
            var safeSortFields = await dbConnection.GetValidatedDbFieldsAsync(tableName, sortFields).ConfigureAwait(false);
            return safeSortFields;
        }

        public static async Task<IEnumerable<OrderField>> GetValidatedDbFieldsAsync(
            this DbConnection dbConnection,
            string tableName,
            IEnumerable<OrderField> sortFields
        )
        {
            var dbFieldCollection = await GetDbFieldCollectionForTableInternalAsync(dbConnection, tableName).ConfigureAwait(false);
            if (dbFieldCollection == null || sortFields == null)
                return null;

            //FILTER for only VALID fields from the sort fields by safely comparing to the valid fields from the DB Schema!
            //NOTE: Per RepoDb source we need to compare unquoted names to get pure matches...
            var dbSetting = dbConnection.GetDbSetting();
            var safeSortFields = sortFields.Where(s => !(dbFieldCollection.GetByUnquotedName(s.Name.AsUnquoted(dbSetting)) is null));

            return safeSortFields;
        }

        /// <summary>
        /// Helper Method to get the Field Collection from RepoDb...
        /// </summary>
        private static async Task<DbFieldCollection> GetDbFieldCollectionForTableInternalAsync(this DbConnection dbConnection, string tableName)
        {
            if (dbConnection == null || string.IsNullOrWhiteSpace(tableName))
                return null;

            return await DbFieldCache
                .GetAsync(dbConnection, tableName, null, false)
                .ConfigureAwait(false);
        }

        public static async Task<string> GetValidatedSelectClauseAsync<TEntity>(
            this DbConnection dbConnection,
            IEnumerable<Field> selectFields
        )
        {
            var tableName = ClassMappedNameCache.Get<TEntity>();
            var safeSelectClause = await dbConnection.GetValidatedSelectClauseAsync(tableName, selectFields).ConfigureAwait(false);
            return safeSelectClause;
        }

        public static async Task<string> GetValidatedSelectClauseAsync(
            this DbConnection dbConnection,
            string tableName,
            IEnumerable<Field> selectFields
        )
        {
            if (dbConnection == null || selectFields == null || string.IsNullOrWhiteSpace(tableName))
                return null;

            var validSelectFields = await dbConnection.GetValidatedDbFieldsAsync(tableName, selectFields).ConfigureAwait(false);

            var dbSetting = dbConnection.GetDbSetting();
            var safeSelectClause = new QueryBuilder().FieldsFrom(validSelectFields, dbSetting).GetString();
            return safeSelectClause;
        }

        public static async Task<string> GetValidatedOrderByClauseAsync<TEntity>(
            this DbConnection dbConnection,
            IEnumerable<OrderField> sortFields
        )
        {
            var tableName = ClassMappedNameCache.Get<TEntity>();
            var safeSortClause = await dbConnection.GetValidatedOrderByClauseAsync(tableName, sortFields).ConfigureAwait(false);
            return safeSortClause;
        }

        public static async Task<string> GetValidatedOrderByClauseAsync(
            this DbConnection dbConnection,
            string tableName,
            IEnumerable<OrderField> sortFields
        )
        {
            if (dbConnection == null || sortFields == null || string.IsNullOrWhiteSpace(tableName))
                return null;

            var safeSortFields = await dbConnection.GetValidatedDbFieldsAsync(tableName, sortFields).ConfigureAwait(false);

            var dbSetting = dbConnection.GetDbSetting();
            var safeSortClause = new QueryBuilder().OrderByFrom(safeSortFields, dbSetting).GetString();
            return safeSortClause;
        }
    }
}
