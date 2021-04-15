using System;
using System.Reflection;
using HotChocolate.Internal;
using HotChocolate.Types.Pagination;

namespace HotChocolate.PreProcessingExtensions.Pagination
{
    /// <summary>
    /// A Paging Provider for HotChocolate that provides No Operation during the execution
    /// of the Pipeline; because all processing was already completed within the Resolver method!
    /// This enables Pagination handling, dynamic Schema creation, dynamic
    /// Connection class creation, etc. while allowing the actual pagination logic to be implemented
    /// at a lower layer in the Resolvers or below (e.g. in Service or Repository classes that may 
    /// already exist) so that the results are unaffected by the HotChocolate pipeline; because they
    /// have already been pre-processed.
    /// </summary>
    public class PreProcessedOffsetPagingProvider : OffsetPagingProvider
    {
        //BBernard
        //This allows one single instance to be used for Generic types that are not known a compile time,
        //  because of the dynamic Schema & Classes gen processes provided by HotChocolate OOTB Sorting/Paging
        //  functionality.
        //NOTE: Borrowed from QueryableOffsetPagingProvider implementation of HotChocolate.
        private static readonly MethodInfo _createHandler = typeof(PreProcessedOffsetPagingProvider).GetMethod(
            nameof(CreateHandlerInternal), BindingFlags.Static | BindingFlags.NonPublic
        )!;


        /// <summary>
        /// BBernard
        /// This determines if this Paging Provider can handle the request. Here we validate that we
        /// are using PagedResult and handle those only as NoOp-Paged processing, leaving other requests
        /// to be handled by Default implementations.
        /// NOTE: Borrowed from QueryableOffsetPagingProvider implementation of HotChocolate.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public override bool CanHandle(IExtendedType source)
        {
            if (source is null)
                throw new ArgumentNullException(nameof(source));

            bool isPreProcessedResult = source.Type.IsDerivedFromGenericParent(typeof(IPreProcessedOffsetPageResults<>));
            return source.IsArrayOrList && isPreProcessedResult;
        }

        /// <summary>
        /// BBernard
        /// This is the dynamic initialization of the Handler based on Generic type provided at Runtime.
        ///     HotChocolate handles a lot of dynamic Schema and Class type generation, therefore this must be
        ///     completely dynamic at runtime to support.
        /// NOTE: Borrowed from QueryableOffsetPagingProvider implementation of HotChocolate.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        protected override OffsetPagingHandler CreateHandler(IExtendedType source, PagingOptions options)
        {
            if (source is null)
                throw new ArgumentNullException(nameof(source));

            return (OffsetPagingHandler)_createHandler
                .MakeGenericMethod(source.ElementType?.Source ?? source.Source)
                .Invoke(null, new object[] { options })!;
        }

        private static PreProcessedOffsetPagingHandler<TEntity> CreateHandlerInternal<TEntity>(
            PagingOptions options) => new PreProcessedOffsetPagingHandler<TEntity>(options);
    }
}
