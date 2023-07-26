using System;

namespace HotChocolate.PreProcessingExtensions
{
    /// <summary>
    /// Decorator/Adapter class for Projection Dependencies between field selections for pre-processing
    /// of selection fields.
    /// </summary>
    public class PreProcessingDependencyLink
    {
        /// <summary>
        /// Class Property/Member name of the dependency; this is not the GraphQL Schema name,
        /// because this is for internal dependency mapping between C# classes/models as the graphql
        /// has already been processed and at this point the primary goal is to get the correct
        /// property of the C# Class Model.
        /// </summary>
        public string DependencyMemberName { get; protected set; }

        /// <summary>
        /// Create the Dependency Link to an actual Class Property/Member name of the dependent (e.g. Parent) model; 
        /// this is not the GraphQL Schema name, because this is for internal dependency mapping between 
        /// C# classes/models as the GraphQL has already been processed and at this point the primary goal is to 
        /// get the correct property of the C# Class Model.
        /// </summary>
        /// <param name="selectionMemberName"></param>
        /// <param name="resolverMethod"></param>
        public PreProcessingDependencyLink(string selectionMemberName)
        {
            this.DependencyMemberName = selectionMemberName;
        }
    }
}
