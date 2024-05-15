using System.Collections.Generic;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;


namespace RepoDb.SqlServer.PagingOperations.Tests
{
    [TestClass]
    public class RepoDbCursorHelperTests : BaseTest
    {
        [TestMethod]
        public void TestCursorCreationAndParsing()
        {
            var testIndexes = new[] { 4, 16, 744, 7422999, int.MaxValue };
            var createdCursors = new List<string>();

            TestContext.WriteLine("Creating Cursors...");
            foreach (var index in testIndexes)
            {
                var cursor = RepoDbCursorHelper.CreateCursor(index);
                TestContext.WriteLine($"[{index}] ==> [{cursor}]");

                //Assert our results are valid...
                cursor.Should().NotBeNullOrWhiteSpace();
                
                createdCursors.Add(cursor);
            }

            TestContext.WriteLine("Parsing Cursors...");
            var i = 0;
            foreach (var cursor in createdCursors)
            {
                var cursorIndex = RepoDbCursorHelper.ParseCursor(cursor);
                TestContext.WriteLine($"[{cursor}] ==> [{cursorIndex}]");

                //Assert our results are valid...
                cursorIndex.Should().BePositive();
                cursorIndex.Should().Be(testIndexes[i++]);
            }
        }
    }
}