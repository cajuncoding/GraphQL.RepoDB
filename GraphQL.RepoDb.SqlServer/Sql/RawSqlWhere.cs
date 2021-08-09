using System;
using System.Collections.Generic;
using System.Text;

namespace GraphQL.RepoDB.Sql
{
    public class RawSqlWhere
    {
        const string WHERE_PREFIX = "WHERE ";
        public RawSqlWhere(string rawSqlWhereClause, object whereWhereParams)
        {
            if (string.IsNullOrWhiteSpace(rawSqlWhereClause))
                throw new ArgumentException("The raw sql cannot be null or whitespace.");

            var sanitizedWhereClauseSql = rawSqlWhereClause.Trim();
            if (sanitizedWhereClauseSql.StartsWith(WHERE_PREFIX, StringComparison.OrdinalIgnoreCase))
            {
                sanitizedWhereClauseSql = sanitizedWhereClauseSql.Substring(WHERE_PREFIX.Length);
            }

            RawSqlWhereClause = sanitizedWhereClauseSql;
            WhereParams = whereWhereParams;
        }
        public string RawSqlWhereClause { get; }
        public object WhereParams { get; }
    }
}
