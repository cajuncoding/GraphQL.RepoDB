# nullable enable
using HotChocolate.Types;
using System;
using System.Reflection;
using HotChocolate.Data.Projections.Context;
using GraphQL.ResolverProcessingExtensions.Selections;

namespace HotChocolate.ResolverProcessingExtensions
{
    /// <summary>
    /// Adapter class to support mapping both ObjectTypes and InterfaceType 
    /// Field objects from HotChocolate to a set of common shared values; 
    /// primarily the GraphQL Field Name for Selection/Projection.
    /// </summary>
    public class ResolverProcessingSelection : INameProvider, IResolverProcessingSelection
    {
        public ResolverProcessingSelection(ISelectedField selectedField)
        {
            GraphQLSelectedField = selectedField ?? throw new ArgumentNullException(nameof(selectedField));
            if (GraphQLSelectedField.Field == null)
                throw new ArgumentNullException(nameof(GraphQLSelectedField.Field));
        }

        public Type RuntimeType => GraphQLSelectedField.Type.ToRuntimeType();

        public ISelectedField GraphQLSelectedField { get; }

        public MemberInfo? ClassMemberInfo => GraphQLSelectedField.GetResolverMethodInfo();

        public string Name => GraphQLSelectedField.Field.Name;
        public string SelectionName => Name;

        public string SelectionMemberName => ClassMemberInfo?.Name ?? Name;

        /// <summary>
        /// Select the MemberName if possible otherwise retrieve the SelectionName
        /// because technically the underlying IFieldSelection.Member is a nullable field.
        /// </summary>
        public string SelectionMemberNameOrDefault => ClassMemberInfo?.Name! ?? SelectionName;

        public override string ToString()
        {
            return $"{GraphQLSelectedField.Field.DeclaringType.Name}:{SelectionName}";
        }
    }
}
