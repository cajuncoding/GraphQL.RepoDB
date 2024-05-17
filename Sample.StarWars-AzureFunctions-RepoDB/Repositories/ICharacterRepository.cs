using System.Collections.Generic;
using System.Threading.Tasks;
using HotChocolate.ResolverProcessingExtensions.Pagination;
using RepoDb;
using RepoDb.PagingPrimitives.CursorPaging;
using RepoDb.PagingPrimitives.OffsetPaging;
using StarWars.Characters;

namespace StarWars.Repositories
{
    public interface ICharacterRepository
    {
        Task<IEnumerable<ICharacter>> GetAllSortedCharactersAsync(
            IEnumerable<Field> selectFields,
            IEnumerable<OrderField> sortFields
        );

        Task<ICursorPageResults<ICharacter>> GetCursorPagedCharactersAsync(
            IEnumerable<Field> selectFields,
            IEnumerable<OrderField> sortFields, 
            ICursorPagingParams pagingParams
        );

        Task<IOffsetPageResults<ICharacter>> GetOffsetPagedCharactersAsync(
            IEnumerable<Field> selectFields,
            IEnumerable<OrderField> sortFields,
            IOffsetPagingParams pagingParams
        );

        Task<ICursorPageResults<Human>> GetPagedHumanCharactersAsync(
            IEnumerable<Field> selectFields,
            IEnumerable<OrderField> sortFields,
            ICursorPagingParams pagingParams
        );

        Task<ICursorPageResults<Droid>> GetPagedDroidCharactersAsync(
            IEnumerable<Field> selectFields,
            IEnumerable<OrderField> sortFields,
            ICursorPagingParams pagingParams
        );

        Task<IEnumerable<ICharacter>> GetCharacterFriendsAsync(int characterId);

        Task<ICursorPageResults<ICharacter>> GetCharacterFriendsAsync(int characterId, ICursorPagingParams pagingParams);

        Task<IEnumerable<ICharacter>> GetCharactersByIdAsync(IEnumerable<Field> selectFields, int[] ids);
        
        Task<ICharacter> GetHeroAsync(Episode episode);
        
        Task<IEnumerable<ISearchResult>> SearchAsync(string text);
    }
}
