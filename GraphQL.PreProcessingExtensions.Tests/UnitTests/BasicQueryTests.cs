using System.Threading.Tasks;
using Microsoft.AspNetCore.TestHost;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace GraphQL.PreProcessingExtensions.Tests
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
            TestServer server = CreateSimpleTestServer();

            // act
            GraphQLQueryResult result = await server.PostAsync(
                new GraphQLQueryRequest { Query = "{ hello }" }
            );


            // assert
            Assert.IsNotNull(result);

            //Data results are JObjects/JArrays
            var helloResult = result.Data["hello"].ToString();
            Assert.IsFalse(string.IsNullOrWhiteSpace(helloResult));
            Assert.AreEqual("Hello World!", helloResult);
        }

        [TestMethod]
        public async Task TestStarWarsCharactersQuery()
        {
            // arrange
            TestServer server = CreateSimpleTestServer();

            // act
            GraphQLQueryResult result = await server.PostAsync(
                new GraphQLQueryRequest 
                {              
                    Query = @"
                    {
                        starWarsCharacters {
                            id
                            name
                        }
                    }"
                }
            );

            // assert
            Assert.IsNotNull(result);

            //Data results are JObjects/JArrays
            var starWarsResults = result.Data["starWarsCharacters"] as JArray;
            Assert.IsNotNull(starWarsResults);
            Assert.IsTrue(starWarsResults.Count > 1);
        }

    }
}
