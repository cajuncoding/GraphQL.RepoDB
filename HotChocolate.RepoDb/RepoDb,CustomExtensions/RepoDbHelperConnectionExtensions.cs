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
            var validFields = await dbConnection.GetValidatedDbFields(tableName, selectFields);
            return validFields;
        }

        public static async Task<IEnumerable<Field>> GetValidatedDbFields(
            this DbConnection dbConnection,
            string tableName,
            IEnumerable<Field> selectFields
        )
        {
            //FILTER for only VALID fields from the seleciton by safely comparing to the valid fields from the DB Schema!
            //NOTE: Per RepoDb source we need to compare unquoated names to get pure matches...
            var dbSetting = dbConnection.GetDbSetting();
            var dbFields = await DbFieldCache.GetAsync(dbConnection, tableName, null, false);
            
            var dbFieldLookup = dbFields.ToLookup(f => f.Name.AsUnquoted(dbSetting).ToLower());
            var validSelectFields = selectFields.Where(s => dbFieldLookup[s.Name.AsUnquoted(dbSetting).ToLower()].Any());
            
            return validSelectFields;
        }
    }
}
