# nullable enable
using HotChocolate.Types;
using HotChocolate.Data.Projections.Context;
using System.Reflection;
using HotChocolate.Execution.Processing;

namespace HotChocolate.PreProcessingExtensions
{
    public interface IPreProcessingSelection
    {
        ISelection GraphQLFieldSelection { get; }
        
        ObjectType GraphQLObjectType { get; }
        
        MemberInfo? ClassMemberInfo { get; }
        
        //TODO: DELETE If now redundant since NameString isn't available anymore...
        string Name { get; }
        
        string SelectionName { get; }
        
        string SelectionMemberName { get; }
        
        /// <summary>
        /// Select the MemberName if possible otherwise retrieve the SelectionName
        /// because technically the underlying IFieldSelection.Member is a nullable field.
        /// </summary>
        string SelectionMemberNameOrDefault { get; }
    }
}