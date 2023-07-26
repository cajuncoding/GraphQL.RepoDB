using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;

namespace HotChocolate.ResolverProcessingExtensions.Tests
{
    [TestClass]
    public class ParamsContextTests : GraphQLTestBase
    {
        public ParamsContextTests() : base(new GraphQLTestServerFactory())
        {
        }

        [TestMethod]
        public void TestParamsContextInterfaceImplementationsByAssemblyScan()
        {
            // arrange
            var searchType = typeof(IParamsContext);
            var searchAssembly = searchType.Assembly;

            // act
            var implementationTypes = searchAssembly.GetTypes().Where(t => t.IsClass && searchType.IsAssignableFrom(t)).ToList();

            // assert
            Assert.AreEqual(implementationTypes.Count, 1);
            Assert.AreEqual(implementationTypes[0], typeof(GraphQLParamsContext));
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
        public async Task TestParamsContextExecutionForAllFieldsOnlyWhenRequested()
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

            //SHOULD Execute ONLY Once for the main resolver as no other Field requests it!
            var starWarsResults = (JArray)result.Data["starWarsCharacters"];
            var resultCount = starWarsResults.Count;
            Assert.AreEqual(1, server.ParamsContextList.Count);
        }
    }
}
