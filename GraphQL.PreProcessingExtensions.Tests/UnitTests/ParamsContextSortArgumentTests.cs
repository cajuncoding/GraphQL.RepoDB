using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HotChocolate;
using HotChocolate.PreProcessingExtensions;
using Microsoft.AspNetCore.TestHost;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HotChocolate.PreProcessingExtensions.Tests
{
    [TestClass]
    public class ParamsContextSortArgumentTests : GraphQLTestBase
    {
        public ParamsContextSortArgumentTests() : base(new GraphQLTestServerFactory())
        {
        }

        [TestMethod]
        public async Task TestParamsContextMiddlewareInjectionSimpleExample()
        {
            // arrange
            var server = CreateHelloWorldTestServer();

            // act
            var result = await server.PostQueryAsync("{ hello }");

            // assert
            Assert.IsNotNull(result?.Data, "Query Execution Failed");
            Assert.AreEqual(1, server.ParamsContextList.Count);
        }

        [TestMethod]
        public async Task TestParamsContextSortOrderWhenNotEnabled()
        {
            // arrange
            var server = CreateHelloWorldTestServer();

            // act
            var result = await server.PostQueryAsync(@"{ hello }");

            // assert
            Assert.IsNotNull(result?.Data, "Query Execution Failed");

            var paramsContext = server.GetParamsContext("hello");
            Assert.IsNotNull(paramsContext);
            Assert.IsNotNull(paramsContext.SortArgs);
            Assert.IsFalse(paramsContext.SortArgs.Any());
        }

        [TestMethod]
        public async Task TestParamsContextSortOrderWhenNotSpecified()
        {
            // arrange
            var server = CreateStarWarsTestServer();

            // act
            var result = await server.PostQueryAsync(@"{
                starWarsCharacters {
                    id
                    name
                }
            }");

            // assert
            Assert.IsNotNull(result?.Data, "Query Execution Failed");

            var paramsContext = server.GetParamsContext("starWarsCharacters");
            Assert.IsNotNull(paramsContext);
            Assert.IsNotNull(paramsContext.SortArgs);
            Assert.IsFalse(paramsContext.SortArgs.Any());
        }

        [TestMethod]
        public async Task TestParamsContextSortOrderNameAscending()
        {
            // arrange
            var server = CreateStarWarsTestServer();

            // act
            var result = await server.PostQueryAsync(@"{
                starWarsCharacters(order: {name: ASC}) {
                    id
                    name
                }
            }");

            // assert
            Assert.IsNotNull(result?.Data, "Query Execution Failed");

            var paramsContext = server.GetParamsContext("starWarsCharacters");
            Assert.IsNotNull(paramsContext.SortArgs);
            Assert.IsTrue(paramsContext.SortArgs.Count == 1);

            var sortArt = paramsContext.SortArgs[0];
            Assert.AreEqual("name", sortArt.FieldName);
            Assert.IsTrue(sortArt.IsAscending());
        }

        [TestMethod]
        public async Task TestParamsContextSortOrderIdDescending()
        {
            // arrange
            var server = CreateStarWarsTestServer();

            // act
            var result = await server.PostQueryAsync(@"{
                starWarsCharacters(order: {id: DESC}) {
                    id
                    name
                }
            }");

            // assert
            Assert.IsNotNull(result?.Data, "Query Execution Failed");

            var paramsContext = server.GetParamsContext("starWarsCharacters");
            Assert.IsNotNull(paramsContext.SortArgs);
            Assert.IsTrue(paramsContext.SortArgs.Count == 1);

            var sortArt = paramsContext.SortArgs[0];
            Assert.AreEqual("id", sortArt.FieldName);
            Assert.IsTrue(sortArt.IsDescending());
        }
    }
}
