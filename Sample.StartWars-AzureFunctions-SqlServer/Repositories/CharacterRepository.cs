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
        public CharacterRepository(string connectionString)
            : base(connectionString)
        {
            _starships = CreateStarships().ToDictionary(t => t.Id);
        }

        private Dictionary<int, Starship> _starships;


        public async Task<IEnumerable<ICharacter>> GetCharactersAsync(
            IEnumerable<Field> selectFields,
            IEnumerable<OrderField> sortFields
        )
        {
            var sqlConn = CreateConnection();

            var characters = await sqlConn.QueryAllAsync<Character>(
                fields: selectFields,
                orderBy: sortFields
            );

            return characters;
        }


        public async Task<ICursorPageSlice<ICharacter>> GetCharactersAsync(
            IEnumerable<Field> selectFields,
            IEnumerable<OrderField> sortFields, 
            IRepoDbCursorPagingParams pagingParams
        )
        {
            var sqlConn = CreateConnection();

            var pageSlice = await sqlConn.BatchSliceQueryAsync<Character>(
                afterCursor: pagingParams.AfterIndex!,
                beforeCursor: pagingParams.BeforeIndex!,
                firstTake: pagingParams.First,
                lastTake: pagingParams.Last,
                orderBy: sortFields,
                fields: selectFields
            );

            return pageSlice.OfType<ICharacter>();
        }

        public async Task<IEnumerable<ICharacter>> GetCharactersAsync(int[] ids)
        {
            var sqlConn = CreateConnection();
            return await sqlConn.QueryAsync<Character>(c => ids.Contains(c.Id));
        }

        public async Task<IEnumerable<ICharacter>> GetCharacterFriendsAsync(
            IEnumerable<Field> selectFields,
            IEnumerable<OrderField> sortFields,
            ICharacter character
        )
        {
            var sqlConn = CreateConnection();
            var validDbFields = await sqlConn.GetValidatedDbFields<ICharacter>(selectFields);

            var friends = await sqlConn.QueryAsync<Character>(
                tableName: "ViewStarWarsFriends",
                fields: validDbFields,
                orderBy: sortFields,
                where: new QueryField("FriendOfId", character.Id)
            );

            return friends.OfType<ICharacter>();
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


        private static IEnumerable<Starship> CreateStarships()
        {
            yield return new Starship
            (
                3000,
                "TIE Advanced x1",
                 9.2
            );
        }

    }
}
