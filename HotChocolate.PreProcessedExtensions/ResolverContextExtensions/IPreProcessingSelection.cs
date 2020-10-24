using HotChocolate.Resolvers;

namespace HotChocolate.PreProcessedExtensions
{
    public interface IPreProcessingSelection
    {
        IFieldSelection FieldSelection { get; }
        bool HasFieldSelection { get; }
        NameString Name { get; }
        NameString SelectionName { get; }
    }
}