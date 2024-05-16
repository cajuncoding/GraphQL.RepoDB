using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using StarWars.Characters.DbModels;


namespace RepoDb.SqlServer.PagingOperations.Tests
{
    [TestClass]
    public class RawSqlPagingTests : BaseTest
    {
        [TestMethod]
        public async Task TestCursorPagingWithRawSql()
        {
            using (var sqlConnection = await CreateSqlConnectionAsync().ConfigureAwait(false))
            {
                const int count = 2;

                var pageResults = await sqlConnection.ExecutePagingCursorQueryAsync<CharacterDbModel>(
                    "SELECT [Id], [Name] FROM [dbo].[StarWarsCharacters]",
                    new [] {OrderField.Descending<CharacterDbModel>(c => c.Id) },
                    pagingParams: RepoDbCursorPagingParams.ForCursors(first: count)
                );


                pageResults.Should().NotBeNull();

                var resultsList = pageResults.CursorResults.ToList();
                resultsList.Should().HaveCount(count);

                TestContext.WriteLine($"[{resultsList.Count}] Results:");
                TestContext.WriteLine("----------------------------------------");
                foreach (var result in resultsList)
                {
                    var entity = result.Entity;
                    entity.Id.Should().BePositive();
                    entity.Name.Should().NotBeNullOrWhiteSpace();
                    
                    TestContext.WriteLine($"[{result.Cursor}] ==> ({entity.Id}) {entity.Name}");
                }
            }
        }
    }
}