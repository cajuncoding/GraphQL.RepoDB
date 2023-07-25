using HotChocolate.Data.Sorting;
using HotChocolate.PreProcessingExtensions.Pagination;
using HotChocolate.Resolvers;
using System;
using System.Threading.Tasks;

namespace HotChocolate.PreProcessingExtensions
{
    public class PreProcessingResultsMiddleware
    {
        private readonly FieldDelegate _next;

        public PreProcessingResultsMiddleware(FieldDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(IMiddlewareContext context)
        {
            var paramsContextFacade = new GraphQLParamsContext(context);
            context.SetLocalState(nameof(GraphQLParamsContext), paramsContextFacade);
            
            await _next(context).ConfigureAwait(false);

            var result = context.Result;
            if (result is IAmPreProcessedResult)
            {
                //Since sorting is already 'pre-processed' (e.g. by the Resolver) 
                //  we can immediately yield control back to the HotChocolate Pipeline
                paramsContextFacade.SetSortingIsHandled(true);
            }
        }
    }
}
