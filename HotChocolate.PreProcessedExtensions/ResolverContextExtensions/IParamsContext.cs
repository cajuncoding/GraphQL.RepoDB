using HotChocolate.PreProcessedExtensions.Sorting;
using HotChocolate.Resolvers;
using HotChocolate.Types.Pagination;
using System.Collections.Generic;

namespace HotChocolate.PreProcessedExtensions
{
    public interface IParamsContext
    {
        IResolverContext ResolverContext { get; }
        IReadOnlyList<IFieldSelection> SelectionFields { get; }
        IReadOnlyList<string> SelectionNames { get; }
        IReadOnlyList<ISortOrderField> SortArgs { get; }
        CursorPagingArguments PagingArgs { get; }
    }
}