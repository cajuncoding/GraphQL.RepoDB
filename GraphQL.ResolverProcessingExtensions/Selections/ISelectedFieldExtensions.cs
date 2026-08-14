using HotChocolate.Data.Projections.Context;
using System.Reflection;

namespace GraphQL.ResolverProcessingExtensions.Selections
{
    public static class ISelectedFieldExtensions
    {
        public static MethodInfo GetResolverMethodInfo(this ISelectedField selectedField)
            => selectedField?.Selection.Field.ResolverMember as MethodInfo;

    }
}
