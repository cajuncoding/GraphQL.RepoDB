# nullable enable
using HotChocolate.Types;
using System;
using System.Reflection;
using HotChocolate.Data.Projections.Context;
using HotChocolate.Execution.Processing;

namespace HotChocolate.PreProcessingExtensions
{
    /// <summary>
    /// Adapter class to support mapping both ObjectTypes and InterfaceType 
    /// Field objects from HotChocolate to a set of common shared values; 
    /// primarily the GraphQL Field Name for Selection/Projection.
    /// </summary>
    public class PreProcessingSelection : IHasName, IPreProcessingSelection
    {
        public PreProcessingSelection(ISelectedField selectedField)
        {
            GraphQLFieldSelection = selectedField ?? throw new ArgumentNullException(nameof(selectedField));
            if (GraphQLFieldSelection.Field == null)
                throw new ArgumentNullException(nameof(GraphQLFieldSelection.Field));
        }

        public Type RuntimeType => GraphQLFieldSelection.Field.RuntimeType;

        public ISelectedField GraphQLFieldSelection { get; }

        public MemberInfo? ClassMemberInfo => GraphQLFieldSelection.Field?.Member;

        //TODO: TEST Which of these can/should be used???
        public string Name => GraphQLFieldSelection.Field.Name;
        public string SelectionName => GraphQLFieldSelection.Field.Name;

        public string SelectionMemberName => ClassMemberInfo?.Name! ?? SelectionName;

        /// <summary>
        /// Select the MemberName if possible otherwise retrieve the SelectionName
        /// because technically the underlying IFieldSelection.Member is a nullable field.
        /// </summary>
        public string SelectionMemberNameOrDefault => ClassMemberInfo?.Name! ?? SelectionName;

        public override string ToString()
        {
            return $"{GraphQLFieldSelection.Field.DeclaringType.Name}:{SelectionName}";
        }
    }
}
