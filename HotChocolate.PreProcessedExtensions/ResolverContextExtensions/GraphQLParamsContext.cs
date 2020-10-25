using HotChocolate.PreProcessedExtensions;
using HotChocolate.PreProcessedExtensions.Pagination;
using HotChocolate.PreProcessedExtensions.Sorting;
using HotChocolate.Resolvers;
using HotChocolate.Types.Pagination;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HotChocolate.PreProcessedExtensions
{
    public class GraphQLParamsContext : IParamsContext
    {
        protected IResolverContext _resolverContext;
        protected CursorPagingArguments? _pagingArgs;
        protected IReadOnlyList<ISortOrderField> _sortArgs;
        protected IReadOnlyList<IPreProcessingSelection> _selectionFields;

        public GraphQLParamsContext(IResolverContext resolverContext)
        {
            _resolverContext = resolverContext ?? throw new ArgumentNullException(nameof(resolverContext));
        }

        public virtual IResolverContext ResolverContext => _resolverContext;
        
        public virtual IReadOnlyList<IPreProcessingSelection> AllSelectionFields 
            => _selectionFields ??= _resolverContext.GetPreProcessingSelections();

        public virtual IReadOnlyList<IPreProcessingSelection> SelectionFieldsFor<TObjectType>()
            => AllSelectionFields?.Where(s => typeof(TObjectType).IsAssignableFrom(s.GraphQLObjectType.RuntimeType)).ToList();

        public virtual IReadOnlyList<string> SelectionNamesFor<TObjectType>()
            //When retrieving only Names, we should take all Distinct values...
            => SelectionFieldsFor<TObjectType>()?.Select(s=> s.SelectionName).Distinct().ToList();

        public virtual IReadOnlyList<string> SelectionNames
            //When retrieving only Names, we should take all Distinct values...
            => AllSelectionFields?.Select(s => s.SelectionName).Distinct().ToList();

        public virtual IReadOnlyList<ISortOrderField> SortArgs 
            => _sortArgs ??= _resolverContext.GetSortingArgsSafely();
        
        //TODO: TEST lazy loading for Struct Type CursorPagingArguments...
        public virtual CursorPagingArguments PagingArgs 
            => _pagingArgs ??= _resolverContext.GetCursorPagingArgsSafely();
    }
}
