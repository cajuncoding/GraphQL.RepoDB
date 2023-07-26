using System;
using HotChocolate.Resolvers;

namespace HotChocolate.RepoDb
{
    public static class GraphQLRepoDbMapperResolverExtensions
    {
        internal static IGraphQLRepoDbMapper SetGraphQLRepoDbMapper(this IResolverContext context, IGraphQLRepoDbMapper graphqlRepoDbMapper)
        {
            context.SetLocalState(nameof(IGraphQLRepoDbMapper), graphqlRepoDbMapper);
            return graphqlRepoDbMapper;
        }

        public static IGraphQLRepoDbMapper GetGraphQLRepoDbMapper(this IResolverContext context)
        {
            return context.GetLocalStateOrDefault<IGraphQLRepoDbMapper>(nameof(IGraphQLRepoDbMapper));
        }
    }
}
