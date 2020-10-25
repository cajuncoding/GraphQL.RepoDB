# nullable enable

using HotChocolate.Resolvers;
using HotChocolate.Types;
using System;
using System.Collections.Generic;
using System.Text;

namespace HotChocolate.PreProcessedExtensions
{
    /// <summary>
    /// Adapter class to support mapping both ObjectTypes and InterfaceType 
    /// Field objects from HotChocolate to a set of common shared values; 
    /// primariy the GraphQL Field Name for Selection/Projection.
    /// </summary>
    public class PreProcessingSelection : IHasName, IPreProcessingSelection
    {
        public PreProcessingSelection(ObjectType objectType, IFieldSelection selectionField)
        {
            GraphQLObjectType = objectType;
            GraphQLFieldSelection = selectionField;
        }

        public ObjectType GraphQLObjectType { get; }

        public IFieldSelection GraphQLFieldSelection { get; }

        public string SelectionName => GraphQLFieldSelection.ResponseName.ToString();

        public NameString Name => GraphQLFieldSelection.ResponseName;

        public override string ToString()
        {
            return $"{GraphQLObjectType.Name}:{SelectionName}";
        }
    }
}
