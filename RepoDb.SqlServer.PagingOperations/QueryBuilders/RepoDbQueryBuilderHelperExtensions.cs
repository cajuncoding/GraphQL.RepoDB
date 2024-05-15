using RepoDb.Extensions;
using System.Collections.Generic;
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
            if (dbConnection == null || selectFields == null || string.IsNullOrWhiteSpace(tableName))
                return null;

            //FILTER for only VALID fields from the selection by safely comparing to the valid fields from the DB Schema!
            //NOTE: Per RepoDb source we need to compare unquoted names to get pure matches...
            var dbSetting = dbConnection.GetDbSetting();
            var dbFields = await DbFieldCache
                .GetAsync(dbConnection, tableName, null, false)
                .ConfigureAwait(false);

            var dbFieldLookup = dbFields.ToLookup(f => f.Name.AsUnquoted(dbSetting).ToLower());
            var safeSelectFields = selectFields.Where(s => dbFieldLookup.Contains(s.Name.AsUnquoted(dbSetting).ToLower()));

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
            if (dbConnection == null || sortFields == null || string.IsNullOrWhiteSpace(tableName))
                return null;

            //FILTER for only VALID fields from the sort fields by safely comparing to the valid fields from the DB Schema!
            //NOTE: Per RepoDb source we need to compare unquoted names to get pure matches...
            var dbSetting = dbConnection.GetDbSetting();
            var dbFields = await DbFieldCache
                .GetAsync(dbConnection, tableName, null, false)
                .ConfigureAwait(false);

            var dbFieldLookup = dbFields.ToLookup(f => f.Name.AsUnquoted(dbSetting).ToLower());
            var safeSortFields = sortFields.Where(s => dbFieldLookup.Contains(s.Name.AsUnquoted(dbSetting).ToLower()));

            return safeSortFields;
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
