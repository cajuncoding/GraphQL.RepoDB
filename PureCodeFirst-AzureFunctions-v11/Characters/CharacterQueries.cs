using System.Collections.Generic;
using HotChocolate;
using HotChocolate.Data;
using HotChocolate.PreProcessedExtensions;
using HotChocolate.Types;
using HotChocolate.Types.Relay;
using StarWars.Repositories;
using System.Linq;
using System.ComponentModel;
using HotChocolate.PreProcessedExtensions.Sorting;
using HotChocolate.PreProcessedExtensions.Pagination;
using Microsoft.Extensions.DependencyInjection;
using HotChocolate.Types.Pagination;

namespace StarWars.Characters
{
    [ExtendObjectType(Name = "Query")]
    public class CharacterQueries
    {
        /// <summary>
        /// Retrieve a hero by a particular Star Wars episode.
        /// </summary>
        /// <param name="episode">The episode to look up by.</param>
        /// <param name="repository"></param>
        /// <returns>The character.</returns>
        public ICharacter GetHero(
            Episode episode,
            [Service] ICharacterRepository repository) =>
            repository.GetHero(episode);

        /// <summary>
        /// Gets all character.
        /// </summary>
        /// <param name="repository"></param>
        /// <returns>The character.</returns>
        [UsePaging]
        [UseFiltering]
        [UseSorting]
        public PreProcessedCursorSliceResults<ICharacter> GetCharacters(
            [Service] ICharacterRepository repository,
            //THIS is now injected by Pre-Processed extensions middleware...
            [GraphQLParams] IParamsContext graphQLParams
        )
        {
            var characters = repository.GetCharacters().ToList();
            
            //Perform some pre-processed Sorting!
            var sortedCharacters = characters.SortDynamically(graphQLParams.SortArgs);
            var slicedCharacters = sortedCharacters.PaginateAsCursorSlice(graphQLParams.PagingArgs);

            //With a valid Page/Slice we can return a PreProcessed Cursor Result so that
            //  it will not have additional post-processing in the HotChocolate pipeline!
            return new PreProcessedCursorSliceResults<ICharacter>(slicedCharacters);
        }

        /// <summary>
        /// Gets a character by it`s id.
        /// </summary>
        /// <param name="ids">The ids of the human to retrieve.</param>
        /// <param name="repository"></param>
        /// <returns>The character.</returns>
        public IEnumerable<ICharacter> GetCharacter(
            int[] ids,
            [Service] ICharacterRepository repository) =>
            repository.GetCharacters(ids);

        public IEnumerable<ISearchResult> Search(
            string text,
            [Service] ICharacterRepository repository) =>
            repository.Search(text);

    }

    public static class CustomExtensionsFor
    {
        public static IOrderedEnumerable<T> SortDynamically<T>(this IEnumerable<T> items, IReadOnlyList<ISortOrderField> sortArgs)
        {
            //Map the Sort by property string names to actual Property Descriptors
            //for dynamic processign...
            var propCollection = TypeDescriptor.GetProperties(typeof(T));
            var sortGetters = sortArgs?.Select(s => new {
                SortArg = s,
                Getter = propCollection.Find(s.FieldName, true)
            });

            IOrderedEnumerable<T> orderedItems = null;
            foreach (var sort in sortGetters)
            {
                if (orderedItems == null)
                {
                    orderedItems = sort.SortArg.IsAscending()
                        ? items.OrderBy(c => sort.Getter.GetValue(c))
                        : items.OrderByDescending(c => sort.Getter.GetValue(c));
                }
                else
                {
                    orderedItems = sort.SortArg.IsAscending()
                        ? orderedItems.ThenBy(c => sort.Getter.GetValue(c))
                        : orderedItems.ThenByDescending(c => sort.Getter.GetValue(c));
                }
            }

            return orderedItems;
        }

        public static ICursorPageSlice<T> PaginateAsCursorSlice<T>(this IEnumerable<T> items, CursorPagingArguments pagingArgs)
            where T: class
        {
            
            var after = pagingArgs.After != null
                ? IndexEdge<string>.DeserializeCursor(pagingArgs.After)
                : 0;

            var before = pagingArgs.Before != null
                ? IndexEdge<string>.DeserializeCursor(pagingArgs.Before)
                : 0;

            //FIRST log the index of all items in the list BEFORE slicing, 
            // as these indexes are the Cursor Indexes for paging up/down the entire list...
            //ICursorResult is the Decorator around the Entity Models.
            int index = 0;
            IEnumerable<ICursorResult<T>> slice = items.Select(c => new CursorResult<T>(c, ++index)).ToList();
            int totalCount = slice.Count();

            //Now we can extract the Slice requested.
            if (after > 0 && slice.Count() > after)
            {
                slice = slice.Skip(after);
            }

            if(pagingArgs.First > 0 && slice.Count() > pagingArgs.First)
            {
                slice = slice.Take(pagingArgs.First.Value);
            }

            if (before > 0 && slice.Count() > before)
            {
                slice = slice.SkipLast(before);
            }

            if (pagingArgs.Last > 0 && slice.Count() > pagingArgs.Last)
            {
                slice = slice.TakeLast(pagingArgs.Last.Value);
            }

            //Wrap all results into a PagedCursor Slice result wit Total Count...
            var cursorPageSlice = new CursorPageSlice<T>(slice, totalCount);
            return cursorPageSlice;
        }
    }
}
