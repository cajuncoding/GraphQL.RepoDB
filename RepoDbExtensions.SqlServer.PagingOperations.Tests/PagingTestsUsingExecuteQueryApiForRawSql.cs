using System;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.PagingPrimitives.CursorPaging;
using RepoDb.PagingPrimitives.OffsetPaging;
using StarWars.Characters.DbModels;


namespace RepoDb.SqlServer.PagingOperations.Tests
{
    [TestClass]
    public class PagingTestsUsingExecuteQueryApiForRawSql : BaseTest
    {
        [TestMethod]
        public async Task TestCursorPagingWithRawSqlAsync()
        {
            using var sqlConnection = await CreateSqlConnectionAsync().ConfigureAwait(false);

            const int pageSize = 2;
            int? totalCount = null;
            int runningTotal = 0;
            ICursorPageResults<CharacterDbModel> page = null;

            do
            {
                page = await sqlConnection.ExecutePagingCursorQueryAsync<CharacterDbModel>(
                    //TEST Formatted SQL with line breaks, ending semi-colon, etc....
                    commandText: @"
                        SELECT * 
                        FROM [dbo].[StarWarsCharacters] c;
                    ",
                    new [] {OrderField.Descending<CharacterDbModel>(c => c.Id) },
                    first: pageSize, 
                    afterCursor: page?.EndCursor,
                    retrieveTotalCount: totalCount is null
                );

                page.Should().NotBeNull();

                var resultsList = page.CursorResults.ToList();
                resultsList.Should().HaveCount(pageSize);

                //Validate that we get Total Count only once, and on all following pages it is skipped and Null is returned as expected!
                if (totalCount is null)
                {
                    page.TotalCount.Should().BePositive();
                    totalCount = page.TotalCount;
                    TestContext.WriteLine("*********************************************************");
                    TestContext.WriteLine($"[{totalCount}] Total Results to be processed...");
                    TestContext.WriteLine("*********************************************************");
                }
                else
                {
                    page.TotalCount.Should().BeNull();
                }
                    
                runningTotal += resultsList.Count;

                TestContext.WriteLine("");
                TestContext.WriteLine($"[{resultsList.Count}] Page Results:");
                TestContext.WriteLine("----------------------------------------");
                foreach (var result in resultsList)
                {
                    var entity = result.Entity;
                    TestContext.WriteLine($"[{result.Cursor}] ==> ({entity.Id}) {entity.Name}");
                    AssertCharacterDbModelIsValid(entity);
                }

            } while (page.HasNextPage);

            Assert.AreEqual(totalCount, runningTotal, "Total Count doesn't Match the final running total tally!");
        }

        [TestMethod]
        public async Task TestCursorPagingWithRawSqlWhereClauseAndNullPagingParamValuesAsync()
        {
            using var sqlConnection = await CreateSqlConnectionAsync().ConfigureAwait(false);

            var namePattern = "%Luke%";

            ICursorPageResults<CharacterDbModel> page = await sqlConnection.ExecutePagingCursorQueryAsync<CharacterDbModel>(
                //TEST Formatted SQL with line breaks, ending semi-colon, etc....
                commandText: @"
                    SELECT * 
                    FROM [dbo].[StarWarsCharacters] c
                    WHERE c.[Name] LIKE @NamePattern;
                ",
                new[] { OrderField.Descending<CharacterDbModel>(c => c.Id) },
                //TEST PASSING IN Empty Paging Params (all values being NULL)....
                pagingParams: CursorPagingParams.ForCursors(null, null, null, null),
                sqlParams: new
                {
                    NamePattern = namePattern
                }
            );

            page.Should().NotBeNull();
            page.TotalCount.Should().BeNull();
            page.CursorResults.Should().HaveCount(1);

            TestContext.WriteLine("NO Cursor values provided so ALL results are expected...");
            TestContext.WriteLine($"[{namePattern}] WHERE Filter Applied to Limit Primary Result Set...");
            TestContext.WriteLine($"[{page.PageCount}] Page Results Returned...");
        }

