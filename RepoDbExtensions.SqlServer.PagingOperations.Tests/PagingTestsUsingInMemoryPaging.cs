using System;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Data.SqlClient;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.PagingPrimitives.CursorPaging;
using RepoDb.PagingPrimitives.OffsetPaging;
using RepoDb.SqlServer.PagingOperations.InMemoryPaging;
using StarWars.Characters.DbModels;


namespace RepoDb.SqlServer.PagingOperations.Tests
{
    [TestClass]
    public class PagingTestsUsingInMemoryPaging : BaseTest
    {
        [TestMethod]
        public void TestCursorPagingWithInMemorySlicing()
        {
            var expectedTotalCount = 100;
            var items = Enumerable.Range(1, expectedTotalCount).ToArray();

            const int pageSize = 10;
            int? totalCount = null;
            int runningTotal = 0;
            ICursorPageResults<int> page = null;
            var pageCounter = 0;

            do
            {
                page = items.SliceAsCursorPage(
                    first: pageSize, 
                    after: page?.EndCursor, 
                    before: null, 
                    last: null,
                    includeTotalCount: totalCount is null
                );

                pageCounter++;
                page.Should().NotBeNull();

                if (pageCounter == 1)
                {
                    page.HasNextPage.Should().BeTrue();
                    page.HasPreviousPage.Should().BeFalse();
                }
                else if (pageCounter == (int)Math.Ceiling((decimal)expectedTotalCount / pageSize))
                {
                    page.HasNextPage.Should().BeFalse();
                    page.HasPreviousPage.Should().BeTrue();
                }
                else
                {
                    page.HasNextPage.Should().BeTrue();
                    page.HasPreviousPage.Should().BeTrue();
                }

                var resultsList = page.CursorResults.ToList();
                resultsList.Should().HaveCount(pageSize);

                //Validate that we get Total Count only once, and on all following pages it is skipped and Null is returned as expected!
                if (totalCount is null)
                {
                    page.TotalCount.Should().Be(expectedTotalCount);
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
                    var value = result.Entity;
                    value.Should().BeInRange(1, expectedTotalCount);
                    TestContext.WriteLine($"[{result.Cursor}] ==> ({value})");
                }

            } while (page.HasNextPage);

            Assert.AreEqual(totalCount, runningTotal, "Total Count doesn't Match the final running total tally!");
        }

        [TestMethod]
        public void TestOffsetPagingWithInMemorySlicing()
        {
            var expectedTotalCount = 100;
            var items = Enumerable.Range(1, expectedTotalCount).ToArray();

            const int pageSize = 10;
            int? totalCount = null;
            int runningTotal = 0;
            IOffsetPageResults<int> page = null;
            var pageCounter = 0;

            do
            {
                page = items.SliceAsOffsetPage(
                    skip: page?.EndIndex,
                    take: pageSize,
                    includeTotalCount: totalCount is null
                );
                    
                pageCounter++;
                page.Should().NotBeNull();

                if (pageCounter == 1)
                {
                    page.HasNextPage.Should().BeTrue();
                    page.HasPreviousPage.Should().BeFalse();
                }
                else if (pageCounter == (int)Math.Ceiling((decimal)expectedTotalCount / pageSize))
                {
                    page.HasNextPage.Should().BeFalse();
                    page.HasPreviousPage.Should().BeTrue();
                }
                else
                {
                    page.HasNextPage.Should().BeTrue();
                    page.HasPreviousPage.Should().BeTrue();
                }

                var resultsList = page.Results.ToList();
                resultsList.Should().HaveCount(pageSize);

                //Validate that we get Total Count only once, and on all following pages it is skipped and Null is returned as expected!
                if (totalCount is null)
                {
                    page.TotalCount.Should().Be(expectedTotalCount);
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
                    result.Should().BeInRange(1, expectedTotalCount);
                    TestContext.WriteLine($"[{result}]");
                }

            } while (page.HasNextPage);

            Assert.AreEqual(totalCount, runningTotal, "Total Count doesn't Match the final running total tally!");
        }
    }
}