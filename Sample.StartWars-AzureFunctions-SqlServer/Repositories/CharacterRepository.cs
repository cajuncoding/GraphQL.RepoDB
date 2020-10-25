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

            var convertedResults = results.Select(r =>
            {
                return r.IsHuman
                    ? (ICharacter)new Human(r.Id, r.Name, r.HomePlanet)
                    : (ICharacter)new Droid(r.Id, r.Name, r.PrimaryFunction);
            });

            return convertedResults;

            //var safeSelectClause = await sqlConn.GetValidatedSelectClause(TableNames.StarWarsCharacters, selectFields);
            //var safeOrderByClause = await sqlConn.GetValidatedOrderByClause(TableNames.StarWarsCharacters, sortFields);

            //var query = await sqlConn.ExecuteQueryMultipleAsync(
            //    commandText: @$"
            //        --Query Humans (by ID Convention)
            //        SELECT {safeSelectClause} FROM [{TableNames.StarWarsCharacters}] c 
            //            WHERE c.Id BETWEEN 1000 and 1999 
            //            {safeOrderByClause};

            //        --Query Droids (by ID Convention)
            //        SELECT {safeSelectClause} FROM [{TableNames.StarWarsCharacters}] c 
            //            WHERE c.Id BETWEEN 2000 and 2999;
            //            {safeOrderByClause};
            //    "
            //);

            //var results = new List<ICharacter>();
            //results.AddRange(query.Extract<Human>());
            //results.AddRange(query.Extract<Droid>());

            //return results;
        }


        public async Task<ICursorPageSlice<ICharacter>> GetPagedCharactersAsync(
            IEnumerable<Field> selectFields,
            IEnumerable<OrderField> sortFields, 
            IRepoDbCursorPagingParams pagingParams
        )
        {
            var sqlConn = CreateConnection();

            var pageSlice = await sqlConn.BatchSliceQueryAsync<CharacterDbModel>(
                afterCursor: pagingParams.AfterIndex!,
                beforeCursor: pagingParams.BeforeIndex!,
                firstTake: pagingParams.First,
                lastTake: pagingParams.Last,
                orderBy: sortFields,
                fields: selectFields
            );

            var convertedSlice = pageSlice.AsMappedType<ICharacter>(r =>
            {
                return r.IsHuman
                    ? (ICharacter)new Human(r.Id, r.Name, r.HomePlanet)
                    : (ICharacter)new Droid(r.Id, r.Name, r.PrimaryFunction);
            });

            return convertedSlice;
        }

        public async Task<IEnumerable<ICharacter>> GetCharactersByIdAsync(int[] ids)
        {
            var sqlConn = CreateConnection();
            var results = await sqlConn.QueryAsync<CharacterDbModel>(c => ids.Contains(c.Id));

            return results.Select(r =>
            {
                return r.IsHuman
                    ? (ICharacter)new Human(r.Id, r.Name, r.HomePlanet)
                    : (ICharacter)new Droid(r.Id, r.Name, r.PrimaryFunction);
            });
        }

        public async Task<IEnumerable<ICharacter>> GetCharacterFriendsAsync(ICharacter character)
        {

            throw new NotImplementedException();

            //var sqlConn = CreateConnection();
            //return await sqlConn.QueryAsync<Character>(c => ids.Contains(c.Id));
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
