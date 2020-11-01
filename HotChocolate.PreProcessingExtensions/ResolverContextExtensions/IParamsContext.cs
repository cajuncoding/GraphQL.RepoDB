using HotChocolate.PreProcessingExtensions.Pagination;
using HotChocolate.PreProcessingExtensions.Sorting;
using HotChocolate.Resolvers;
using HotChocolate.Types.Pagination;
using System;
using System.Collections.Generic;

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
        /// The underlying Selection Fields, from HotChocolate, for the original GraphQL Selections 
        /// based on GraphQL Schema names.
        /// </summary>
        IReadOnlyList<IPreProcessingSelection> AllSelectionFields { get; }

        /// <summary>
        /// The original GraphQL Selections based on GraphQL Schema names.
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
        /// Interface Types & Union Types may have multipel objects that share parital common 
        /// fields, along with unique fields, and this will get all selections valid for the specific type.
        /// </summary>
        IReadOnlyList<IPreProcessingSelection> GetSelectionFieldsFor<TObjectType>();

        /// <summary>
        /// GatherSelectionNames based on provided parameters to simplify getting the specifid
        /// </summary>
        /// <param name="includeDependencyNames"></param>
        /// <returns></returns>
        IEnumerable<string> GetSelectionMappedNames(SelectionNameFlags flags = SelectionNameFlags.All);

        /// <summary>
        /// GatherSelectionNames based on provided parameters to simplify getting the specifid
        /// </summary>
        /// <param name="includeDependencyNames"></param>
        /// <returns></returns>
        IEnumerable<string> GetSelectionMappedNamesFor<TObjectType>(SelectionNameFlags flags = SelectionNameFlags.All);

        /// <summary>
        /// The Sort Arguments for the GraphQL request
        /// </summary>
        IReadOnlyList<ISortOrderField> SortArgs { get; }

        /// <summary>
        /// The Paging arguments for the GrqphQL request
        /// </summary>
        CursorPagingArguments? CursorPagingArgs { get; }

        /// <summary>
        /// The Offset Paging arguments for the GrqphQL request (if available)
        /// </summary>
        OffsetPagingArguments? OffsetPagingArgs { get; }

        /// <summary>
        /// The default paging method is Cursor based paging which matches HotChocolate UsePaging default;
        ///     use OffsetPagingArgs otherwise.
        /// </summary>
        CursorPagingArguments PagingArgs { get; }
    }
}