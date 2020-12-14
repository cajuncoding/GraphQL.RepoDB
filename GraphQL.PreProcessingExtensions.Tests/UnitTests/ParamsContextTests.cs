using System.Collections.Generic;
using System.Threading.Tasks;
using HotChocolate;
using HotChocolate.PreProcessingExtensions;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
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
        public async Task TestParamsContextMiddlewareInjection()
        {
            // arrange
            GraphQLParamsContext paramsContext = null;

            TestServer server = CreateSimpleTestServer(
                fieldMiddleware: next => context =>
                {
                    //BBernard intercept the ParamsContext initialized by the PreProcessing Results Middleware...
                    paramsContext = context.GetLocalValue<GraphQLParamsContext>(nameof(GraphQLParamsContext));

                    //BBernard - WE MUST ALLOW THE PIPELINE TO CONTINUE!
                    return next.Invoke(context);
                }
            );


            // act
            GraphQLQueryResult result = await server.PostAsync(
                new GraphQLQueryRequest { Query = "{ hello }" }
            );

            // assert
            Assert.IsNotNull(result);
            Assert.IsNotNull(paramsContext);
        }

    }
}
