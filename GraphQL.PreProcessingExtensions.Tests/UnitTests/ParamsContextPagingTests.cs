using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GraphQL.PreProcessingExtensions.Selections;
using HotChocolate;
using HotChocolate.PreProcessingExtensions;
using HotChocolate.PreProcessingExtensions.Selections;
using Microsoft.AspNetCore.TestHost;
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
            Assert.AreEqual(cursorPagingParams.First, 2);

            var resultsJson = (JObject)result.Data[queryKey];
            var results = resultsJson[SelectionNodeName.Nodes];

            Assert.AreEqual(results.Count(), 2);
            Assert.AreEqual(results.FirstOrDefault()?["name"], "Luke Skywalker");
            Assert.AreEqual(results.LastOrDefault()?["name"], "Darth Vader");
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
            //Assert.AreEqual(cursorPagingParams.First, 2);

            //var resultsJson = (JObject)result.Data[queryKey];
            //var results = resultsJson["nodes"];
            //Assert.AreEqual(results.Count(), 2);
            //Assert.AreEqual(results.FirstOrDefault()?["name"], "Luke Skywalker");
            //Assert.AreEqual(results.LastOrDefault()?["name"], "Darth Vader");
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
            Assert.AreEqual(offsetPagingParams.Skip, 2);
            Assert.AreEqual(offsetPagingParams.Take, 2);

            var resultsJson = (JObject)result.Data[queryKey];
            var results = resultsJson[SelectionNodeName.Items];

            Assert.AreEqual(results.Count(), 2);
            Assert.AreEqual(results.FirstOrDefault()?["name"], "Han Solo");
            Assert.AreEqual(results.LastOrDefault()?["name"], "Leia Organa");
        }
    }
}
