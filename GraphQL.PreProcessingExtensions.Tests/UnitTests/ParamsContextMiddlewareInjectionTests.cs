using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HotChocolate;
using HotChocolate.PreProcessingExtensions;
using Microsoft.AspNetCore.TestHost;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;

namespace GraphQL.PreProcessingExtensions.Tests
{
    [TestClass]
    public class ParamsContextTests : GraphQLTestBase
    {
        public ParamsContextTests() : base(new GraphQLTestServerFactory())
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
            Assert.IsTrue(server.ParamsContextLookup.Count == 1);
        }

        [TestMethod]
        public async Task TestParamsContextExecutionForAllFieldsOfAllResults()
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

            //SHOULD Execute Once for the main resolver, and again for each field since it is injected as FieldMiddleware!
            var starWarsResults = (JArray)result.Data["starWarsCharacters"];
            var resultCount = starWarsResults.Count;
            Assert.AreEqual(server.ParamsContextList.Count, 1 + (resultCount * 2));
        }
    }
}
