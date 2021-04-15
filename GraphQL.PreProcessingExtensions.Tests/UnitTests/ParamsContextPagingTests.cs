using System;
using System.Linq;
using System.Threading.Tasks;
using GraphQL.PreProcessingExtensions.Selections;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;

namespace HotChocolate.PreProcessingExtensions.Tests
{
    [TestClass]
    public class ParamsContextPagingTests : GraphQLTestBase
    {
        public ParamsContextPagingTests() : base(new GraphQLTestServerFactory())
        {
        }

        [TestMethod]
        public async Task TestParamsContextCursorPagingOnlyFirst()
        {
            // arrange
            var server = CreateStarWarsTestServer();

            // act
            var result = await server.PostQueryAsync(@"{
                starWarsCharactersCursorPaginated(first:2) {
                    nodes {
                        id
                        name
                    }
                }
            }");

            // assert
            Assert.IsNotNull(result?.Data, "Query Execution Failed");

            var queryKey = "starWarsCharactersCursorPaginated";
            var paramsContext = server.GetParamsContext(queryKey);
            var cursorPagingParams = paramsContext.CursorPagingArgs;

            Assert.IsNotNull(cursorPagingParams);
            Assert.AreEqual(2, cursorPagingParams.First);

            var resultsJson = (JObject)result.Data[queryKey];
            var results = resultsJson[SelectionNodeName.Nodes];

            Assert.IsNotNull(results);
            Assert.AreEqual(2, results.Count());
            Assert.AreEqual("Luke Skywalker", results.FirstOrDefault()?["name"]);
            Assert.AreEqual("Darth Vader", results.LastOrDefault()?["name"]);
        }

        //TODO: Implement a few more tests...
        //[TestMethod]
        //public async Task TestParamsContextCursorPagingMiddleSlice()
        //{
        //// arrange
        //var server = CreateStarWarsTestServer();

        //// act
        //var result = await server.PostQueryAsync(@"{
        //    starWarsCharactersCursorPaginated(first:2) {
        //        nodes {
        //            id
        //            name
        //        }
        //    }
        //}");

        //// assert
        //Assert.IsNotNull(result?.Data, "Query Execution Failed");

        //var queryKey = "starWarsCharactersCursorPaginated";
        //var paramsContext = server.GetParamsContext(queryKey);
        //var cursorPagingParams = paramsContext.CursorPagingArgs;
        //Assert.IsNotNull(cursorPagingParams);
        //Assert.AreEqual(2, cursorPagingParams.First);

        //var resultsJson = (JObject)result.Data[queryKey];
        //var results = resultsJson[SelectionNodeName.Nodes];
        //Assert.AreEqual(2, results.Count());
        //Assert.AreEqual("Luke Skywalker", results.FirstOrDefault()?["name"]);
        //Assert.AreEqual("Darth Vader", results.LastOrDefault()?["name"]);
        //}

        [TestMethod]
        public async Task TestParamsContextOffsetPaging()
        {
            // arrange
            var server = CreateStarWarsTestServer();

            // act
            var result = await server.PostQueryAsync(@"{
                starWarsCharactersOffsetPaginated(skip:2, take:2) {
                    items {
                        id
                        name
                    }
                }
            }");

            // assert
            Assert.IsNotNull(result?.Data, "Query Execution Failed");

            var queryKey = "starWarsCharactersOffsetPaginated";
            var paramsContext = server.GetParamsContext(queryKey);
            var offsetPagingParams = paramsContext.OffsetPagingArgs;

            Assert.IsNotNull(offsetPagingParams);
            Assert.AreEqual(2, offsetPagingParams.Skip);
            Assert.AreEqual(2, offsetPagingParams.Take);

            var resultsJson = (JObject)result.Data[queryKey];
            var results = resultsJson[SelectionNodeName.Items];

            Assert.IsNotNull(results);
            Assert.AreEqual(2, results.Count());
            Assert.AreEqual("Han Solo", results.FirstOrDefault()?["name"]);
            Assert.AreEqual("Leia Organa", results.LastOrDefault()?["name"]);
        }
    }
}
