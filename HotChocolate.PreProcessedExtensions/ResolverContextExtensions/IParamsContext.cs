using HotChocolate.PreProcessedExtensions.Sorting;
using HotChocolate.Resolvers;
using HotChocolate.Types.Pagination;
using System.Collections.Generic;

namespace HotChocolate.PreProcessedExtensions
{
    public interface IParamsContext
    {
        IResolverContext ResolverContext { get; }

        IReadOnlyList<IPreProcessingSelection> AllSelectionFields { get; }
        IReadOnlyList<IPreProcessingSelection> SelectionFieldsFor<TObjectType>();

        IReadOnlyList<string> SelectionNames { get; }
        IReadOnlyList<string> SelectionNamesFor<TObjectType>();


        IReadOnlyList<ISortOrderField> SortArgs { get; }
        CursorPagingArguments PagingArgs { get; }
    }
}