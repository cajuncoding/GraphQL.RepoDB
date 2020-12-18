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
    public class ParamsContextArgumentsTests : GraphQLTestBase
    {
        public ParamsContextArgumentsTests() : base(new GraphQLTestServerFactory())
        {
        }

        [TestMethod]
        public async Task TestParamsContextAllArgumentSchemaNamesNoneProvided()
        {
            // arrange
            var server = CreateHelloWorldTestServer();

            // act
            var result = await server.PostQueryAsync("{ hello }");

            // assert
            Assert.IsNotNull(result?.Data, "Query Execution Failed");

            var paramsContext = server.GetParamsContext("hello");
            Assert.IsNotNull(paramsContext?.AllArgumentSchemaNames);
            Assert.AreEqual(paramsContext.AllArgumentSchemaNames.Count, 1);
            Assert.AreEqual(paramsContext.AllArgumentSchemaNames[0], "name");
        }

        [TestMethod]
        public async Task TestParamsContextAllArgumentSchemaNamesSortProvided()
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
            Assert.IsNotNull(paramsContext?.AllArgumentSchemaNames);
            Assert.AreEqual(paramsContext.AllArgumentSchemaNames.Count, 1);
            Assert.AreEqual(paramsContext.AllArgumentSchemaNames[0], "order");
        }

        [TestMethod]
        public async Task TestParamsContextAllArgumentsNoneProvided()
        {
            // arrange
            var server = CreateHelloWorldTestServer();

            // act
            var result = await server.PostQueryAsync(@"{ hello }");

            // assert
            Assert.IsNotNull(result?.Data, "Query Execution Failed");

            var paramsContext = server.GetParamsContext("hello");
            Assert.IsNotNull(paramsContext?.AllArguments);
            Assert.AreEqual(paramsContext.AllArguments.Count, 0);
        }

        [TestMethod]
        public async Task TestParamsContextAllArgumentsSortProvided()
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
            Assert.IsNotNull(paramsContext?.AllArguments);
            Assert.AreEqual(paramsContext.AllArguments.Count, 1);
            Assert.IsNotNull(paramsContext?.AllArguments[0]);
            Assert.AreEqual(paramsContext.AllArguments[0].Name, "order");
            Assert.IsNotNull(paramsContext?.AllArguments[0].Value);
        }
    }
}
