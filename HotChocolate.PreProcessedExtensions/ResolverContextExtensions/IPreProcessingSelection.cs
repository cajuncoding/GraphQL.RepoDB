using HotChocolate.Resolvers;
using HotChocolate.Types;

namespace HotChocolate.PreProcessedExtensions
{
    public interface IPreProcessingSelection
    {
        IFieldSelection GraphQLFieldSelection { get; }
        ObjectType GraphQLObjectType { get; }
        NameString Name { get; }
        string SelectionName { get; }
    }
}