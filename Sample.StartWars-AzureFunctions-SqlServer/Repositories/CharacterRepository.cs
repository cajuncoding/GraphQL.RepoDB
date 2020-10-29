//NOTE: Updated to alleviate Visual Studio warning...
#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HotChocolate.PreProcessedExtensions.Pagination;
using Microsoft.Data.SqlClient;
using RepoDb;
using RepoDb.CursorPagination;
using RepoDb.Enumerations;
using StarWars.Characters;
using StarWars.Characters.DbModels;

namespace StarWars.Repositories
{
    public class CharacterRepository : BaseRepository<ICharacter, SqlConnection>, ICharacterRepository
    {
        public static class TableNames
        {
            public const string StarWarsCharacters = "StarWarsCharacters";
        }

        public CharacterRepository(string connectionString)
            : base(connectionString)
        {
        }

        public async Task<IEnumerable<ICharacter>> GetAllSortedCharactersAsync(
            IEnumerable<Field> selectFields,
            IEnumerable<OrderField> sortFields
        )
        {
            var sqlConn = CreateConnection();

            var results = await sqlConn.QueryAsync<CharacterDbModel>(
                where: c => c.Id >= 1000 && c.Id <=2999,
                fields: selectFields,
                orderBy: sortFields
            );

            var mappedResults = MapDbModelsToCharacterModels(results);
            return mappedResults;
        }


        public async Task<ICursorPageSlice<ICharacter>> GetPagedCharactersAsync(
            IEnumerable<Field> selectFields,
            IEnumerable<OrderField> sortFields, 
            IRepoDbCursorPagingParams pagingParams
        )
        {
            var sqlConn = CreateConnection();

            var pageSlice = await sqlConn.GraphQLBatchSliceQueryAsync<CharacterDbModel>(
                orderBy: sortFields,
                fields: selectFields,
                afterCursor: pagingParams.AfterIndex!,
                beforeCursor: pagingParams.BeforeIndex!,
                firstTake: pagingParams.First,
                lastTake: pagingParams.Last
            );

            var convertedSlice = pageSlice.AsMappedType(r => MapDbModelToCharacterModel(r));
            return convertedSlice;
        }

        public async Task<IEnumerable<ICharacter>> GetCharactersByIdAsync(int[] ids)
        {
            var sqlConn = CreateConnection();
            var results = await sqlConn.QueryAsync<CharacterDbModel>(c => ids.Contains(c.Id));
            
            var mappedResults = MapDbModelsToCharacterModels(results);
            return mappedResults;
        }

        public async Task<IEnumerable<ICharacter>> GetCharacterFriendsAsync(int characterId)
        {
            var sqlConn = CreateConnection();
            var results = await sqlConn.QueryAsync<CharacterFriendDbModel>(
                where: f => f.FriendOfId == characterId,
                //Always include a Default Sort Order (for paging)
                orderBy: OrderField.Parse(new { Name = Order.Ascending })
            );
            
            var mappedResults = MapDbModelsToCharacterModels(results);
            return mappedResults;
        }

        public async Task<ICursorPageSlice<ICharacter>> GetCharacterFriendsAsync(int characterId, IRepoDbCursorPagingParams pagingParams)
        {
            var sqlConn = CreateConnection();
            var results = await sqlConn.GraphQLBatchSliceQueryAsync<CharacterFriendDbModel>(
                where: f => f.FriendOfId == characterId,
                //Always include a Default Sort Order (for paging)
                orderBy: OrderField.Parse(new { Name = Order.Ascending }),
                afterCursor: pagingParams.AfterIndex,
                firstTake: pagingParams.First,
                beforeCursor: pagingParams.BeforeIndex,
                lastTake: pagingParams.Last
            );

            var mappedResults = results.AsMappedType(c => MapDbModelToCharacterModel(c));
            return mappedResults;
        }

        private IEnumerable<ICharacter> MapDbModelsToCharacterModels(IEnumerable<CharacterDbModel> dbModels)
        {
            var results = dbModels.Select(r => MapDbModelToCharacterModel(r));
            return results;
        }

        private ICharacter MapDbModelToCharacterModel(CharacterDbModel dbModel)
        {
            var m = dbModel;
            return m.IsHuman
                ? (ICharacter)new Human(m.Id, m.Name, m.HomePlanet)
                : (ICharacter)new Droid(m.Id, m.Name, m.PrimaryFunction);
        }


        public async Task<ICharacter> GetHeroAsync(Episode episode)
        {
            throw new NotImplementedException();
            //if (episode == Episode.Empire)
            //{
            //    return _characters[1000];
            //}
            //return _characters[2001];
        }

        public async Task<IEnumerable<ISearchResult>> SearchAsync(string text)
        {
            throw new NotImplementedException();
            //IEnumerable<ICharacter> filteredCharacters = _characters.Values
            //    .Where(t => t.Name.Contains(text,
            //        StringComparison.OrdinalIgnoreCase));

            //foreach (ICharacter character in filteredCharacters)
            //{
            //    yield return character;
            //}

            //IEnumerable<Starship> filteredStarships = _starships.Values
            //    .Where(t => t.Name.Contains(text,
            //        StringComparison.OrdinalIgnoreCase));

            //foreach (Starship starship in filteredStarships)
            //{
            //    yield return starship;
            //}
        }
    }
}
