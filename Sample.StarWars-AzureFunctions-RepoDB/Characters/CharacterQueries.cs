using System.Collections.Generic;
using HotChocolate;
using HotChocolate.Data;
using HotChocolate.Types;
using StarWars.Repositories;
using System.Threading.Tasks;
using HotChocolate.RepoDb;
using RepoDb;
using RepoDb.Enumerations;
using StarWars.Characters.DbModels;
using HotChocolate.Types.Pagination;
using HotChocolate.ResolverProcessingExtensions;

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
        public async Task<ICharacter> GetHeroAsync(
            [Service] ICharacterRepository repository,
            Episode episode
        )
        {
            return await repository.GetHeroAsync(episode);
        }

        /// <summary>
        /// Gets all character.
        /// </summary>
        /// <param name="repository"></param>
        /// <param name="graphQLParams"></param>
        /// <returns>The character.</returns>
        [UsePaging]
        //[UseFiltering]
        [UseSorting]
        [GraphQLName("characters")]
        public async Task<Connection<ICharacter>> GetCharactersPaginatedAsync(
            [Service] ICharacterRepository repository,
            //THIS is now injected by Pre-Processed extensions middleware...
            [GraphQLParams] IParamsContext graphQLParams
        )
        {
            var repoDbParams = new GraphQLRepoDbMapper<CharacterDbModel>(graphQLParams);

            //********************************************************************************
            //Get the data from the database via our lower level data access repository class.
            //NOTE: Selections (e.g. Projections), SortFields, PagingArgs are all pushed
            //       down to the Repository (and underlying Database) layer.
            var charactersSlice = await repository.GetCursorPagedCharactersAsync(
                repoDbParams.GetSelectFields(),
                repoDbParams.GetSortOrderFields(),
                repoDbParams.GetCursorPagingParameters()
            );

            //With a valid Page/Slice we can return a PreProcessed Cursor Result so that
            //  it will not have additional post-processing in the HotChocolate pipeline!
            //NOTE: Filtering can be applied but ONLY to the results we are now returning;
            //       Because this would normally be pushed down to the Sql Database layer.
            return charactersSlice.ToGraphQLConnection();
            //********************************************************************************
        }

        /// <summary>
        /// Gets all character.
        /// </summary>
        /// <param name="repository"></param>
        /// <param name="graphQLParams"></param>
        /// <returns>The character.</returns>
        [UseOffsetPaging]
        //[UseFiltering]
        [UseSorting]
        [GraphQLName("charactersWithOffsetPaging")]
        public async Task<CollectionSegment<ICharacter>> GetCharactersWithOffsetPagingAsync(
            [Service] ICharacterRepository repository,
            //THIS is now injected by Pre-Processed extensions middleware...
            [GraphQLParams] IParamsContext graphQLParams
        )
        {
            var repoDbParams = new GraphQLRepoDbMapper<CharacterDbModel>(graphQLParams);

            //********************************************************************************
            //Get the data from the database via our lower level data access repository class.
            //NOTE: Selections (e.g. Projections), SortFields, PagingArgs are all pushed
            //       down to the Repository (and underlying Database) layer.
            var charactersPage = await repository.GetOffsetPagedCharactersAsync(
                repoDbParams.GetSelectFields(),
                repoDbParams.GetSortOrderFields(),
                repoDbParams.GetOffsetPagingParameters()
            );

            //With a valid Page/Slice we can return a PreProcessed Cursor Result so that
            //  it will not have additional post-processing in the HotChocolate pipeline!
            //NOTE: Filtering can be applied but ONLY to the results we are now returning;
            //       Because this would normally be pushed down to the Sql Database layer.
            return charactersPage.ToGraphQLCollectionSegment();
            //********************************************************************************
        }

        [UsePaging]
        //[UseFiltering]
        [UseSorting]
        [GraphQLName("humans")]
        public async Task<Connection<Human>> GetHumansPaginatedAsync(
            [Service] ICharacterRepository repository,
            //THIS is now injected by Pre-Processed extensions middleware...
            [GraphQLParams] IParamsContext graphQLParams
        )
        {
            var repoDbParams = new GraphQLRepoDbMapper<CharacterDbModel>(graphQLParams);

            //********************************************************************************
            //Get the data and convert to List() to ensure it's an Enumerable
            //  and no longer using IQueryable to successfully simulate 
            //  pre-processed results.
            //NOTE: Selections (e.g. Projections), SortFields, PagingArgs are all pushed
            //       down to the Repository (and underlying Database) layer.
            var charactersSlice = await repository.GetPagedHumanCharactersAsync(
                repoDbParams.GetSelectFields(),
                repoDbParams.GetSortOrderFields() ?? OrderField.Parse(new { Name = Order.Ascending }),
                repoDbParams.GetCursorPagingParameters()
            );

            //With a valid Page/Slice we can return a PreProcessed Cursor Result so that
            //  it will not have additional post-processing in the HotChocolate pipeline!
            //NOTE: Filtering can be applied but ONLY to the results we are now returning;
            //       Because this would normally be pushed down to the Sql Database layer.
            return charactersSlice.ToGraphQLConnection();
            //********************************************************************************
        }

        [UsePaging]
        //[UseFiltering]
        [UseSorting]
        [GraphQLName("droids")]
        public async Task<Connection<Droid>> GetDroidsPaginatedAsync(
            [Service] ICharacterRepository repository,
            //THIS is now injected by Pre-Processed extensions middleware...
            [GraphQLParams] IParamsContext graphQLParams
        )
        {
            var repoDbParams = new GraphQLRepoDbMapper<CharacterDbModel>(graphQLParams);

            //********************************************************************************
            //Get the data and convert to List() to ensure it's an Enumerable
            //  and no longer using IQueryable to successfully simulate 
            //  pre-processed results.
            //NOTE: Selections (e.g. Projections), SortFields, PagingArgs are all pushed
            //       down to the Repository (and underlying Database) layer.
            var charactersSlice = await repository.GetPagedDroidCharactersAsync(
                repoDbParams.GetSelectFields(),
                repoDbParams.GetSortOrderFields() ?? OrderField.Parse(new { Name = Order.Ascending }),
                repoDbParams.GetCursorPagingParameters()
            );

            //With a valid Page/Slice we can return a PreProcessed Cursor Result so that
            //  it will not have additional post-processing in the HotChocolate pipeline!
            //NOTE: Filtering can be applied but ONLY to the results we are now returning;
            //       Because this would normally be pushed down to the Sql Database layer.
            return charactersSlice.ToGraphQLConnection();
            //********************************************************************************
        }

        [UseSorting]
        [GraphQLName("allCharacters")]
        public async Task<IEnumerable<ICharacter>> GetAllCharactersAsync(
            [Service] ICharacterRepository repository,
            [GraphQLParams] IParamsContext graphQLParams
        )
        {
            var repoDbParams = new GraphQLRepoDbMapper<CharacterDbModel>(graphQLParams);

            var sortedCharacters = await repository.GetAllSortedCharactersAsync(
                selectFields: repoDbParams.GetSelectFields(), 
                sortFields: repoDbParams.GetSortOrderFields()
            );

            graphQLParams.SetSortingIsHandled();
            return sortedCharacters;
        }

        /// <summary>
        /// Gets all Characters for the specified Id values.
        /// </summary>
        /// <param name="ids">The ids of the human to retrieve.</param>
        /// <param name="repository"></param>
        /// <returns>The character.</returns>
        public async Task<IEnumerable<ICharacter>> GetCharactersByIdAsync(
            [Service] ICharacterRepository repository,
            [GraphQLParams] IParamsContext graphQLParams,
            int[] ids
        )
        {
            var repoDbParams = new GraphQLRepoDbMapper<CharacterDbModel>(graphQLParams);
            var characters = await repository.GetCharactersByIdAsync(repoDbParams.GetSelectFields(), ids);
            return characters;
        }

        /// <summary>
        /// Search across all Character types via Union of results in GraphQL.
        /// </summary>
        /// <param name="text"></param>
        /// <param name="repository"></param>
        /// <returns></returns>
        public async Task<IEnumerable<ISearchResult>> SearchAsync(
            string text,
            [Service] ICharacterRepository repository
        )
        {
            var searchResults = await repository.SearchAsync(text);
            return searchResults;
        }


    }
}
