using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HotChocolate.Data;
using HotChocolate.PreProcessingExtensions.Pagination;
using HotChocolate.Types;

namespace HotChocolate.PreProcessingExtensions.Tests
{
    [ExtendObjectType("Query")]
    public class StarWarsCharacterResolver
    {
        [UseSorting]
        [GraphQLName("starWarsCharacters")]
        public Task<IEnumerable<IStarWarsCharacter>> GetStarWarsCharactersAsync()
        {
            var results = StarWarsCharacterRepo.CreateCharacters();
            return Task.FromResult(results);
        }

        [UsePaging]
        [UseSorting]
        [GraphQLName("starWarsCharactersCursorPaginated")]
        public Task<PreProcessedCursorSlice<IStarWarsCharacter>> GetStarWarsCharactersWithCursorPagingAsync(
            [GraphQLParams] IParamsContext paramsContext,
            bool testEmptyResults = false
        )
        {
            var characters = testEmptyResults
                ? Enumerable.Empty<IStarWarsCharacter>()
                : StarWarsCharacterRepo.CreateCharacters();
            
            var results = characters.SliceAsCursorPage(paramsContext.CursorPagingArgs);
            
            return Task.FromResult(results.AsPreProcessedCursorSlice());
        }

        [UseOffsetPaging]
        [UseSorting]
        [GraphQLName("starWarsCharactersOffsetPaginated")]
        public Task<PreProcessedOffsetPageResults<IStarWarsCharacter>> GetStarWarsCharactersWithOffsetPagingAsync(
            [GraphQLParams] IParamsContext paramsContext,
            bool testEmptyResults = false
        )
        {
            var characters = testEmptyResults
                ? Enumerable.Empty<IStarWarsCharacter>()
                : StarWarsCharacterRepo.CreateCharacters();

            var results = characters.SliceAsOffsetPage(paramsContext.OffsetPagingArgs);

            return Task.FromResult(results.AsPreProcessedPageResults());
        }
    }
}
