using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HotChocolate.PreProcessingExtensions.Pagination;
using RepoDb;
using RepoDb.CursorPagination;
using StarWars.Characters;

namespace StarWars.Repositories
{
    public interface ICharacterRepository
    {
        Task<IEnumerable<ICharacter>> GetAllSortedCharactersAsync(
            IEnumerable<Field> selectFields,
            IEnumerable<OrderField> sortFields
        );

        Task<ICursorPageSlice<ICharacter>> GetPagedCharactersAsync(
            IEnumerable<Field> selectFields,
            IEnumerable<OrderField> sortFields, 
            IRepoDbCursorPagingParams pagingParams
        );

        Task<ICursorPageSlice<Human>> GetPagedHumanCharactersAsync(
            IEnumerable<Field> selectFields,
            IEnumerable<OrderField> sortFields,
            IRepoDbCursorPagingParams pagingParams
        );

        Task<IEnumerable<ICharacter>> GetCharacterFriendsAsync(int characterId);

        Task<ICursorPageSlice<ICharacter>> GetCharacterFriendsAsync(int characterId, IRepoDbCursorPagingParams pagingParams);

        Task<IEnumerable<ICharacter>> GetCharactersByIdAsync(params int[] ids);
        
        Task<ICharacter> GetHeroAsync(Episode episode);
        
        Task<IEnumerable<ISearchResult>> SearchAsync(string text);
    }
}
