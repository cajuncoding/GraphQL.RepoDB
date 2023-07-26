using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HotChocolate.Data;
using HotChocolate.ResolverProcessingExtensions.Pagination;
using HotChocolate.Types;
using HotChocolate.Types.Pagination;

namespace HotChocolate.ResolverProcessingExtensions.Tests
{
    [ExtendObjectType("Query")]
    public class StarWarsCharacterResolver
    {
        [UseSorting]
        [GraphQLName("starWarsCharacters")]
        public Task<IEnumerable<IStarWarsCharacter>> GetStarWarsCharactersAsync(
            [GraphQLParams] IParamsContext paramsContext
        )
        {
            var results = StarWarsCharacterRepo.CreateCharacters();
            return Task.FromResult(results);
        }

        [UsePaging]
        [UseSorting]
        [GraphQLName("starWarsCharactersCursorPaginated")]
        public Task<Connection<IStarWarsCharacter>> GetStarWarsCharactersWithCursorPagingAsync(
            [GraphQLParams] IParamsContext paramsContext,
            bool testEmptyResults = false
        )
        {
            var characters = testEmptyResults
                ? Enumerable.Empty<IStarWarsCharacter>()
                : StarWarsCharacterRepo.CreateCharacters();
            
            var results = characters.SliceAsCursorPage(paramsContext.CursorPagingArgs);
            
            return Task.FromResult(results.ToGraphQLConnection());
        }

        [UseOffsetPaging]
        [UseSorting]
        [GraphQLName("starWarsCharactersOffsetPaginated")]
        public Task<CollectionSegment<IStarWarsCharacter>> GetStarWarsCharactersWithOffsetPagingAsync(
            [GraphQLParams] IParamsContext paramsContext,
            bool testEmptyResults = false
        )
        {
            var characters = testEmptyResults
                ? Enumerable.Empty<IStarWarsCharacter>()
                : StarWarsCharacterRepo.CreateCharacters();

            var results = characters.SliceAsOffsetPage(paramsContext.OffsetPagingArgs);

            return Task.FromResult(results.ToGraphQLCollectionSegment());
        }
    }
}
