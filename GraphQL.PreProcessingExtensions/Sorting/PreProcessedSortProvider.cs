//using HotChocolate.Data;
//using HotChocolate.Data.Sorting;
//using HotChocolate.Data.Sorting.Expressions;
//using HotChocolate.Resolvers;
//using System;
//using System.Threading.Tasks;

//namespace HotChocolate.PreProcessingExtensions.Sorting
//{
//    public class PreProcessedSortProvider : QueryableSortProvider
//    {
//        public PreProcessedSortProvider()
//            : this(x => x.AddDefaultFieldHandlers())
//        {
//        }

//        public PreProcessedSortProvider(
//            Action<ISortProviderDescriptor<QueryableSortContext>> configure)
//            : base(configure)
//        {
//        }

//        /// <summary>
//        /// Overrides the default behavior of the OOTB QueryableSortProvider so that we can intercept
//        ///  and achieve "right of first refusal" for special case handling of results that are already
//        ///  pre-processed!
//        ///  
//        /// NOTE: As a 'Convention' this works differently than the Paging pipeline which takes in a provider
//        ///     via DI, and checks support via the CanHandle() 'right of first refusal' interface.  Here the
//        ///     SortProvider is a single instance that we configure with some Dispatch behavior that overrides
//        ///     when needed but defaults to original behavior when not.
//        /// </summary>
//        /// <typeparam name="TEntityType"></typeparam>
//        /// <param name="argumentName"></param>
//        /// <returns></returns>
//        public override FieldMiddleware CreateExecutor<TEntityType>(NameString argumentName)
//        {
//            var queryableExecutor = base.CreateExecutor<TEntityType>(argumentName);

//            //Construct the FieldMiddleware delegate dynamically:
//            //  - At runtime the HotChocolate middleware will call the delegate passing in 'next',
//            //  - Which in turn is a delegate that will be called passing in 'context',
//            //  - Which we then wire up to our handler 'ExecuteAsync' whereby we pass down both next & context
//            //      for processing at request runtime.
//            return next => (context) => ExecuteAsync(next, context);

//            //Create a Delegate for Dispatching the right processing based on the type at Runtime!
//            //  this is the Handler of the 'FieldMiddleware' delegate that is returned and Cached
//            //  
//            async ValueTask ExecuteAsync(FieldDelegate next, IMiddlewareContext context)
//            {
//                // The SortConvention will intercept the request before the Resolvers, which are later
//                //  in the HotChocolate Pipeline.  Therefore we must FIRST let the pipeline run and produce a result.
//                await next(context).ConfigureAwait(false);

//                //If the result is a pre-processed result, we assume that Sorting was handled previously 
//                if (context.Result is IAmPreProcessedResult)
//                {
//                    //Since sorting is already 'pre-processed' (e.g. by the Resolver) 
//                    //  we can immediately yield control back to the HotChocolate Pipeline
//                    return;
//                }
//                //Otherwise, we dispatch back to the original Pipeline for Default behavior!
//                else
//                {
//                    //NOTE: This does not create an infinite loop (even though the QueryableExecutor will 
//                    //  also await the next() method, because as an Async process it may have already completed;
//                    //  async Tasks will only execute once though they can be awaited multiple times.
//                    queryableExecutor(next);
//                }
//            }
//        }
//    }
//}
