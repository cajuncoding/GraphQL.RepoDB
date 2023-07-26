using HotChocolate.PreProcessingExtensions;
using System;

namespace HotChocolate.RepoDb
{
    /// <summary>
    /// Convenience Attribute to support Local Injecting of GraphQL RepoDB Mapper
    /// already constructed, with the IParamsContext already injected, for greatly simplified resolver code 
    /// when using RepoDb to perform database logic within in the Resolver or lower layer of code (e.g. Service/Repository).
    /// </summary>
    [AttributeUsage(AttributeTargets.Parameter)]
    public class GraphQLRepoDbMapperAttribute : LocalStateAttribute
    {
        public GraphQLRepoDbMapperAttribute() : base(nameof(IGraphQLRepoDbMapper))
        {
        }
    }
}
