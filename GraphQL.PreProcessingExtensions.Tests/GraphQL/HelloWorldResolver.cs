using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using HotChocolate;
using HotChocolate.Types;

namespace HotChocolate.PreProcessingExtensions.Tests.GraphQL
{
    [ExtendObjectType(Name = "Query")]
    public class HelloWorldResolver
    {
        [GraphQLName("hello")]
        public Task<string> GetHelloAsync(string name = "")
        {
            var result = string.IsNullOrWhiteSpace(name)
                ? "Hello World!"
                : $"Hello {name}!";

            return Task.FromResult(result);
        }
    }
}
