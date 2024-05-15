using System;

namespace RepoDb.SqlServer.PagingOperations
{
    public class RawSqlWhere
    {
        const string WHERE_PREFIX = "WHERE ";
        public RawSqlWhere(string rawSqlWhereClause, object whereParams)
        {
            if (string.IsNullOrWhiteSpace(rawSqlWhereClause))
                throw new ArgumentException("The raw sql where clause cannot be null or whitespace.");

            var sanitizedWhereClauseSql = rawSqlWhereClause.Trim();
            if (sanitizedWhereClauseSql.StartsWith(WHERE_PREFIX, StringComparison.OrdinalIgnoreCase))
            {
                sanitizedWhereClauseSql = sanitizedWhereClauseSql.Substring(WHERE_PREFIX.Length);
            }

            RawSqlWhereClause = sanitizedWhereClauseSql;
            WhereParams = whereParams;
        }

        public static RawSqlWhere From(string rawSqlWhereClause, object whereParams = null)
            => new RawSqlWhere(rawSqlWhereClause, whereParams);

        public string RawSqlWhereClause { get; }
        public object WhereParams { get; }
    }
}
