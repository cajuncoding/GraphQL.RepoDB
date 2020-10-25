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
            : this(selectionField.Field.Name.Value)
        {
            FieldSelection = selectionField;
        }

        public PreProcessingSelection(Language.NamedSyntaxNode selectionNode)
            : this(selectionNode.Name.Value)
        {
        }

        public PreProcessingSelection(Language.InlineFragmentNode fragmentNode, Language.NamedSyntaxNode selectionNode)
            : this(selectionNode.Name.Value)
        {
            ParentFragmentNode = fragmentNode;
        }

        protected PreProcessingSelection(NameString name)
        {
            SelectionName = name;
        }

        public string SelectionName { get; }

        public NameString Name => new NameString(SelectionName);

        public IFieldSelection? FieldSelection { get; }

        public bool IsFieldSelection => FieldSelection != null;

        public Language.InlineFragmentNode? ParentFragmentNode { get; }
        
        public bool IsFragmentSelection => ParentFragmentNode != null;


        public override string ToString()
        {
            return SelectionName.ToString();
        }
    }
}
