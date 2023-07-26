using HotChocolate.Resolvers;
using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Reflection;
using System.Reflection.GraphQLRepoDb.CustomExtensions;
using System.Threading.Tasks;
using HotChocolate.Data.Projections.Context;
using HotChocolate.PreProcessingExtensions;

namespace HotChocolate.RepoDb
{
    public class GraphQLRepoDbMappingMiddleware
    {
        private static readonly ConcurrentDictionary<MethodInfo, Lazy<Delegate>> _repoDbMapperDelegateCache = new();

        private static readonly MethodInfo _createRepoDbMapperFactoryMethod = StaticReflectionHelper.FindStaticMethod(
            typeof(GraphQLRepoDbMappingMiddleware),
            nameof(CreateGraphQLRepoDbMapperInternal),
            new[] { typeof(IParamsContext) }
        );

        private readonly FieldDelegate _next;

        public GraphQLRepoDbMappingMiddleware(FieldDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(IMiddlewareContext context)
        {
            var field = context.GetSelectedField();

            if (field.Field.ResolverMember is MethodInfo resolverMethod 
                && GetRepoDbMapperFactoryForResolver(resolverMethod) is { } repoDbMapperFactoryDelegate)
            {
                //ONLY initialize/retrieve the  Params Context if the Resolver is requesting (via DI) the GraphQLRepoDbMapper0...
                var paramsContextFacade = context.InitializeGraphQLParamsContextSafely();

                var graphqlRepoDbMapper = (IGraphQLRepoDbMapper)repoDbMapperFactoryDelegate.DynamicInvoke(paramsContextFacade);
                context.SetGraphQLRepoDbMapper(graphqlRepoDbMapper);
            }

            await _next(context).ConfigureAwait(false);
        }

        /// <summary>
        /// This method will interrogate the Resolver signature (MethodInfo) to determine if it is requesting that
        ///     a GraphQLRepoDbMapper be injected, and if so then it will dynamically resolve the Generic Entity Type
        ///     and create a factory Delegate that dynamically creates the RepoDbMapper needed for the Entity Type.
        /// NOTE: With the use of Lazy caching (e.g. blocking/self-populating cache) we get great iterative performance
        ///     because the reflection to compute the factory delegate for this Resolver is only Ever executed one-and-only-one time!
        /// </summary>
        /// <param name="resolverMethod"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        private static Delegate GetRepoDbMapperFactoryForResolver(MethodInfo resolverMethod)
        {
            var doesResolverNeedParamsContextLazy = _repoDbMapperDelegateCache.GetOrAdd(
                resolverMethod,
                new Lazy<Delegate>(() =>
                {
                    var resolverParams = resolverMethod.GetParameters();
                    var repoDbMapperParam = resolverParams.FirstOrDefault(p => 
                        p.ParameterType.IsAssignableTo(typeof(IGraphQLRepoDbMapper))
                    );

                    if (repoDbMapperParam?.ParameterType is {} paramType)
                    {
                        //In order to correctly Map the GraphQL Params Context to an Entity for RepoDb, we must know
                        //  the have a Generic Type for the Entity therefore we require that the Resolver Param be a concrete type with Generic Parameter.
                        //  
                        if (paramType.IsGenericType)
                        {
                            var genericType = paramType.GenericTypeArguments.First();
                            return _createRepoDbMapperFactoryMethod.CreateDynamicDelegate(genericType);
                        }
                        else if (paramType == typeof(IGraphQLRepoDbMapper))
                        {
                            throw new ArgumentException($"The Resolver method signature is expecting a parameter of type [{nameof(IGraphQLRepoDbMapper)}]"
                                                        + " however, a concrete generic type must be specified so that the correct mapping can be resolved;"
                                                        + " use GraphQLRepoDbMapper<TEntity> instead.");
                        }
                    }

                    return null;
                })
            );

            return doesResolverNeedParamsContextLazy.Value;
        }

        private static GraphQLRepoDbMapper<TEntity> CreateGraphQLRepoDbMapperInternal<TEntity>(IParamsContext graphqlParamsContext) where TEntity : class
            => new GraphQLRepoDbMapper<TEntity>(graphqlParamsContext);

    }
}
