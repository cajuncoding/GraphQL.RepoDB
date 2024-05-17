using System.Collections.Generic;
using HotChocolate;
using HotChocolate.Data;
using HotChocolate.ResolverProcessingExtensions;
using HotChocolate.Types;
using StarWars.Repositories;
using System.Linq;
using HotChocolate.RepoDb.InMemoryPaging;
using HotChocolate.ResolverProcessingExtensions.Sorting;
using HotChocolate.Types.Pagination;

namespace StarWars.Characters
{
    [ExtendObjectType("Query")]
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
        public Connection<ICharacter> GetCharacters(
            [Service] ICharacterRepository repository,
            //THIS is now injected by the Resolver Processing Extensions middleware...
            [GraphQLParams] IParamsContext graphQLParams
        )
        {
            //********************************************************************************
            //Get the data and convert to List() to ensure it's an Enumerable
            //  and no longer using IQueryable to successfully simulate 
            //  pre-processed results.
            var characters = repository.GetCharacters().AsEnumerable();
            
            //Perform some pre-processed Sorting & Then Paging!
            //This could be done in a lower repository or pushed to the Database!
            var sortedCharacters = characters.SortDynamically(graphQLParams.SortArgs);
            var slicedCharacters = sortedCharacters.SliceAsCursorPage(graphQLParams.PagingArgs);

            //With a valid Page/Slice we can return a ResolverProcessed Cursor Result so that
            //  it will not have additional post-processing in the HotChocolate pipeline!
            //NOTE: Filtering will be applied but ONLY to the results we are now returning;
            //       Because this would normally be pushed down to the Sql Database layer.
            return slicedCharacters.ToGraphQLConnection();
            //********************************************************************************
        }

        /// <summary>
        /// Gets all character.
        /// </summary>
        /// <param name="repository"></param>
        /// <returns>The character.</returns>
        [UseOffsetPaging]
        [UseFiltering]
        [UseSorting]
        public CollectionSegment<ICharacter> GetCharactersWithOffsetPaging(
            [Service] ICharacterRepository repository,
            //THIS is now injected by the Resolver Processing Extensions middleware...
            [GraphQLParams] IParamsContext graphQLParams
        )
        {
            //********************************************************************************
            //Get the data and convert to List() to ensure it's an Enumerable
            //  and no longer using IQueryable to successfully simulate 
            //  pre-processed results.
            var characters = repository.GetCharacters().AsEnumerable();

            //Perform some pre-processed Sorting & Then Paging!
            //This could be done in a lower repository or pushed to the Database!
            var sortedCharacters = characters.SortDynamically(graphQLParams.SortArgs);
            var slicedCharacters = sortedCharacters.SliceAsOffsetPage(graphQLParams.OffsetPagingArgs);

            //With a valid Page/Slice we can return a ResolverProcessed Offset Paging Result so that
            //  it will not have additional post-processing in the HotChocolate pipeline!
            //NOTE: Filtering will be applied but ONLY to the results we are now returning;
            //       Because this would normally be pushed down to the Sql Database layer.
            return slicedCharacters.ToGraphQLCollectionSegment();
            ////********************************************************************************
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
