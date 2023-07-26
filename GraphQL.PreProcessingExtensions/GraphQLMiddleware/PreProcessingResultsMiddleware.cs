using HotChocolate.Resolvers;
using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using HotChocolate.Data.Projections.Context;

namespace HotChocolate.PreProcessingExtensions
{
    public class PreProcessingResultsMiddleware
    {
        private static readonly ConcurrentDictionary<MethodInfo, Lazy<bool>> _paramsContextResolverRegistry = new();

        private readonly FieldDelegate _next;

        public PreProcessingResultsMiddleware(FieldDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(IMiddlewareContext context)
        {
            IParamsContext paramsContextFacade = null;

            var field = context.GetSelectedField();
            if (field.Field.ResolverMember is MethodInfo resolverMethod 
                && DoesResolverNeedGraphQLParamsContext(resolverMethod))
            {
                paramsContextFacade = context.InitializeGraphQLParamsContextSafely();
            }

            await _next(context).ConfigureAwait(false);

            var result = context.Result;
            if (paramsContextFacade != null && result is IAmPreProcessedResult)
            {
                //Since sorting is already 'pre-processed' (e.g. by the Resolver) 
                //  we can immediately yield control back to the HotChocolate Pipeline
                paramsContextFacade.SetSortingIsHandled(true);
            }
        }

        private static bool DoesResolverNeedGraphQLParamsContext(MethodInfo resolverMethod)
        {
            var doesResolverNeedParamsContextLazy = _paramsContextResolverRegistry.GetOrAdd(
                resolverMethod,
                new Lazy<bool>(() =>
                {
                    var resolverParams = resolverMethod.GetParameters();
                    return resolverParams.Any(p => p.ParameterType.IsAssignableTo(typeof(IParamsContext)));
                })
            );

            return doesResolverNeedParamsContextLazy.Value;
        }
    }
}
