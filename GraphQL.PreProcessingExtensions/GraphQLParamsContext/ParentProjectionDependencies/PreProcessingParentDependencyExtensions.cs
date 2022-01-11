using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using HotChocolate.Types;
using HotChocolate.Types.Descriptors;
using HotChocolate.Types.Descriptors.Definitions;
using HotChocolate.Types.Pagination;

namespace HotChocolate.PreProcessingExtensions
{
    /// <summary>
    /// Static Elements for PreProcessing Parent Dependencies.
    /// </summary>
    public static class PreProcessingParentDependencies
    {
        public const string ContextDataKey = nameof(PreProcessingParentDependencies);
    }

    /// <summary>
    /// Configuration Extensions for "Code First" configuration of Parent Selection/Project
    /// dependencies for child resolver methods. By specifying Selection names here
    /// we make it easy for the GraphQLParamsContext to resolve dependent Selection field
    /// names dynamically.
    /// </summary>
    public static class ExtensionDataExtensionsForPreProcessingProjectionDependencies
    {
        public static IObjectFieldDescriptor AddPreProcessingParentProjectionDependencies(this IObjectFieldDescriptor descriptor, params string[] selectionDependencies)
        {
            //Create list of Dependency Links...
            IReadOnlyList<PreProcessingDependencyLink> dependencies = selectionDependencies
                .Select(s => new PreProcessingDependencyLink(s))
                .ToList();

            //Add to the pre-compiled Field Context for future use/retrieval.
            descriptor.AddDescriptorContextData(new Dictionary<string, object>()
            {
                [PreProcessingParentDependencies.ContextDataKey] = dependencies
            });

            //Keep chainable...
            return descriptor;
        }

        public static void AddDescriptorContextData(this IObjectFieldDescriptor descriptor, IReadOnlyDictionary<string, object> contextBag)
        {
            //var context = descriptor;
            //context.ContextData.TryAdd(key, value);
            descriptor
                .Extend()
                .OnBeforeCreate((descriptorContext, fieldDefinition) =>
                {
                    foreach (var (key, value) in contextBag)
                        fieldDefinition.ContextData.TryAdd(key, value);
                });
        }
    }
}
