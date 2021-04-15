using HotChocolate.PreProcessingExtensions.Pagination;
using HotChocolate.PreProcessingExtensions.Sorting;
using HotChocolate.Resolvers;
using HotChocolate.Types.Pagination;
using System;
using System.Collections.Generic;
using HotChocolate.Language;
using HotChocolate.PreProcessingExtensions.Arguments;

namespace HotChocolate.PreProcessingExtensions
{
    [Flags]
    public enum SelectionNameFlags: short
    {
        SelectedNames = 0,
        DependencyNames = 1,
        All = SelectedNames | DependencyNames
    };

    public interface IParamsContext
    {
        IResolverContext ResolverContext { get; }

        /// <summary>
        /// All of the original GraphQL Argument Names provided; based on GraphQL Schema arguments.
        /// </summary>
        IReadOnlyList<string> AllArgumentSchemaNames { get; }

        /// <summary>
        /// All of the original GraphQL Argument Names provided; based on GraphQL Schema arguments.
        /// </summary>
        IReadOnlyList<IArgumentValue> AllArguments { get; }

        /// <summary>
        /// The underlying Selection Fields, from HotChocolate, for the original GraphQL Selections 
        /// based on GraphQL Schema names.
        /// </summary>
        IReadOnlyList<IPreProcessingSelection> AllSelectionFields { get; }

        /// <summary>
        /// All of the original GraphQL Selections provided; based on GraphQL Schema names.
        /// </summary>
        IReadOnlyList<string> AllSelectionNames { get; }

        /// <summary>
        /// Dependency Links that may have been configured for various fields included in the current Selection;
        /// for example if a dynamic/virtual resolver field is selected it may be dependent on the Id of the Parent,
        /// and if configured properly this will return the Dependency Link details.
        /// </summary>
        IReadOnlyList<PreProcessingDependencyLink> SelectionDependencies { get;  }


        /// <summary>
        /// The underlying Selection Fields of a Specific Type, from HotChocolate, for the 
        /// original GraphQL Selections based on GraphQL Schema names.
        /// Interface Types & Union Types may have multiple objects that share partial common 
        /// fields, along with unique fields, and this will get all selections valid for the specific type.
        /// </summary>
        IReadOnlyList<IPreProcessingSelection> GetSelectionFieldsFor<TObjectType>();

        /// <summary>
        /// GatherSelectionNames based on provided parameters to simplify getting the specifid
        /// </summary>
        /// <param name="flags"></param>
        /// <returns></returns>
        IEnumerable<string> GetSelectionMappedNames(SelectionNameFlags flags = SelectionNameFlags.All);

        /// <summary>
        /// GatherSelectionNames based on provided parameters to simplify getting the specifid
        /// </summary>
        /// <param name="flags"></param>
        /// <returns></returns>
        IEnumerable<string> GetSelectionMappedNamesFor<TObjectType>(SelectionNameFlags flags = SelectionNameFlags.All);

        /// <summary>
        /// The Sort Arguments for the GraphQL request
        /// </summary>
        IReadOnlyList<ISortOrderField> SortArgs { get; }

        /// <summary>
        /// The Cursor Paging arguments for the GrqphQL request; the default paging method is Cursor based paging which 
        /// matches HotChocolate [UsePaging] attribute. Otherwise use OffsetPagingArgs with the [UseOffsetPaging] attribute.
        /// </summary>
        CursorPagingArguments PagingArgs { get; }

        /// <summary>
        /// The Cursor Paging arguments for the GraphQL request; same results as PagingArgs which use Cursor paging as default
        /// via the [UsePaging] HotChocolate attribute.
        /// </summary>
        CursorPagingArguments CursorPagingArgs { get; }

        /// <summary>
        /// The Offset Paging arguments for the GraphQL request (if available); naming convention matches the [UseOffsetPaging]
        /// HotChocolate attribute.
        /// </summary>
        OffsetPagingArguments OffsetPagingArgs { get; }
    }
}