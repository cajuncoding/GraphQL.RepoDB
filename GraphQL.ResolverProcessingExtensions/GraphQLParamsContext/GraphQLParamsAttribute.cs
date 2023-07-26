using System;

namespace HotChocolate.ResolverProcessingExtensions
{
    /// <summary>
    /// Convenience Attribute to support Local Injecting of GraphQL Parameters
    /// already parsed in the IParamsContext class for greatly simplified resolver code 
    /// when using Resolver Processed extensions to perform Sorting, Paging, etc. inside the Resolver
    /// or lower layer of code (e.g. Service/Repository).
    /// </summary>
    [AttributeUsage(AttributeTargets.Parameter)]
    public class GraphQLParamsAttribute : LocalStateAttribute
    {
        public GraphQLParamsAttribute() : base(nameof(GraphQLParamsContext))
        {
        }
    }
}
