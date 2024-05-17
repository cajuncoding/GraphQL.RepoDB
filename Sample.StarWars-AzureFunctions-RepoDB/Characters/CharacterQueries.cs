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
        /// Gets all character using Cursor Paging (Default / Recommended).
        /// </summary>
        /// <param name="repository"></param>
        /// <param name="repoDbGraphQL"></param>
        /// <returns>The character.</returns>
        [UsePaging]
        //[UseFiltering]
        [UseSorting]
        [GraphQLName("characters")]
        public async Task<Connection<ICharacter>> GetCharactersPaginatedAsync(
            [Service] ICharacterRepository repository,
            //THIS is now injected by the Resolver Processing Extensions middleware...
            [GraphQLRepoDbMapper] GraphQLRepoDbMapper<CharacterDbModel> repoDbGraphQL
        )
        {
            //********************************************************************************
            //Get the data from the database via our lower level data access repository class.
            //NOTE: Selections (e.g. Projections), SortFields, PagingArgs are all pushed
            //       down to the Repository (and underlying Database) layer.
            var charactersSlice = await repository.GetCursorPagedCharactersAsync(
                repoDbGraphQL.GetSelectFields(),
                repoDbGraphQL.GetSortOrderFields(),
                repoDbGraphQL.GetCursorPagingParameters()
            );

            //With a valid Cursor Page we can return a ready to use Connection Result so that
            //  it will not have additional post-processing in the HotChocolate pipeline!
            //NOTE: Filtering can be applied but ONLY to the results we are now returning;
            //       because filtering should be pushed down to the Sql Database layer.
            return charactersSlice.ToGraphQLConnection();
            //********************************************************************************
        }

        /// <summary>
        /// Gets all characters using Offset Paging
        /// </summary>
        /// <param name="repository"></param>
        /// <param name="repoDbGraphQL"></param>
        /// <returns>The character.</returns>
        [UseOffsetPaging]
        //[UseFiltering]
        [UseSorting]
        [GraphQLName("charactersWithOffsetPaging")]
        public async Task<CollectionSegment<ICharacter>> GetCharactersWithOffsetPagingAsync(
            [Service] ICharacterRepository repository,
            //THIS is now injected by the Resolver Processing Extensions middleware...
            [GraphQLRepoDbMapper] GraphQLRepoDbMapper<CharacterDbModel> repoDbGraphQL
        )
        {
            //********************************************************************************
            //Get the data from the database via our lower level data access repository class.
            //NOTE: Selections (e.g. Projections), SortFields, PagingArgs are all pushed
            //       down to the Repository (and underlying Database) layer.
            var charactersPage = await repository.GetOffsetPagedCharactersAsync(
                repoDbGraphQL.GetSelectFields(),
                repoDbGraphQL.GetSortOrderFields(),
                repoDbGraphQL.GetOffsetPagingParameters()
            );

            //With a valid Cursor Page we can return a ready to use Collection Segment Result so that
            //  it will not have additional post-processing in the HotChocolate pipeline!
            //NOTE: Filtering can be applied but ONLY to the results we are now returning;
            //       because filtering should be pushed down to the Sql Database layer.
            return charactersPage.ToGraphQLCollectionSegment();
            //********************************************************************************
        }

        /// <summary>
        /// Get all Humans (not Droids) using Cursor Paging.
        /// </summary>
        /// <param name="repository"></param>
        /// <param name="repoDbGraphQL"></param>
        /// <returns></returns>
        [UsePaging]
        //[UseFiltering]
        [UseSorting]
        [GraphQLName("humans")]
        public async Task<Connection<Human>> GetHumansPaginatedAsync(
            [Service] ICharacterRepository repository,
            //THIS is now injected by the Resolver Processing Extensions middleware...
            [GraphQLRepoDbMapper] GraphQLRepoDbMapper<CharacterDbModel> repoDbGraphQL
        )
        {
            //********************************************************************************
            //Get the data and convert to List() to ensure it's an Enumerable
            //  and no longer using IQueryable to successfully simulate 
            //  pre-processed results.
            //NOTE: Selections (e.g. Projections), SortFields, PagingArgs are all pushed
            //       down to the Repository (and underlying Database) layer.
            var charactersSlice = await repository.GetPagedHumanCharactersAsync(
                repoDbGraphQL.GetSelectFields(),
                repoDbGraphQL.GetSortOrderFields() ?? OrderField.Parse(new { Name = Order.Ascending }),
                repoDbGraphQL.GetCursorPagingParameters()
            );

            //With a valid Cursor Page we can return a ready to use Connection Result so that
            //  it will not have additional post-processing in the HotChocolate pipeline!
            //NOTE: Filtering can be applied but ONLY to the results we are now returning;
            //       because filtering should be pushed down to the Sql Database layer.
            return charactersSlice.ToGraphQLConnection();
            //********************************************************************************
        }

        /// <summary>
        /// Get all Droids (not Humans) using Cursor Paging.
        /// </summary>
        /// <param name="repository"></param>
        /// <param name="repoDbGraphQL"></param>
        /// <returns></returns>
        [UsePaging]
        //[UseFiltering]
        [UseSorting]
        [GraphQLName("droids")]
        public async Task<Connection<Droid>> GetDroidsPaginatedAsync(
            [Service] ICharacterRepository repository,
            //NOTE: In this example we use the core IParamsContext and then manually initialize the RepoDb GraphQL Params....
            //THIS is now injected by the Resolver Processing Extensions middleware...
            [GraphQLRepoDbMapper] GraphQLRepoDbMapper<CharacterDbModel> repoDbGraphQL
        )
        {
            //********************************************************************************
            //Get the data and convert to List() to ensure it's an Enumerable
            //  and no longer using IQueryable to successfully simulate 
            //  pre-processed results.
            //NOTE: Selections (e.g. Projections), SortFields, PagingArgs are all pushed
            //       down to the Repository (and underlying Database) layer.
            var charactersSlice = await repository.GetPagedDroidCharactersAsync(
                repoDbGraphQL.GetSelectFields(),
                repoDbGraphQL.GetSortOrderFields() ?? OrderField.Parse(new { Name = Order.Ascending }),
                repoDbGraphQL.GetCursorPagingParameters()
            );

            //With a valid Cursor Page we can return a ready to use Connection Result so that
            //  it will not have additional post-processing in the HotChocolate pipeline!
            //NOTE: Filtering can be applied but ONLY to the results we are now returning;
            //       because filtering should be pushed down to the Sql Database layer.
            return charactersSlice.ToGraphQLConnection();
            //********************************************************************************
        }

        /// <summary>
        /// Get All Characters with custom Sorting (non-paginated).
        /// </summary>
        /// <param name="repository"></param>
        /// <param name="repoDbGraphQL"></param>
        /// <returns></returns>
        [UseSorting]
        [GraphQLName("allCharacters")]
        public async Task<IEnumerable<ICharacter>> GetAllCharactersAsync(
            [Service] ICharacterRepository repository,
            [GraphQLRepoDbMapper] GraphQLRepoDbMapper<CharacterDbModel> repoDbGraphQL
        )
        {
            var sortedCharacters = await repository.GetAllSortedCharactersAsync(
                selectFields: repoDbGraphQL.GetSelectFields(), 
                sortFields: repoDbGraphQL.GetSortOrderFields()
            );

            //The now recommended approach by the HC Core team is to return an IEnumerable<T> results when only Sorting
            //  has been implemented and then set the correct flag on the Resolver Context to let the HC Pipeline know that no
            //  further sort processing should be done -- this can now be done slightly more easily with the convenience method
            //  we now have on the IParamsContext facade (also accessible from the RepoDb GraphQL params).
            //NOTE: The older implementation of returning a ResolverProcessedSortedResults<T> is obsolete and may be removed  in the future.
            repoDbGraphQL.GraphQLParamsContext.SetSortingIsHandled();
            return sortedCharacters;
        }

        /// <summary>
        /// Gets all Characters for the specified Id values.
        /// </summary>
        /// <param name="graphQLParams"></param>
        /// <param name="ids">The ids of the human to retrieve.</param>
        /// <param name="repository"></param>
        /// <returns>The character.</returns>
        public async Task<IEnumerable<ICharacter>> GetCharactersByIdAsync(
            [Service] ICharacterRepository repository,
            //NOTE: In this example we use the core IParamsContext and then manually initialize the RepoDb GraphQL Params....
            [GraphQLParams] IParamsContext graphQLParams,
            int[] ids
        )
        {
            //NOTE: In this example we use the core IParamsContext and then manually initialize the RepoDb GraphQL Params....
            var repoDbGraphQL = new GraphQLRepoDbMapper<CharacterDbModel>(graphQLParams);
            var characters = await repository.GetCharactersByIdAsync(repoDbGraphQL.GetSelectFields(), ids);
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
