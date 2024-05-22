using System;
using System.Text.RegularExpressions;

namespace RepoDb.SqlServer.PagingOperations
{
    public class RawSql
    {
        private static readonly Regex SelectPrefixValidationRegex = new Regex(@"^\s*SELECT\s+", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        private static readonly Regex OrderByValidationRegex = new Regex(@"\s+ORDER BY\s+", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        
        public RawSql(string rawSql, object sqlParams)
        {
            var sanitizedRawSql = rawSql.Trim();

            if (string.IsNullOrWhiteSpace(sanitizedRawSql))
                throw new ArgumentException("The raw sql select statement cannot be null or whitespace.");

            if (!SelectPrefixValidationRegex.IsMatch(sanitizedRawSql))
                throw new ArgumentException("The raw sql select statement provided does not appear to be a valid simple SELECT statement.");

            if (OrderByValidationRegex.IsMatch(sanitizedRawSql))
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
