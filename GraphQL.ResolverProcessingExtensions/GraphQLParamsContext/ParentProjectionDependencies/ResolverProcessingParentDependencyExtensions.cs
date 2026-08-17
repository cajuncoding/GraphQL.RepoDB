using System.Collections.Generic;
using System.Linq;
using HotChocolate.Types;

namespace HotChocolate.ResolverProcessingExtensions
{
    /// <summary>
    /// Static Elements for ResolverProcessing Parent Dependencies.
    /// </summary>
    public static class ResolverProcessingParentDependencies
    {
        public const string ContextDataKey = nameof(ResolverProcessingParentDependencies);
    }

    /// <summary>
    /// Configuration Extensions for "Code First" configuration of Parent Selection/Project
    /// dependencies for child resolver methods. By specifying Selection names here
    /// we make it easy for the graphqlParamsContext to resolve dependent Selection field
    /// names dynamically.
    /// </summary>
    public static class ExtensionDataExtensionsForResolverProcessingProjectionDependencies
    {
        public static IObjectFieldDescriptor AddResolverProcessingParentProjectionDependencies(this IObjectFieldDescriptor descriptor, params string[] selectionDependencies)
        {
            //Create list of Dependency Links...
            IReadOnlyList<ResolverProcessingDependencyLink> dependencies = selectionDependencies
                .Select(s => new ResolverProcessingDependencyLink(s))
                .ToList();

            //Add to the pre-compiled Field Features for future use/retrieval.
            descriptor.AddResolverProcessingParentDependenciesFeature(dependencies);

            //Keep chainable...
            return descriptor;
        }

        public static void AddResolverProcessingParentDependenciesFeature(this IObjectFieldDescriptor descriptor, IReadOnlyList<ResolverProcessingDependencyLink> dependencies)
        {
            descriptor
                .Extend()
                .OnBeforeCreate((descriptorContext, fieldDefinition) =>
                {
                    fieldDefinition.Features.Set(new ResolverProcessingParentDependenciesFeature(dependencies));
                });
        }
    }
}
