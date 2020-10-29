using HotChocolate;
using HotChocolate.PreProcessedExtensions;
using HotChocolate.Types;
using StarWars.Characters;
using StarWars.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StarWars.Characters
{
    [ExtendObjectType(nameof(Human))]
    public class HumanFieldResolvers
    {
        [GraphQLName("droids")]
        [PreProcessingParentDependencies(nameof(ICharacter.Id))]
        public async Task<IEnumerable<Droid>> GetDroidsAsync(
            [Service] ICharacterRepository repository,
            [Parent] ICharacter character
            //[GraphQLParams] IParamsContext graphQLParams    
        )
        {
            var friends = await repository.GetCharacterFriendsAsync(character.Id);
            var droids = friends.OfType<Droid>();
            return droids;
        }
    }
}
