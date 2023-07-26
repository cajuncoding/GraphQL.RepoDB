using System.Reflection;
using HotChocolate.Types;
using HotChocolate.Types.Descriptors;

namespace HotChocolate.ResolverProcessingExtensions
{
    /// <summary>
    /// Custom Attribute for "Pure Code First" configuration of Parent Selection/Projection
    /// dependencies for child resolver methods. By specifying Selection names here
    /// we make it easy for the graphqlParamsContext to resolve dependent Selection field
    /// names dynamically.
    /// </summary>
    public class ResolverProcessingParentDependenciesAttribute : ObjectFieldDescriptorAttribute
    {

        public string[] SelectionDependencies { get; }

        public ResolverProcessingParentDependenciesAttribute(params string[] selections)
        {
            SelectionDependencies = selections;
        }

        protected override void OnConfigure(IDescriptorContext context, IObjectFieldDescriptor descriptor, MemberInfo member)
        {
            //Dynamically pipe the specified Dependencies into the custom ContextData for this Field!
            descriptor.AddResolverProcessingParentProjectionDependencies(this.SelectionDependencies);
        }
    }

}