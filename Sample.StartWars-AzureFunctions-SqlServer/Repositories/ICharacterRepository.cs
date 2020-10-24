using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HotChocolate.PreProcessedExtensions.Pagination;
using RepoDb;
using RepoDb.CursorPagination;
using StarWars.Characters;

namespace StarWars.Repositories
{
    public interface ICharacterRepository
    {
        Task<IEnumerable<ICharacter>> GetCharactersAsync(
            IEnumerable<Field> selectFields,
            IEnumerable<OrderField> sortFields
        );

        Task<ICursorPageSlice<ICharacter>> GetCharactersAsync(
            IEnumerable<Field> selectFields,
            IEnumerable<OrderField> sortFields, 
            IRepoDbCursorPagingParams pagingParams
        );

        Task<IEnumerable<ICharacter>> GetCharacterFriendsAsync(ICharacter character);
        Task<IEnumerable<ICharacter>> GetCharactersAsync(params int[] ids);
        Task<ICharacter> GetHeroAsync(Episode episode);

        Task<IEnumerable<ISearchResult>> SearchAsync(string text);
    }
}
