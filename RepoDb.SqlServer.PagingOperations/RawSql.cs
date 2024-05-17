using System;

namespace RepoDb.SqlServer.PagingOperations
{
    public class RawSql
    {
        const string SELECT_PREFIX = "SELECT ";
        const string ORDER_BY_CLAUSE = "ORDER BY";
        public RawSql(string rawSql, object sqlParams)
        {
            var sanitizedRawSql = rawSql.Trim();

            if (string.IsNullOrWhiteSpace(sanitizedRawSql))
                throw new ArgumentException("The raw sql select statement cannot be null or whitespace.");

            if (!sanitizedRawSql.StartsWith(SELECT_PREFIX, StringComparison.OrdinalIgnoreCase))
                throw new ArgumentException("The raw sql select statement provided does not appear to be a valid simple SELECT statement.");

            if (sanitizedRawSql.IndexOf(ORDER_BY_CLAUSE, StringComparison.OrdinalIgnoreCase) >= 0)
                throw new ArgumentException("The raw sql select statement cannot contains an Order By clause; Order By must be specified using the API for proper Pagination.");

            RawSqlStatement = sanitizedRawSql;
            SqlParams = sqlParams;
        }

        public static RawSql From(string rawSql, object sqlParams = null)
            => new RawSql(rawSql, sqlParams);

        public string RawSqlStatement { get; }
        public object SqlParams { get; }
    }
}
