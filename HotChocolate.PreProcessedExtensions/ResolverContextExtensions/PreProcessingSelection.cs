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
        public PreProcessingSelection(IFieldSelection selectionField)
            : this(selectionField.Field.Name)
        {
            FieldSelection = selectionField;
        }

        public PreProcessingSelection(InterfaceField field)
            : this(field.Name)
        {
        }

        protected PreProcessingSelection(NameString name)
        {
            SelectionName = name;
        }

        public NameString SelectionName { get; }

        public NameString Name => SelectionName;

        public IFieldSelection? FieldSelection { get; }

        public bool HasFieldSelection => FieldSelection != null;

        public override string ToString()
        {
            return SelectionName.ToString();
        }
    }
}
