using System.Collections.Generic;
using HotChocolate;
using HotChocolate.Data;
using HotChocolate.PreProcessingExtensions;
using HotChocolate.Types;
using HotChocolate.Types.Relay;
using StarWars.Repositories;
using System.Linq;
using System.ComponentModel;
using HotChocolate.PreProcessingExtensions.Sorting;
using HotChocolate.PreProcessingExtensions.Pagination;
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
        public PreProcessedCursorSlice<ICharacter> GetCharacters(
            [Service] ICharacterRepository repository,
            //THIS is now injected by Pre-Processed extensions middleware...
            [GraphQLParams] IParamsContext graphQLParams
        )
        {
            //********************************************************************************
            //Get the data and convert to List() to ensure it's an Enumerable
            //  and no longer using IQueryable to successfully simulate 
            //  pre-processed results.
            var characters = repository.GetCharacters().ToList();
            
            //Perform some pre-processed Sorting & Then Paging!
            //This could be done in a lower repository or pushed to the Database!
            var sortedCharacters = characters.SortDynamically(graphQLParams.SortArgs);
            var slicedCharacters = sortedCharacters.SliceAsCursorPage(graphQLParams.PagingArgs);

            //With a valid Page/Slice we can return a PreProcessed Cursor Result so that
            //  it will not have additional post-processing in the HotChocolate pipeline!
            //NOTE: Filtering will be applied but ONLY to the results we are now returning;
            //       Because this would normally be pushed down to the Sql Database layer.
            return new PreProcessedCursorSlice<ICharacter>(slicedCharacters);
            //********************************************************************************
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
}
