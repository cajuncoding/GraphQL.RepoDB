# nullable enable

using HotChocolate.Resolvers;
using HotChocolate.Types;
using System.Reflection;

namespace HotChocolate.PreProcessedExtensions
{
    public interface IPreProcessingSelection
    {
        IFieldSelection GraphQLFieldSelection { get; }
        ObjectType GraphQLObjectType { get; }
        MemberInfo? ClassMemberInfo { get; }
        NameString Name { get; }
        string SelectionName { get; }
        string SelectionMemberName { get; }
        /// <summary>
        /// Select the MemberName if possible otherwise retrieve the SelectionName
        /// because technically the underlying IFieldSelection.Member is a nullable field.
        /// </summary>
        string SelectionMemberNameOrDefault { get; }
    }
}