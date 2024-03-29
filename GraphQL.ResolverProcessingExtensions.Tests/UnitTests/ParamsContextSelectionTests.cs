using System.Linq;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HotChocolate.ResolverProcessingExtensions.Tests
{
    [TestClass]
    public class ParamsContextSelectionTests : GraphQLTestBase
    {
        public ParamsContextSelectionTests() : base(new GraphQLTestServerFactory())
        {
        }

        [TestMethod]
        public async Task TestParamsContextSelectionsWhenNotEnabled()
        {
            // arrange
            var server = CreateHelloWorldTestServer();

            // act
            var result = await server.PostQueryAsync(@"{ hello }");

            // assert
            Assert.IsNotNull(result?.Data, "Query Execution Failed");

            var paramsContext = server.GetParamsContext("hello");
            Assert.IsNotNull(paramsContext);
            Assert.IsNotNull(paramsContext.AllSelectionFields);
            Assert.IsFalse(paramsContext.AllSelectionFields.Any());
        }

        [TestMethod]
        public async Task TestParamsContextCursorSelectTotalCountCursorPaging()
        {
            // arrange
            var server = CreateStarWarsTestServer();

            // Validate TotalCount Is Specified!
            var result = await server.PostQueryAsync(@"{
                starWarsCharactersCursorPaginated(first:2) {
                    totalCount
                    nodes {
                        name
                        id
                    }
                }
            }");

            // assert
            var queryKey = "starWarsCharactersCursorPaginated";
            var paramsContext = server.GetParamsContext(queryKey);
            Assert.IsNotNull(paramsContext?.TotalCountSelection);
            Assert.IsTrue(paramsContext.IsTotalCountRequested);

            // Validate TotalCount is Not Specified!
            result = await server.PostQueryAsync(@"{
                starWarsCharactersCursorPaginated(first:2) {
                    nodes {
                        id
                    }
                }
            }");

            // assert
            paramsContext = server.GetParamsContext(queryKey);
            Assert.IsNull(paramsContext.TotalCountSelection);
            Assert.IsFalse(paramsContext.IsTotalCountRequested);
        }

        [TestMethod]
        public async Task TestParamsContextCursorSelectTotalCountOffsetPaging()
        {
            // arrange
            var server = CreateStarWarsTestServer();

            // Validate TotalCount Is Specified!
            var result = await server.PostQueryAsync(@"{
                starWarsCharactersOffsetPaginated(skip: 2, take: 2) {
                    totalCount
                    items {
                        name
                        id
                    }
                }
            }");

            // assert
            var queryKey = "starWarsCharactersOffsetPaginated";
            var paramsContext = server.GetParamsContext(queryKey);
            Assert.IsNotNull(paramsContext?.TotalCountSelection);
            Assert.IsTrue(paramsContext.IsTotalCountRequested);

            // Validate TotalCount is Not Specified!
            result = await server.PostQueryAsync(@"{
                starWarsCharactersOffsetPaginated(skip: 2, take: 2) {
                    items {
                        id
                    }
                }
            }");

            // assert
            paramsContext = server.GetParamsContext(queryKey);
            Assert.IsNull(paramsContext.TotalCountSelection);
            Assert.IsFalse(paramsContext.IsTotalCountRequested);
        }



        //TODO: Add a few more tests for Cursor Paging with Edges, Offset Paging, and No Paging...
        [TestMethod]
        public async Task TestParamsContextSelectionsWithCursorPagingNodes()
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

            var selectionNames = paramsContext?.AllSelectionNames;
            Assert.IsNotNull(selectionNames);

            Assert.AreEqual(selectionNames.Count, 2);
            Assert.AreEqual("id", selectionNames.FirstOrDefault());
            Assert.AreEqual("name", selectionNames.LastOrDefault());
        }

        [TestMethod]
        public async Task TestParamsContextSelectionsWithCursorPagingEdges()
        {
            // arrange
            var server = CreateStarWarsTestServer();

            // act
            var result = await server.PostQueryAsync(@"{
                starWarsCharactersCursorPaginated(first:2) {
                    edges {
                        node {
                            id
                            name
                        }
                    }
                }
            }");

            // assert
            Assert.IsNotNull(result?.Data, "Query Execution Failed");

            var queryKey = "starWarsCharactersCursorPaginated";
            var paramsContext = server.GetParamsContext(queryKey);

            var selectionNames = paramsContext?.AllSelectionNames;
            Assert.IsNotNull(selectionNames);

            Assert.AreEqual(selectionNames.Count, 2);
            Assert.AreEqual("id", selectionNames.FirstOrDefault());
            Assert.AreEqual("name", selectionNames.LastOrDefault());
        }

        [TestMethod]
        public async Task TestParamsContextSelectionsWithOffsetPaging()
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

            var selectionNames = paramsContext?.AllSelectionNames;
            Assert.IsNotNull(selectionNames);

            Assert.AreEqual(selectionNames.Count, 2);
            Assert.AreEqual("id", selectionNames.FirstOrDefault());
            Assert.AreEqual("name", selectionNames.LastOrDefault());
        }

        [TestMethod]
        public async Task TestParamsContextDependencySelectionForId()
        {
            // arrange
            var server = CreateStarWarsTestServer();

            // act
            var result = await server.PostQueryAsync(@"{
            starWarsCharacters(order: {name: ASC}) {
                    name
                    ... on StarWarsHuman {                    
                        droids {
                            name
                            primaryFunction
                        }
                    }
                }
            }");

            // assert
            Assert.IsNotNull(result?.Data, "Query Execution Failed");

            var paramsContext = server.GetParamsContext("starWarsCharacters");
            Assert.IsNotNull(paramsContext?.SelectionDependencies);
            Assert.AreEqual(1, paramsContext.SelectionDependencies.Count);
            Assert.AreEqual(nameof(IStarWarsCharacter.Id), paramsContext.SelectionDependencies[0].DependencyMemberName);

            Assert.IsNotNull(paramsContext?.AllSelectionNames);
            Assert.AreEqual(2, paramsContext.AllSelectionNames.Count);
            Assert.AreEqual("name", paramsContext.AllSelectionNames[0]);
            Assert.AreEqual("droids", paramsContext.AllSelectionNames[1]);
        }
    }
}
