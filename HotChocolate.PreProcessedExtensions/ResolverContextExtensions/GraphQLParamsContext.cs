using HotChocolate.PreProcessedExtensions;
using HotChocolate.PreProcessedExtensions.Pagination;
using HotChocolate.PreProcessedExtensions.Sorting;
using HotChocolate.Resolvers;
using HotChocolate.Types.Pagination;
using System;
using System.Collections.Generic;

namespace HotChocolate.PreProcessedExtensions
{
    public class GraphQLParamsContext : IParamsContext
    {
        protected IResolverContext _resolverContext;
        protected CursorPagingArguments? _pagingArgs;
        protected IReadOnlyList<ISortOrderField> _sortArgs;
        protected IReadOnlyList<IFieldSelection> _selectionFields;
        protected IReadOnlyList<string> _selectionNames;

        public GraphQLParamsContext(IResolverContext resolverContext)
        {
            _resolverContext = resolverContext ?? throw new ArgumentNullException(nameof(resolverContext));
        }

        public virtual IResolverContext ResolverContext => _resolverContext;
        public virtual IReadOnlyList<IFieldSelection> SelectionFields => _selectionFields ??= _resolverContext.GetSelectionsSafely();
        public virtual IReadOnlyList<string> SelectionNames => _selectionNames ??= _resolverContext.GetSelectionNamesSafely();
        public virtual IReadOnlyList<ISortOrderField> SortArgs => _sortArgs ??= _resolverContext.GetSortingArgsSafely();
        //TODO: TEST lazy loading for Struct Type CursorPagingArguments...
        public virtual CursorPagingArguments PagingArgs => _pagingArgs ??= _resolverContext.GetCursorPagingArgsSafely();
    }
}
