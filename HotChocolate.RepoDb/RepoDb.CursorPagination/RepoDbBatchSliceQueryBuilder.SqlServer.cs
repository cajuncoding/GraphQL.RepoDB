using HotChocolate.PreProcessedExtensions.Pagination;
using RepoDb;
using RepoDb.CustomExtensions;
using RepoDb.Enumerations;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace RepoDb.CursorPagination
{
    //TODO: Abstract this out so that DBSettings is Injected and this can support all RepoDb database targets... etc.
    public class RepoDbCursorPagingQueryBuilder
    {
        /// <summary>
        /// BBernard
        /// Borrowed and Adapted from RepoDb source code: SqlServerStatementBuilder.CreateBatchQuery.
        /// NOTE: Due to internally only accessible elements it was easier to just construct the query as needed
        ///         to allow us to return the RowNumber as the CursorIndex!
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="fields"></param>
        /// <param name="page"></param>
        /// <param name="rowsPerBatch"></param>
        /// <param name="orderBy"></param>
        /// <param name="where"></param>
        /// <param name="hints"></param>
        /// <returns></returns>
        public static string BuildSqlServerBatchSliceQuery<TEntity>(
            string tableName,
            IEnumerable<Field> fields,
            IEnumerable<OrderField> orderBy,
            QueryGroup whereGroup = null,
            string hints = null,
            int? afterCursorIndex = null,
            int? firstTake = null,
            int? beforeCursorIndex = null,
            int? lastTake = null,
            bool includeTotalCountQuery = true
        )
            where TEntity : class
        {
            var dbSetting = RepoDbSettings.SqlServerSettings;
            string cursorIndexName = nameof(IHaveCursor.CursorIndex);
            
            // Initialize the builder
            var builder = new QueryBuilder();

            //Ensure that we Remove any risk of Name conflicts with the CursorIndex field on the CTE
            //  because we are dynamically adding ROW_NUMBER() as [CursorInex]!
            var cteFields = fields
                .Where(f => !f.Name.Equals(cursorIndexName, StringComparison.OrdinalIgnoreCase))
                .ToList();

            var selectFields = new List<Field>();
            selectFields.AddRange(fields);
            selectFields.Add(new Field(cursorIndexName));

            // Build the base Paginated Query
            builder.Clear()
                .With()
                .WriteText("CTE").As().OpenParen()
                    .Select()
                        .RowNumber().Over().OpenParen().OrderByFrom(orderBy, dbSetting).CloseParen().As($"[{cursorIndexName}],")
                        .FieldsFrom(cteFields, dbSetting)
                    .From().TableNameFrom(tableName, dbSetting)
                    .HintsFrom(hints)
                //TODO: NOT IMPLEMENTED YET due to the utilties to easily map the QueryGroup to a query param object 
                //  being 'internal' scoped; that we will need to access....
                //.WhereFrom(whereGroup, dbSetting)
                .CloseParen()
                .Select()
                    .FieldsFrom(selectFields, dbSetting)
                .From().WriteText("CTE")
                //Implement Relay Spec Cursor Slicing Algorithm!
                .WriteText(BuildRelaySpecWhereCondition(cursorIndexName, afterCursorIndex, beforeCursorIndex, firstTake, lastTake))
                .OrderByFrom(orderBy, dbSetting)
                .End();

            if (includeTotalCountQuery)
            {

                //Look for PKey Field to use as the Count Colunm... as this just makes sense...
                //NOTE: COUNT(1) may not work as expected when column permission are in use, so we use a real field.
                var countField = PropertyCache.Get<TEntity>().FirstOrDefault(p => p.GetPrimaryAttribute() != null)?.AsField()
                                    ?? selectFields.FirstOrDefault();

                //Add SECOND Count Query into the Query so it can be executed as an efficient MultipleQuery!
                //NOTE: For optimization we do not need to Sort or, select more than one field to get the Total Count,
                //      the only thing that changes this result is the Where filter fields!
                builder.Select()
                    .Count(countField, dbSetting)
                    .From().TableNameFrom(tableName, dbSetting)
                    .HintsFrom(hints)
                    .WhereFrom(whereGroup, dbSetting)
                .End();
            }

            // Return the query
            return builder.GetString();
        }

        public static void AsserteCursorPagingArgsAreValid(
            int? after, int? before, int? first, int? last,
            IEnumerable<OrderField> orderBy = null
        )
        {
            //If both cursors are specified we can provide some validation rather than resulting in an 
            //  unexpected set of results (e.g. none).
            if (after.HasValue && before.HasValue && after > before)
            {
                throw new ArgumentException($"The cursor values are invalid; the '{CursorPagingArgNames.AfterDescription}' cursor " +
                    $"must occur before the '{CursorPagingArgNames.BeforeDescription}' cursor argument.");
            }

            //Implement Exception as defined by Relay Spec.
            if (first.HasValue && first < 0)
            {
                throw new ArgumentException(
                    $"The value for '{CursorPagingArgNames.FirstDescription}' must be greater than zero.",
                    CursorPagingArgNames.FirstDescription
                );
            }

            //Implement Exception as defined by Relay Spec.
            if (last.HasValue && last < 0)
            {
                throw new ArgumentException(
                    $"The value for '{CursorPagingArgNames.LastDescription}' must be greater than zero.",
                    CursorPagingArgNames.LastDescription
                );
            }

            if (orderBy?.Count() <= 0)
                throw new ArgumentException(
                    "A valid Order By field must be specified to ensure consistent ordering of data.",
                    nameof(orderBy)
                );
        }

        [SuppressMessage("Style", "IDE0059:Unnecessary assignment of a value", Justification = "<Pending>")]
        private static string BuildRelaySpecWhereCondition(string cursorFieldName, int? afterCursorIndex, int? beforeCursorIndex, int? firstTake, int? lastTake)
        {
            //Implement Cursor pagination algorithm in alightment with industry accepted Relay Spec. for GraphQL
            //For more info. see: https://relay.dev/graphql/connections.htm#sec-Pagination-algorithm
            AsserteCursorPagingArgsAreValid(
                after: afterCursorIndex,
                before: beforeCursorIndex,
                first: firstTake,
                last: lastTake
            );

            string where = string.Empty;
            int? startIndex = null;
            int? endIndex = null;
            int count = 0;

            //FIRST we process after/before args...
            if (afterCursorIndex.HasValue && beforeCursorIndex.HasValue)// Both 'after' & 'before' args
            {
                //Both Before & After are specified; This scenario is discouraged by the Spec but not prohibited!
                startIndex = ((int)afterCursorIndex + 1);
                endIndex = ((int)beforeCursorIndex - 1);
                count = ((int)endIndex - (int)startIndex) + 1;
            }
            else if (afterCursorIndex.HasValue) //Only 'after' arg
            {
                startIndex = ((int)afterCursorIndex + 1);
                count = int.MaxValue;
            }
            else if (beforeCursorIndex.HasValue) //Only 'before' arg
            {
                // When taking from the End we know that the start is 1 (vice versa is not true)
                startIndex = 1;
                endIndex = ((int)beforeCursorIndex - 1);
                count = (int)endIndex;
            }
            else //NEITHER After or Before where Specified...
            {
                //Even now start from 1; with undefined End...
                startIndex = 1;
                count = int.MaxValue;
            }

            //SECOND we process first/last args which may apply combinatorially...
            //  For example this may be the first time endIndex is intialized, based on FIRST process above.
            if (firstTake.HasValue && count > firstTake && startIndex.HasValue)
            {
                endIndex = (startIndex + (int)firstTake) - 1;
                count = (int)firstTake;
            }

            //Last can be combined in addition to First above (not exclusive)
            if (lastTake.HasValue && count > lastTake)
            {
                if (endIndex.HasValue)
                {
                    startIndex = ((int)endIndex - (int)lastTake) + 1;
                    //count = (int)lastTake;
                }
                else
                {
                    //This is the Case where We are requested to select from the End of all results
                    //  dynamically because no Before Cursor (ending cursor) was provided...
                    //Therefore this requires a special Query condition to Dynamically compute 
                    //  the currently uknown StartIndex...
                    startIndex = null;
                    endIndex = null;
                }
            }

            //Finally we can conver the Mapped Start & End indexes to valid Where Clause...
            if (startIndex.HasValue && endIndex.HasValue)
            {
                where = $"WHERE ([{cursorFieldName}] BETWEEN {startIndex} AND {endIndex})";
            }
            else if (startIndex.HasValue)
            {
                where = $"WHERE ([{cursorFieldName}] >= {startIndex})";
            }
            else if (endIndex.HasValue)
            {
                where = $"WHERE ([{cursorFieldName}] <= {endIndex})";
            }
            //Handle Unique Case of ONLY 'Last' was provided without any 'Before' cursor!
            else if (lastTake.HasValue)
            {
                //Computing from the End is the most complex so some performance hit is likely acceptable 
                //  since it's a less used use-case.
                //NOTE: This must use the Dynamic Count() to determine the End of the existing result set...
                where = $"WHERE ([{cursorFieldName}] > (SELECT (COUNT([{cursorFieldName}]) - {(int)lastTake}) FROM CTE))";
            }

            return where;
        }
    }
}
