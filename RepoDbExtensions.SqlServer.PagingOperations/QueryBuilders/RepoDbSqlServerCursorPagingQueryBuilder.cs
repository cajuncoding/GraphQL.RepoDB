﻿using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using RepoDb.Interfaces;
using RepoDb.PagingPrimitives.CursorPaging;

namespace RepoDb.SqlServer.PagingOperations.QueryBuilders
{
    //TODO: Abstract this out so that DBSettings is Injected and this can support other RepoDb database targets... etc.
    internal class RepoDbCursorPagingQueryBuilder
    {
        private const string CursorIndexName = nameof(IHaveCursor.CursorIndex);

        /// <summary>
        /// BBernard
        /// Borrowed and Adapted from RepoDb source code: SqlServerStatementBuilder.CreateBatchQuery.
        /// NOTE: Due to internally only accessible elements it was easier to just construct the query as needed
        ///         to allow us to return the RowNumber as the CursorIndex!
        /// </summary>
        internal static SqlQuerySliceInfo BuildSqlServerCursorPagingQuery<TEntity>(
            string tableName,
            IEnumerable<Field> fields,
            string rawSqlStatement,
            IEnumerable<OrderField> orderBy,
            object where = null,
            string hints = null,
            int? afterCursorIndex = null,
            int? firstTake = null,
            int? beforeCursorIndex = null,
            int? lastTake = null,
            bool includeTotalCountQuery = true
        ) where TEntity : class
        {
            var dbSetting = RepoDbSettings.SqlServerSettings;

            var fieldsArray = fields?.ToArray() ?? Array.Empty<Field>();
            var orderByArray = orderBy?.ToArray() ?? Array.Empty<OrderField>();
            string primaryResultsSql;

            //NOTE: If a Raw SQL Statement is provided then we assume the author has formed it properly for the results they wish to attain
            //      with correct/valid Select Fields and Where condition as needed; no further rewriting is done.
            if (!string.IsNullOrWhiteSpace(rawSqlStatement)) primaryResultsSql = rawSqlStatement;
            else if (fieldsArray.Length > 0) primaryResultsSql = BuildSqlServerPrimaryResultsSql(tableName, fieldsArray, orderByArray, dbSetting, where, hints);
            else throw new ArgumentException("Either a Fields set or Raw Sql statement must be specified.");

            //Dynamically build/optimize the core data SQL that will be used as a CTE wrapped by the Pagination logic!
            var rowNumCteBuilder = new QueryBuilder().Clear()
                .Select()
                    .RowNumber().Over().OpenParen().OrderByFrom(orderByArray, dbSetting).CloseParen().As($"[{CursorIndexName}],")
                    .WriteText("p.*")
                .From().WriteText("PrimaryResultsCte p");

            var sqlWhereClauseSliceInfo = BuildRelaySpecQuerySliceInfo(
                cursorFieldName: $"r.{CursorIndexName}", 
                resultsCteTableName: "RowNumResultsCte",
                afterCursorIndex,
                beforeCursorIndex,
                firstTake,
                lastTake
            );

            // Build the base Paginated Query
            var sqlBuilder = new QueryBuilder().Clear()
                //.With()
                //.WriteText("CTE").As().OpenParen()
                //    //Dynamically insert the CTE that is built separately...
                //    .WriteText(sqlCte)
                //.CloseParen()
                .WriteText($@"
                    WITH PrimaryResultsCte AS (
                        {primaryResultsSql}
                    ),
                    RowNumResultsCte AS (
                        {rowNumCteBuilder.GetString()}
                    )
                    SELECT r.*
                    FROM RowNumResultsCte r
                ")
                //Implement Relay Spec Cursor Slicing Algorithm!
                .WriteText(sqlWhereClauseSliceInfo.SQL)
                .OrderByFrom(orderByArray, dbSetting)
                .End(); //Appends ';'

            if (includeTotalCountQuery)
            {
                //////Look for PKey Field to use as the Count Column... as this just makes sense...
                //////NOTE: COUNT(1) may not work as expected when column permission are in use, so we use a real field.
                //////NOTE: 
                ////var countField = PropertyCache.Get<TEntity>().FirstOrDefault(p => p.GetPrimaryAttribute() != null)?.AsField()
                ////                    ?? selectFields.FirstOrDefault();

                //Add SECOND Count Query into the Query so it can be executed as an efficient MultipleQuery!
                //NOTE: For optimization we do not need to Sort or, select more than one field to get the Total Count,
                //      the only thing that changes this result is the Where filter fields!
                //NOTE: TO FIX RISK of Null values being skipped by the Count aggregation, this is changed to use the standard COUNT(*),
                //      which is RepoDb's default behavior if field is null, to ensure that we get the true row count and nothing is
                //      eliminated due to Null values.
                //NOTE We also rely on SqlServer to optimize this query instead of trying to do too much ourselves (with other unknown risks
                //      such as column permissions, etc.
                sqlBuilder.WriteText($@"
                    WITH PrimaryResultsCte AS (
                        {primaryResultsSql}
                    )
                    SELECT COUNT(1) FROM PrimaryResultsCte
                ")
                .End();
            }

            // Build the Query and other Slice Info metadata needed for optimal pagination...
            var sqlQuery = sqlBuilder.GetString();

            var sqlQuerySliceInfo = new SqlQuerySliceInfo()
            {
                SQL = sqlQuery,
                ExpectedCount = sqlWhereClauseSliceInfo.ExpectedCount,
                IsPreviousPagePossible = sqlWhereClauseSliceInfo.IsPreviousPagePossible,
                IsNextPagePossible = sqlWhereClauseSliceInfo.IsNextPagePossible,
                IsEndIndexOverFetchedForNextPageCheck = sqlWhereClauseSliceInfo.IsEndIndexOverFetchedForNextPageCheck
            };

            // Return the query
            return sqlQuerySliceInfo;
        }

        internal static string BuildSqlServerPrimaryResultsSql(
            string tableName,
            Field[] fieldsArray,
            OrderField[] orderByArray,
            IDbSetting dbSetting,
            object where = null,
            string hints = null
        )
        {
            //Ensure that we Remove any risk of Name conflicts with the CursorIndex field on the CTE
            //  because we are dynamically adding ROW_NUMBER() as [CursorIndex]!
            //  And, ensure that there are no conflicts with the OrderBy
            var cteFields = new List<Field>(fieldsArray);
            cteFields.RemoveAll(f => f.Name.Equals(CursorIndexName, StringComparison.OrdinalIgnoreCase));

            //We must ensure that all OrderBy fields are also part of the CTE Select Clause so that they are
            //  actually available to be sorted on; or else 'Invalid Column Errors' will occur if the field is not 
            //  originally part of the Select Fields list.
            var missingSortFieldNames = orderByArray
                .Select(o => o.Name)
                .Except(cteFields.Select(f => f.Name), StringComparer.OrdinalIgnoreCase)
                .ToList();

            if (missingSortFieldNames.Count > 0)
                cteFields.AddRange(missingSortFieldNames.Select(n => new Field(n)));

            //Support either QueryGroup object model or Raw SQL; which enables support for complex Field processing & filtering not supported
            //  by QueryField/QueryGroup objects (e.g. LOWER(), TRIM()), use of Sql Server Full Text Searching on Fields (e.g. CONTAINS(), FREETEXT()), etc.
            //var sqlWhereDataFilter = whereQueryGroup?.GetString(0, dbSetting) ?? whereRawSql;
            string sqlWhereDataFilter = null;
            switch (where)
            {
                case QueryGroup whereQueryGroup:
                    sqlWhereDataFilter = whereQueryGroup.GetString(0, dbSetting);
                    break;
                case RawSqlWhere whereRawSql:
                    sqlWhereDataFilter = whereRawSql.RawSqlWhereClause;
                    break;
                case string whereRawSql:
                    sqlWhereDataFilter = whereRawSql;
                    break;
            }

            bool isWhereFilterSpecified = !string.IsNullOrWhiteSpace(sqlWhereDataFilter);

            //Dynamically build/optimize the core data SQL that will be used as a CTE wrapped by the Pagination logic!
            var primaryResultsCteBuilder = new QueryBuilder().Clear()
                .Select()
                .FieldsFrom(cteFields, dbSetting)
                .From().TableNameFrom(tableName, dbSetting)
                .HintsFrom(hints);

            if (isWhereFilterSpecified)
                primaryResultsCteBuilder.Where().WriteText(sqlWhereDataFilter);

            return primaryResultsCteBuilder.GetString();
        }

        public static void AssertCursorPagingArgsAreValid(int? after, int? before, int? first, int? last, IEnumerable<OrderField> orderBy = null)
        {
            //If both cursors are specified we can provide some validation rather than resulting in an 
            //  unexpected set of results (e.g. none).
            if (after > before)
                throw new ArgumentException($"The cursor values are invalid; the '{CursorPagingArgNames.After}' cursor " +
                    $"must occur before the '{CursorPagingArgNames.Before}' cursor argument.");

            //Implement Exception as defined by Relay Spec.
            if (first < 0)
                throw new ArgumentException(
                    $"The value for '{CursorPagingArgNames.First}' must be greater than zero.",
                    CursorPagingArgNames.First
                );

            //Implement Exception as defined by Relay Spec.
            if (last < 0)
                throw new ArgumentException(
                    $"The value for '{CursorPagingArgNames.Last}' must be greater than zero.",
                    CursorPagingArgNames.Last
                );

            if (orderBy?.Any() == false)
                throw new ArgumentException(
                    "At least one valid Order By field must be specified to ensure consistent ordering of data.",
                    nameof(orderBy)
                );
        }

        private static SqlQuerySliceInfo BuildRelaySpecQuerySliceInfo(string cursorFieldName, string resultsCteTableName, int? afterCursorIndex, int? beforeCursorIndex, int? firstTake, int? lastTake)
        {
            //Implement Cursor pagination algorithm in alignment with industry accepted Relay Spec. for GraphQL
            //For more info. see: https://relay.dev/graphql/connections.htm#sec-Pagination-algorithm
            AssertCursorPagingArgsAreValid(
                after: afterCursorIndex,
                before: beforeCursorIndex,
                first: firstTake,
                last: lastTake
            );

            string where = string.Empty;
            int? startIndex = null;
            int? endIndex = null;
            int count = 0;

            //********************************************************************************
            //FIRST we process after/before args...
            //********************************************************************************
            //NOTE: SQL Server BETWEEN operation is INCLUSIVE therefore to compute ordinal indexes from teh After/Before Cursors
            //      we increment/decrement by One (1) to always get the item actual after or actually before the cursor specified!
            if (afterCursorIndex.HasValue && beforeCursorIndex.HasValue)// Both 'after' & 'before' args
            {
                //Both Before & After are specified; This scenario is discouraged by the Spec but not prohibited!
                startIndex = (int)afterCursorIndex + 1;
                endIndex = (int)beforeCursorIndex - 1;
                count = (int)endIndex - (int)startIndex + 1;
            }
            else if (afterCursorIndex.HasValue) //Only 'after' arg
            {
                startIndex = (int)afterCursorIndex + 1;
                count = int.MaxValue;
            }
            else if (beforeCursorIndex.HasValue) //Only 'before' arg
            {
                // When taking from the End we know that the start is 1 (vice versa is not true)
                startIndex = 1;
                endIndex = (int)beforeCursorIndex - 1;
                count = (int)endIndex;
            }
            else //NEITHER After nor Before where Specified...
            {
                //Even now start from 1; with undefined End...
                startIndex = 1;
                count = int.MaxValue;
            }

            //********************************************************************************
            //SECOND we process first/last args which may applied in combination...
            //********************************************************************************
            //NOTE: This may be the first time endIndex is initialized, based on FIRST filter step above...
            if (count > firstTake)
            {
                endIndex = startIndex + (int)firstTake - 1;
                count = (int)firstTake;
            }

            //Last can be combined in addition to First above (not exclusive)
            //NOTE: We only allow the Last Filter Here if there are enough items expected (expected count)
            //      to be available; meaning the expected count from the FIRST filter step above has to
            //      be greater than the take from the end (as outlined in Relay Spec).
            if (count > lastTake)
            {
                if (endIndex.HasValue)
                {
                    startIndex = (int)endIndex - (int)lastTake + 1;
                }
                else
                {
                    //This is the Case where We are requested to select from the End of all results
                    //  dynamically because no Before Cursor (ending cursor) was provided...
                    //Therefore, this requires a special Query condition to Dynamically compute 
                    //  the currently unknown StartIndex...
                    startIndex = null;
                    endIndex = null;
                }

                //In both of these cases teh total count expected will be the Last Take size
                //  since it's smaller than the original count expected from the FIRST filter step above...
                count = (int)lastTake;
            }

            var sqlSliceInfo = new SqlQuerySliceInfo() { ExpectedCount = count };

            //Finally we can convert the Mapped Start & End indexes to valid Where Clause...
            if (startIndex.HasValue && endIndex.HasValue)
            {
                //NOTE: We INCREMENT End Index by one here to intentionally Over Fetch so that we can optimize
                //      the computation of HasNextPage without knowing the Full table Count!
                sqlSliceInfo.SQL = $"WHERE ({cursorFieldName} BETWEEN {startIndex} AND {endIndex + 1})";
                sqlSliceInfo.IsPreviousPagePossible = true;
                sqlSliceInfo.IsNextPagePossible = true;
                sqlSliceInfo.IsEndIndexOverFetchedForNextPageCheck = true;
            }
            else if (startIndex.HasValue)
            {
                sqlSliceInfo.SQL = $"WHERE ({cursorFieldName} >= {startIndex})";
                sqlSliceInfo.IsPreviousPagePossible = true;
                sqlSliceInfo.IsNextPagePossible = false;
                sqlSliceInfo.IsEndIndexOverFetchedForNextPageCheck = false;
            }
            //NOTE: Is Always False as noted by Re-sharper; in all cases where EndIndex has a value so will StartIndex also have a value hitting the first case above!
            //  But algorithmically it feels right to just leave this since we may optimize code above and risk changing this in the future
            else if (endIndex.HasValue)
            {
                //NOTE: We INCREMENT End Index by one here to intentionally Over Fetch so that we can optimize
                //      the computation of HasNextPage without knowing the Full table Count!
                sqlSliceInfo.SQL = $"WHERE ({cursorFieldName} <= {endIndex + 1})";
                sqlSliceInfo.IsPreviousPagePossible = false;
                sqlSliceInfo.IsNextPagePossible = true;
                sqlSliceInfo.IsEndIndexOverFetchedForNextPageCheck = true;
            }
            //Handle Unique Case for when 'Last' was provided without any 'Before' cursor!
            //NOTE: Is Always True (if we get here) as note by Re-sharper; because we intentionally null out StartIndex for this case to hit
            //      when LastTake is specified without an AFTER or BEFORE!
            //      But algorithmically it feels right to just leave this since we may optimize code above and risk changing this in the future
            else if (lastTake.HasValue)
            {
                //Computing from the End is the most complex so some performance hit is likely acceptable since it's a less used use-case.
                //NOTE: This must use the Dynamic Count() to determine the End of the existing result set...
                sqlSliceInfo.SQL = $"WHERE ({cursorFieldName} > (SELECT (COUNT({cursorFieldName}) - {(int)lastTake}) FROM {resultsCteTableName}))";
                sqlSliceInfo.IsPreviousPagePossible = true;
                sqlSliceInfo.IsNextPagePossible = false;
                sqlSliceInfo.IsEndIndexOverFetchedForNextPageCheck = false;
            }

            return sqlSliceInfo;
        }
    }

    internal class SqlQuerySliceInfo
    {
        // ReSharper disable once InconsistentNaming
        public string SQL { get; set; }
        public bool IsEndIndexOverFetchedForNextPageCheck { get; set; }
        public int ExpectedCount { get; set; }
        public bool IsPreviousPagePossible { get; set; }
        public bool IsNextPagePossible { get; set; }
    }
}
