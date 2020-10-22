using RepoDb;
using RepoDb.CustomExtensions;
using System;
using System.Collections.Generic;
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
            int afterCursor = 0,
            int firstTake = 0,
            int beforeCursor = 0,
            int lastTake = 0,
            bool includeTotalCountQuery = true
        )
            where TEntity : class
        {
            int page = 1, rowsPerBatch = 0;

            //TODO Improve this Algorithm to support BOTH, but for now we support only First+After, or Last+Before!
            if (firstTake > 0)
            {
                //Rows per batch is the same as the Take specified!
                rowsPerBatch = firstTake;
                //Derive a Page from the Cursors existing Position using zero based Page value and zero based starting
                //  cursor value (the first item is #1 after the cursor starting postion of 0) - no shift is needed.
                page = (afterCursor / firstTake);
            }
            else //LastTake must be > 0 asl already validated above...
            {
                //Rows per batch is the same as the Take specified!
                rowsPerBatch = lastTake;
                //Derive a Page from the Cursors existing Position using zero based Page value (the first item #1
                // is the one before the specified cursor) so we need to truncate the division result and then subtract 1.
                page = ((int)beforeCursor / lastTake) - 1;
            }

            // Initialize the builder
            var builder = new QueryBuilder();

            var dbSetting = RepoDbSettings.SqlServerSettings;
            string cursorIndexName = nameof(IHaveCursor.CursorIndex);

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
                //TODO:  Update to Support First+After & Last+Before at the same time...
                .WriteText(string.Concat($"WHERE ([{cursorIndexName}] BETWEEN ", (page * rowsPerBatch) + 1, " AND ", (page + 1) * rowsPerBatch, ")"))
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

    }
}
