//NOTE: Updated to alleviate Visual Studio warning...
#nullable enable

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using HotChocolate.RepoDb.Sql;
using HotChocolate.ResolverProcessingExtensions.Pagination;
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
                pagingParams: pagingParams,
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

            var offsetPageResults = await sqlConn.GraphQLBatchSkipTakeQueryAsync<CharacterDbModel>(
                pagingParams: pagingParams,
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
                whereExpression: c => c.Id >=1000 && c.Id <= 1999,
                pagingParams: pagingParams,
                commandTimeout: 15
            );

            var convertedSlice = pageSlice.AsMappedType(r => (Human)MapDbModelToCharacterModel(r));
            return convertedSlice;
        }

        public async Task<ICursorPageSlice<Droid>> GetPagedDroidCharactersAsync(
            IEnumerable<Field> selectFields,
            IEnumerable<OrderField> sortFields,
            IRepoDbCursorPagingParams pagingParams
        )
        {
            await using var sqlConn = CreateConnection();

            //BBernard - 08/09/2021
            //NOTE: This Examples shows the use of RepoDb mapping and Raw SQL with Batch Slice Query capabilities;
            //      which enables powerful field processing and features not supported by QueryField/QueryGroup objects
            //      (e.g. LOWER(), TRIM(), or Full Text Search via CONTAINS() and FREETEXT()).
            var idFieldName = PropertyMappedNameCache.Get<CharacterDbModel>(c => c.Id);

            var pageSlice = await sqlConn.GraphQLBatchSliceQueryAsync<CharacterDbModel>(
                orderBy: sortFields ?? DefaultCharacterSortFields,
                fields: selectFields,
                whereRawSql: new RawSqlWhere(@$"{idFieldName} >= @StartId AND {idFieldName} < @EndId", new {StartId = 2000, EndId = 3000}),
                pagingParams: pagingParams,
                commandTimeout: 15
            );

            var convertedSlice = pageSlice.AsMappedType(r => (Droid)MapDbModelToCharacterModel(r));
            return convertedSlice;
        }

        public async Task<IEnumerable<ICharacter>> GetCharactersByIdAsync(IEnumerable<Field> selectFields, int[] ids)
        {
            await using var sqlConn = CreateConnection();
            //var results = await sqlConn.QueryAsync<CharacterDbModel>(c => ids.Contains(c.Id));
            var results = await sqlConn.QueryBulkResultsByIdAsync<CharacterDbModel>(ids, fields: selectFields, logTrace: l => Debug.WriteLine(l));

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
                whereExpression: f => f.FriendOfId == characterId,
                //Always include a Default Sort Order (for paging)
                orderBy: OrderField.Parse(new { Name = Order.Ascending }),
                pagingParams: pagingParams,
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


        public Task<ICharacter> GetHeroAsync(Episode episode)
        {
            throw new NotImplementedException();
            //if (episode == Episode.Empire)
            //{
            //    return _characters[1000];
            //}
            //return _characters[2001];
        }

        public Task<IEnumerable<ISearchResult>> SearchAsync(string text)
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
