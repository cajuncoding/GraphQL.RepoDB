using HotChocolate.Resolvers;

namespace HotChocolate.PreProcessedExtensions
{
    public interface IPreProcessingSelection
    {
        IFieldSelection FieldSelection { get; }
        bool IsFieldSelection { get; }
        NameString Name { get; }
        string SelectionName { get; }
    }
}