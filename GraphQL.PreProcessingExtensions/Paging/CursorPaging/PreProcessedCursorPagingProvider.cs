using System;
using System.Reflection;
using HotChocolate.Internal;
using HotChocolate.Types.Pagination;

namespace HotChocolate.PreProcessingExtensions.Pagination
{
    /// <summary>
    /// A Paging Provider for HotChocolate that provides No Operation during the execution
    ///     of the Pipeline; because all processing was already completed within the Resolver method!
    /// 
    /// This enables Pagination handling, dynamic Schema creation, dynamic
    ///     Connection class creation, etc. while allowing the actual pagination logic to be implemented
    ///     at a lower layer in the Resolvers or below (e.g. in Service or Repository classes that may 
    ///     already exist) so that the results are unaffected by the HotChocolate pipeline; because they
    ///     have already been pre-processed.
    /// </summary>
    [Obsolete("It is now Recommended to simply use ToGraphQLConnection() for directly returning a GraphQL Connection from Hot Chocolate Resolvers instead;"
              + " since HC has resolved internal bug(s), a Connection result will offer improved performance. This will likely be removed in future release"
              + " (especially once the new Paging features are available in a later version of v13.")]
    public class PreProcessedCursorPagingProvider : CursorPagingProvider
    {
        //BBernard
        //This allows one single instance to be used for Generic types that are not known a compile time,
        //  because of the dynamic Schema & Classes gen processes provided by HotChocolate OOTB Sorting/Paging
        //  functionality.
        //NOTE: Borrowed from QueryableCursorPagingProvider implementation of HotChocolate.
        private static readonly MethodInfo _createHandler = typeof(PreProcessedCursorPagingProvider).GetMethod(
            nameof(CreateHandlerInternal), BindingFlags.Static | BindingFlags.NonPublic
        )!;


        /// <summary>
        /// BBernard
        /// This determines if this Paging Provider can handle the request. Here we validate that we
        /// are using PagedResult and handle those only as NoOp-Paged processing, leaving other requests
        /// to be handled by Default implementations.
        /// NOTE: Borrowed from QueryableCursorPagingProvider implementation of HotChocolate.
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public override bool CanHandle(IExtendedType source)
        {
            if (source is null)
                throw new ArgumentNullException(nameof(source));

            bool isPreProcessedResult = source.Type.IsDerivedFromGenericParent(typeof(IPreProcessedCursorSlice<>));
            return source.IsArrayOrList && isPreProcessedResult;
        }

        /// <summary>
        /// BBernard
        /// This is the dynamic initialization of the Handler based on Generic type provided at Runtime.
        ///     HotChocolate handles a lot of dynamic Schema and Class type generation, therefore this must be
        ///     completely dynamic at runtime to support.
        /// NOTE: Borrowed from QueryableCursorPagingProvider implementation of HotChocolate.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        protected override CursorPagingHandler CreateHandler(IExtendedType source, PagingOptions options)
        {
            if (source is null)
                throw new ArgumentNullException(nameof(source));

            return (CursorPagingHandler)_createHandler
                .MakeGenericMethod(source.ElementType?.Source ?? source.Source)
                .Invoke(null, new object[] { options })!;
        }

        private static PreProcessedCursorPagingHandler<TEntity> CreateHandlerInternal<TEntity>(
            PagingOptions options) => new PreProcessedCursorPagingHandler<TEntity>(options);
    }
}
