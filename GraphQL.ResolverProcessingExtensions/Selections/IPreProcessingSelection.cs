# nullable enable
using HotChocolate.Data.Projections.Context;
using System.Reflection;
using System;

namespace HotChocolate.ResolverProcessingExtensions
{
    public interface IResolverProcessingSelection
    {
        ISelectedField graphqlFieldSelection { get; }

        public Type RuntimeType { get; }

        MemberInfo? ClassMemberInfo { get; }
        
        string SelectionName { get; }
        
        string SelectionMemberName { get; }
        
        /// <summary>
        /// Select the MemberName if possible otherwise retrieve the SelectionName
        /// because technically the underlying IFieldSelection.Member is a nullable field.
        /// </summary>
        string SelectionMemberNameOrDefault { get; }
    }
}