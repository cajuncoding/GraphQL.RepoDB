using System.Threading.Tasks;
using Microsoft.AspNetCore.TestHost;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace HotChocolate.ResolverProcessingExtensions.Tests
{
    [TestClass]
    public class BasicQueryTests : GraphQLTestBase
    {
        public BasicQueryTests() : base(new GraphQLTestServerFactory())
        {
        }

        [TestMethod]
        public async Task TestHelloWorldQuery()
        {
            // arrange
            var server = CreateHelloWorldTestServer();

            // act
            var result = await server.PostQueryAsync("{ hello }");

            // assert
            Assert.IsNotNull(result?.Data, "Query Execution Failed");

            //Data results are JObjects/JArrays
            var helloResult = result.Data["hello"].ToString();
            Assert.IsFalse(string.IsNullOrWhiteSpace(helloResult));
            Assert.AreEqual("Hello World!", helloResult);
        }

        [TestMethod]
        public async Task TestStarWarsCharactersQuery()
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

            //Data results are JObjects/JArrays
            var starWarsResults = (JArray)result.Data["starWarsCharacters"];
            Assert.IsNotNull(starWarsResults);
            Assert.IsTrue(starWarsResults.Count > 1);
        }

    }
}
