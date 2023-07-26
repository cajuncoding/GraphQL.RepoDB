using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HotChocolate;
using HotChocolate.Types;

namespace HotChocolate.PreProcessingExtensions.Tests.GraphQL
{
    [ExtendObjectType("Query")]
    public class HelloWorldResolver
    {
        [GraphQLName("hello")]
        public Task<string> GetHelloAsync(
            [GraphQLParams] IParamsContext paramsContext, 
            string name = ""
        )
        {
            var result = string.IsNullOrWhiteSpace(name)
                ? "Hello World!"
                : $"Hello {name}!";

            return Task.FromResult(result);
        }
    }
}
