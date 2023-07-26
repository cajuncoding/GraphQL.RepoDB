using System;
using HotChocolate.Resolvers;

namespace HotChocolate.PreProcessingExtensions
{
    public static class GraphQLParamsContextResolverExtensions
    {
        public static IParamsContext InitializeGraphQLParamsContextSafely(this IResolverContext context)
        {
            var paramsContextFacade = context.GetGraphQLParamsContext() 
                ?? context.SetGraphQLParamsContext(new GraphQLParamsContext(context));

            return paramsContextFacade;
        }

        internal static IParamsContext SetGraphQLParamsContext(this IResolverContext context, IParamsContext paramsContextFacade)
        {
            context.SetLocalState(nameof(GraphQLParamsContext), paramsContextFacade);
            return paramsContextFacade;
        }

        public static IParamsContext GetGraphQLParamsContext(this IResolverContext context)
        {
            return context.GetLocalStateOrDefault<IParamsContext>(nameof(GraphQLParamsContext));
        }
    }
}
