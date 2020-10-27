using HotChocolate.PreProcessedExtensions.Sorting;
using HotChocolate.Resolvers;
using HotChocolate.Types.Pagination;
using System.Collections.Generic;

namespace HotChocolate.PreProcessedExtensions
{
    public interface IParamsContext
    {
        IResolverContext ResolverContext { get; }

        /// <summary>
        /// The underlying Selection Fields, from HotChocolate, for the original GraphQL Selections 
        /// based on GraphQL Schema names.
        /// </summary>
        IReadOnlyList<IPreProcessingSelection> AllSelectionFields { get; }

        /// <summary>
        /// The underlying Selection Fields of a Specific Type, from HotChocolate, for the 
        /// original GraphQL Selections based on GraphQL Schema names.
        /// Interface Types & Union Types may have multipel objects that share parital common 
        /// fields, along with unique fields, and this will get all selections valid for the specific type.
        /// </summary>
        IReadOnlyList<IPreProcessingSelection> SelectionFieldsFor<TObjectType>();

        /// <summary>
        /// The original GraphQL Selections based on GraphQL Schema names.
        /// </summary>
        IReadOnlyList<string> SelectionNames { get; }

        /// <summary>
        /// The actual class property/member names as mapped from the GraphQL Schema Selections;
        /// this may be different than the GraphQL Schema names based on GraphQL name mappings.
        /// </summary>
        IReadOnlyList<string> SelectionMemberNames { get; }

        /// <summary>
        /// The selections for a Specific Object Type based on original GraphQL Schema names.
        /// Interface Types & Union Types may have multipel objects that share parital common 
        /// fields, along with unique fields, and this will get all selections valid for the specific type.
        /// </summary>
        IReadOnlyList<string> SelectionNamesFor<TObjectType>();

        /// <summary>
        /// The acutal class property/member names for a Specific Object Type based as mapped from GraphQL Schema Selections;
        /// this may be different than the GraphQL Schema names based on GraphQL name mappings.
        /// Interface Types & Union Types may have multipel objects that share parital common 
        /// fields, along with unique fields, and this will get all selections valid for the specific type.
        /// </summary>
        IReadOnlyList<string> SelectionMemberNamesFor<TObjectType>();
        
        /// <summary>
        /// The Sort Arguments for the GraphQL request
        /// </summary>
        IReadOnlyList<ISortOrderField> SortArgs { get; }

        /// <summary>
        /// The Paging arguments for the GrqphQL request
        /// </summary>
        CursorPagingArguments PagingArgs { get; }
    }
}