using HotChocolate;
using HotChocolate.Data;
using HotChocolate.PreProcessedExtensions;
using HotChocolate.PreProcessedExtensions.Pagination;
using HotChocolate.PreProcessedExtensions.Sorting;
using HotChocolate.RepoDb;
using HotChocolate.Types;
using StarWars.Characters;
using StarWars.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StarWars_AzureFunctions.Characters
{
    [ExtendObjectType(nameof(ICharacter))]
    public class CharacterFieldResolvers
    {
        /// <summary>
        /// Add dynamic 'friends' element to ICharacter GraphQL object and implement resolving logic...
        /// </summary>
        /// <param name="repository"></param>
        /// <param name="graphQLParams"></param>
        /// <returns></returns>
        [UseSorting]
        [GraphQLName("friends")]
        public async Task<PreProcessedSortedResults<ICharacter>> GetFriendsAsync(
            [Service] ICharacterRepository repository,
            [Parent] ICharacter character,
            [GraphQLParams] IParamsContext graphQLParams
        )
        {
            var repoDbParams = new GraphQLRepoDbParams<Character>(graphQLParams);

            var sortedCharacters = await repository.GetCharacterFriendsAsync(
                selectFields: repoDbParams.SelectFields,
                sortFields: repoDbParams.SortOrderFields,
                character
            );

            return new PreProcessedSortedResults<ICharacter>(sortedCharacters);
        }
    }
}
