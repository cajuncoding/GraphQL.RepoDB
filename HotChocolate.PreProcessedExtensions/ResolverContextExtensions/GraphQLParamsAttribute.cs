using HotChocolate;
using System;
using System.Collections.Generic;
using System.Text;

namespace HotChocolate.PreProcessedExtensions
{
    /// <summary>
    /// Convenience Attribute to support Local Injecting of GraphQL Parameters
    /// alreayd parsed in the IParamsContext class for greatly simplified resolver code 
    /// when using Pre-Processed extensions to perform Sorting, Paging, etc. in the Resolver
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