        [TestMethod]
        public async Task TestCursorPagingWithRawSqlWhereClauseToGetTotalCountButNoResults()
        {
            using var sqlConnection = await CreateSqlConnectionAsync().ConfigureAwait(false);

            ICursorPageResults<CharacterDbModel> page = await sqlConnection.ExecutePagingCursorQueryAsync<CharacterDbModel>(
                //TEST Formatted SQL with line breaks, ending semi-colon, etc....
                commandText: @"
                    SELECT * 
                    FROM [dbo].[StarWarsCharacters] c
                    WHERE c.[Name] LIKE @NamePattern;
                ",
                new[] { OrderField.Descending<CharacterDbModel>(c => c.Id) },
                //TEST PASSING IN Empty Paging Params (all values being NULL)....
                pagingParams: CursorPagingParams.ForCursors(first: 0, retrieveTotalCount: true),
                sqlParams: new
                {
                    NamePattern = "%Luke%"
                }
            );

            page.Should().NotBeNull();
            page.TotalCount.Should().Be(1);//TOTAL Count == 1
            page.CursorResults.Should().HaveCount(0);//NO RESULTS REQUESTED!

            TestContext.WriteLine($"[{page.TotalCount}] Total Results...");
            TestContext.WriteLine($"[{page.PageCount}] Page Results Returned...");
        }

        [TestMethod]
        public async Task TestOffsetPagingWithRawSqlAsync()
        {
            using var sqlConnection = await CreateSqlConnectionAsync().ConfigureAwait(false);

            const int pageSize = 2;
            int? totalCount = null;
            int runningTotal = 0;
            IOffsetPageResults<CharacterDbModel> page = null;

            do
            {
                page = await sqlConnection.ExecutePagingOffsetQueryAsync<CharacterDbModel>(
                    "SELECT * FROM [dbo].[StarWarsCharacters]",
                    new[] { OrderField.Descending<CharacterDbModel>(c => c.Id) },
                    skip: page?.EndIndex,
                    take: pageSize,
                    retrieveTotalCount: totalCount is null
                );

                page.Should().NotBeNull();

                var resultsList = page.Results.ToList();
                resultsList.Should().HaveCount(pageSize);

                //Validate that we get Total Count only once, and on all following pages it is skipped and Null is returned as expected!
                if (totalCount is null)
                {
                    page.TotalCount.Should().BePositive();
                    totalCount = page.TotalCount;
                    TestContext.WriteLine("*********************************************************");
                    TestContext.WriteLine($"[{totalCount}] Total Results to be processed...");
                    TestContext.WriteLine("*********************************************************");
                }
                else
                {
                    page.TotalCount.Should().BeNull();
                }

                runningTotal += resultsList.Count;

                TestContext.WriteLine("");
                TestContext.WriteLine($"[{resultsList.Count}] Page Results:");
                TestContext.WriteLine("----------------------------------------");
                int counter = 0;
                foreach (var entity in resultsList)
                {
                    TestContext.WriteLine($"[{++counter}] ==> ({entity.Id}) {entity.Name}");
                    AssertCharacterDbModelIsValid(entity);
                }

            } while (page.HasNextPage);

            Assert.AreEqual(totalCount, runningTotal, "Total Count doesn't Match the final running total tally!");
        }

        private void AssertCharacterDbModelIsValid(CharacterDbModel entity)
        {
            entity.Id.Should().BePositive();
            entity.Name.Should().NotBeNullOrWhiteSpace();
            
            if(entity.IsHuman)
                if (new[] { "Luke", "Darth" }.Any(n => entity.Name.IndexOf(n, StringComparison.OrdinalIgnoreCase) >= 0))
                    entity.HomePlanet.Should().BeEquivalentTo("Tatooine");
                else if(entity.Name.IndexOf("Leia", StringComparison.OrdinalIgnoreCase) >= 0)
                    entity.HomePlanet.Should().BeEquivalentTo("Alderaan");
                else
                    entity.HomePlanet.Should().BeNull();

            if (entity.IsDroid)
                entity.PrimaryFunction.Should().BeEquivalentTo(
                    entity.Name.IndexOf("C-3PO", StringComparison.OrdinalIgnoreCase) >= 0
                        ? "Protocol"
                        : "Astromech"
                );


        }
    }
}