using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using HotChocolate.Types;
using HotChocolate.Types.Descriptors;

namespace HotChocolate.PreProcessedExtensions
{
    /// <summary>
    /// Static Elements for PreProcessing Parent Dependencies.
    /// </summary>
    public static class PreProcessingParentDependencies
    {
        public const string ContextDataKey = nameof(PreProcessingParentDependencies);
    }

    /// <summary>
    /// Configuratino Extensions for "Code First" configuration of Parent Selection/Project
    /// dependencies for child resolver methods. By specifying Selection names here
    /// we make it easy for the GraphQLParamsContext to resolve dependent Selection field
    /// names dynamically.
    /// </summary>
    public static class ExtensionDataExtensionsForPreProcessingProjectionDependencies
    {
        public static ExtensionData AddPreProcessingParentProjectionDependencies(this ExtensionData configure, MemberInfo memberInfo, params string[] selectionDependencies)
        {
            //Create list of Dependency Links...
            IReadOnlyList<PreProcessingDependencyLink> dependencies = selectionDependencies
                .Select(s => new PreProcessingDependencyLink(s, memberInfo))
                .ToList();
            
            //Add to the pre-compiled Field Context for future use/retrieval.
            configure.Add(PreProcessingParentDependencies.ContextDataKey, dependencies);
            
            //Keep chainable...
            return configure;
        }

    }
}
