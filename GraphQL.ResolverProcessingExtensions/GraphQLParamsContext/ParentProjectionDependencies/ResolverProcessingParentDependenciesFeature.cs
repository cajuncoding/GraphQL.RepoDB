using System.Collections.Generic;

namespace HotChocolate.ResolverProcessingExtensions
{
    public sealed class ResolverProcessingParentDependenciesFeature
    {
        public ResolverProcessingParentDependenciesFeature(IReadOnlyList<ResolverProcessingDependencyLink> dependencies)
        {
            Dependencies = dependencies;
        }

        public IReadOnlyList<ResolverProcessingDependencyLink> Dependencies { get; }
    }
}
