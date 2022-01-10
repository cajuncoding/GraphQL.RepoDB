using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HotChocolate;
using HotChocolate.Data;
using HotChocolate.PreProcessingExtensions.Pagination;
using HotChocolate.Types;

namespace HotChocolate.PreProcessingExtensions.Tests
{
    [ExtendObjectType(Name = "Query")]
    public class StarWarsCharacterResolver
    {
        [UseSorting]
        [GraphQLName("starWarsCharacters")]
        public Task<IEnumerable<IStarWarsCharacter>> GetStarWarsCharactersAsync()
        {
            var results = CreateCharacters();
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
                : CreateCharacters();
            
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
                : CreateCharacters();

            var results = characters.SliceAsOffsetPage(paramsContext.OffsetPagingArgs ?? );

            return Task.FromResult(results.AsPreProcessedPageResults());
        }

        private static IEnumerable<IStarWarsCharacter> CreateCharacters()
        {
            yield return new StarWarsHuman
            (
                1000,
                "Luke Skywalker",
                "Tatooine"
            );

            yield return new StarWarsHuman
            (
                1001,
                "Darth Vader",
                "Tatooine"
            );

            yield return new StarWarsHuman
            (
                1002,
                "Han Solo"
            );

            yield return new StarWarsHuman
            (
                1003,
                "Leia Organa",
                "Alderaan"
            );

            yield return new StarWarsHuman
            (
                1004,
                "Wilhuff Tarkin"
            );

            yield return new StarWarsDroid
            (
                2000,
                "C-3PO",
                "Protocol"
            );

            yield return new StarWarsDroid
            (
                2001,
                "R2-D2",
                "Astromech"
            );
        }
    }
}
