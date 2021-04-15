using RepoDb;
using RepoDb.Extensions;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;

namespace RepoDb
{
    public static class RepoDbHelperConnectionExtensions
    {
        public static async Task<IEnumerable<Field>> GetValidatedDbFields<TEntity>(
            this DbConnection dbConnection,
            IEnumerable<Field> selectFields
        )
        {
            var tableName = ClassMappedNameCache.Get<TEntity>();
            var safeSelectFields = await dbConnection.GetValidatedDbFields(tableName, selectFields).ConfigureAwait(false);
            return safeSelectFields;
        }

        public static async Task<IEnumerable<Field>> GetValidatedDbFields(
            this DbConnection dbConnection,
            string tableName,
            IEnumerable<Field> selectFields
        )
        {
            if (dbConnection == null || selectFields == null || string.IsNullOrWhiteSpace(tableName))
                return null;

            //FILTER for only VALID fields from the seleciton by safely comparing to the valid fields from the DB Schema!
            //NOTE: Per RepoDb source we need to compare unquoted names to get pure matches...
            var dbSetting = dbConnection.GetDbSetting();
            var dbFields = await DbFieldCache
                .GetAsync(dbConnection, tableName, null, false)
                .ConfigureAwait(false);
            
            var dbFieldLookup = dbFields.ToLookup(f => f.Name.AsUnquoted(dbSetting).ToLower());
            var safeSelectFields = selectFields.Where(s => dbFieldLookup[s.Name.AsUnquoted(dbSetting).ToLower()].Any());
            
            return safeSelectFields;
        }

        public static async Task<IEnumerable<OrderField>> GetValidatedDbFields<TEntity>(
            this DbConnection dbConnection,
            IEnumerable<OrderField> sortFields
        )
        {
            var tableName = ClassMappedNameCache.Get<TEntity>();
            var safeSortFields = await dbConnection.GetValidatedDbFields(tableName, sortFields).ConfigureAwait(false);
            return safeSortFields;
        }

        public static async Task<IEnumerable<OrderField>> GetValidatedDbFields(
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
            var safeSortFields = sortFields.Where(s => dbFieldLookup[s.Name.AsUnquoted(dbSetting).ToLower()].Any());

            return safeSortFields;
        }

        public static async Task<string> GetValidatedSelectClause<TEntity>(
            this DbConnection dbConnection,
            IEnumerable<Field> selectFields
        )
        {
            var tableName = ClassMappedNameCache.Get<TEntity>();
            var safeSelectClause = await dbConnection.GetValidatedSelectClause(tableName, selectFields).ConfigureAwait(false);
            return safeSelectClause;
        }

        public static async Task<string> GetValidatedSelectClause(
            this DbConnection dbConnection,
            string tableName,
            IEnumerable<Field> selectFields
        )
        {
            if (dbConnection == null || selectFields == null || string.IsNullOrWhiteSpace(tableName))
                return null;

            var validSelectFields = await dbConnection.GetValidatedDbFields(tableName, selectFields).ConfigureAwait(false);
            
            var dbSetting = dbConnection.GetDbSetting();
            var safeSelectClause = new QueryBuilder().FieldsFrom(validSelectFields, dbSetting).GetString();
            return safeSelectClause;
        }

        public static async Task<string> GetValidatedOrderByClause<TEntity>(
            this DbConnection dbConnection,
            IEnumerable<OrderField> sortFields
        )
        {
            var tableName = ClassMappedNameCache.Get<TEntity>();
            var safeSortClause = await dbConnection.GetValidatedOrderByClause(tableName, sortFields).ConfigureAwait(false);
            return safeSortClause;
        }

        public static async Task<string> GetValidatedOrderByClause(
            this DbConnection dbConnection,
            string tableName,
            IEnumerable<OrderField> sortFields
        )
        {
            if (dbConnection == null || sortFields == null || string.IsNullOrWhiteSpace(tableName))
                return null;

            var safeSortFields = await dbConnection.GetValidatedDbFields(tableName, sortFields).ConfigureAwait(false);

            var dbSetting = dbConnection.GetDbSetting();
            var safeSortClause = new QueryBuilder().OrderByFrom(safeSortFields, dbSetting).GetString();
            return safeSortClause;
        }
    }
}
