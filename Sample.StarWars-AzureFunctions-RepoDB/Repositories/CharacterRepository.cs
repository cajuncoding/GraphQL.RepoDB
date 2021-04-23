//NOTE: Updated to alleviate Visual Studio warning...
#nullable enable

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using HotChocolate.PreProcessingExtensions.Pagination;
using Microsoft.Data.SqlClient;
using RepoDb;
using RepoDb.Enumerations;
using RepoDb.CursorPagination;
using RepoDb.OffsetPagination;
using StarWars.Characters;
using StarWars.Characters.DbModels;

namespace StarWars.Repositories
{
    public class CharacterRepository : BaseRepository<ICharacter, SqlConnection>, ICharacterRepository
    {
        public static readonly IReadOnlyList<OrderField> DefaultCharacterSortFields = new List<OrderField>()
        {
            OrderField.Ascending<ICharacter>(c => c.Id)
        }.AsReadOnly();

        public CharacterRepository(string connectionString)
            : base(connectionString)
        {
        }

        public async Task<IEnumerable<ICharacter>> GetAllSortedCharactersAsync(
            IEnumerable<Field> selectFields,
            IEnumerable<OrderField> sortFields
        )
        {
            await using var sqlConn = CreateConnection();

            var results = await sqlConn.QueryAsync<CharacterDbModel>(
                where: c => c.Id >= 1000 && c.Id <=2999,
                fields: selectFields,
                orderBy: sortFields ?? DefaultCharacterSortFields
            );

            var mappedResults = MapDbModelsToCharacterModels(results);
            return mappedResults;
        }


        public async Task<ICursorPageSlice<ICharacter>> GetCursorPagedCharactersAsync(
            IEnumerable<Field> selectFields,
            IEnumerable<OrderField> sortFields, 
            IRepoDbCursorPagingParams pagingParams
        )
        {
            await using var sqlConn = CreateConnection();

            var pageSlice = await sqlConn.GraphQLBatchSliceQueryAsync<CharacterDbModel>(
                fields: selectFields,
                orderBy: sortFields ?? DefaultCharacterSortFields,
                afterCursor: pagingParams.AfterIndex!,
                beforeCursor: pagingParams.BeforeIndex!,
                firstTake: pagingParams.First,
                lastTake: pagingParams.Last,
                logTrace: s => Debug.WriteLine(s),
                commandTimeout: 15
            );

            var convertedSlice = pageSlice.AsMappedType(r => MapDbModelToCharacterModel(r));
            return convertedSlice;
        }

        public async Task<IOffsetPageResults<ICharacter>> GetOffsetPagedCharactersAsync(
            IEnumerable<Field> selectFields,
            IEnumerable<OrderField> sortFields,
            IRepoDbOffsetPagingParams pagingParams
        )
        {
            await using var sqlConn = CreateConnection();

            var offsetPageResults = await sqlConn.GraphQLBatchOffsetPagingQueryAsync<CharacterDbModel>(
                page: pagingParams.Page,
                rowsPerBatch: pagingParams.RowsPerBatch,
                fetchTotalCount: pagingParams.IsTotalCountEnabled,
                orderBy: sortFields ?? DefaultCharacterSortFields,
                fields: selectFields
            );

            var convertedPage = offsetPageResults.AsMappedType(r => MapDbModelToCharacterModel(r));
            return convertedPage;
        }

        public async Task<ICursorPageSlice<Human>> GetPagedHumanCharactersAsync(
            IEnumerable<Field> selectFields,
            IEnumerable<OrderField> sortFields,
            IRepoDbCursorPagingParams pagingParams
        )
        {
            await using var sqlConn = CreateConnection();

            var pageSlice = await sqlConn.GraphQLBatchSliceQueryAsync<CharacterDbModel>(
                orderBy: sortFields ?? DefaultCharacterSortFields,
                fields: selectFields,
                where: c => c.Id >=1000 && c.Id <= 1999,
                afterCursor: pagingParams.AfterIndex!,
                beforeCursor: pagingParams.BeforeIndex!,
                firstTake: pagingParams.First,
                lastTake: pagingParams.Last,
                commandTimeout: 15
            );

            var convertedSlice = pageSlice.AsMappedType(r => (Human)MapDbModelToCharacterModel(r));
            return convertedSlice;
        }

        public async Task<IEnumerable<ICharacter>> GetCharactersByIdAsync(int[] ids)
        {
            await using var sqlConn = CreateConnection();
            var results = await sqlConn.QueryAsync<CharacterDbModel>(c => ids.Contains(c.Id));
            
            var mappedResults = MapDbModelsToCharacterModels(results);
            return mappedResults;
        }

        public async Task<IEnumerable<ICharacter>> GetCharacterFriendsAsync(int characterId)
        {
            await using var sqlConn = CreateConnection();
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
            await using var sqlConn = CreateConnection();
            var results = await sqlConn.GraphQLBatchSliceQueryAsync<CharacterFriendDbModel>(
                where: f => f.FriendOfId == characterId,
                //Always include a Default Sort Order (for paging)
                orderBy: OrderField.Parse(new { Name = Order.Ascending }),
                afterCursor: pagingParams.AfterIndex,
                firstTake: pagingParams.First,
                beforeCursor: pagingParams.BeforeIndex,
                lastTake: pagingParams.Last,
                commandTimeout: 15
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
