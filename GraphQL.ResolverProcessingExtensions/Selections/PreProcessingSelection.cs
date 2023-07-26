# nullable enable
using HotChocolate.Types;
using System;
using System.Reflection;
using HotChocolate.Data.Projections.Context;

namespace HotChocolate.ResolverProcessingExtensions
{
    /// <summary>
    /// Adapter class to support mapping both ObjectTypes and InterfaceType 
    /// Field objects from HotChocolate to a set of common shared values; 
    /// primarily the GraphQL Field Name for Selection/Projection.
    /// </summary>
    public class ResolverProcessingSelection : IHasName, IResolverProcessingSelection
    {
        public ResolverProcessingSelection(ISelectedField selectedField)
        {
            graphqlFieldSelection = selectedField ?? throw new ArgumentNullException(nameof(selectedField));
            if (graphqlFieldSelection.Field == null)
                throw new ArgumentNullException(nameof(graphqlFieldSelection.Field));
        }

        public Type RuntimeType => graphqlFieldSelection.Field.RuntimeType;

        public ISelectedField graphqlFieldSelection { get; }

        public MemberInfo? ClassMemberInfo => graphqlFieldSelection.Field.Member;

        public string Name => graphqlFieldSelection.Field.Name;
        public string SelectionName => Name;

        public string SelectionMemberName => ClassMemberInfo?.Name ?? Name;

        /// <summary>
        /// Select the MemberName if possible otherwise retrieve the SelectionName
        /// because technically the underlying IFieldSelection.Member is a nullable field.
        /// </summary>
        public string SelectionMemberNameOrDefault => ClassMemberInfo?.Name! ?? SelectionName;

        public override string ToString()
        {
            return $"{graphqlFieldSelection.Field.DeclaringType.Name}:{SelectionName}";
        }
    }
}
