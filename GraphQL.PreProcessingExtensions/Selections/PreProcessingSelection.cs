# nullable enable
using HotChocolate.Types;
using System;
using System.Reflection;
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
        public PreProcessingSelection(ObjectType objectType, ISelection selectionField)
        {
            GraphQLObjectType = objectType ?? throw new ArgumentNullException(nameof(objectType));
            GraphQLFieldSelection = selectionField ?? throw new ArgumentNullException(nameof(selectionField));
            ClassMemberInfo = selectionField.Field?.Member;
        }

        public ObjectType GraphQLObjectType { get; }

        public ISelection GraphQLFieldSelection { get; }

        public MemberInfo? ClassMemberInfo { get; }

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
            return $"{GraphQLObjectType.Name}:{SelectionName}";
        }
    }
}
