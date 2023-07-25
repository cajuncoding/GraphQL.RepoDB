using HotChocolate;
using HotChocolate.PreProcessingExtensions;
using HotChocolate.Types;
using StarWars.Characters;
using StarWars.Repositories;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
            [Parent] ICharacter character,
            [GraphQLParams] IParamsContext graphqlParams    
        )
        {
            #if DEBUG
            Debug.WriteLine($"Pre-processing Dependency Fields: [{string.Join(", ", graphqlParams.SelectionDependencies.Select(d => d.DependencyMemberName))}]");
            #endif

            var friends = await repository.GetCharacterFriendsAsync(character.Id);
            var droids = friends.OfType<Droid>();
            return droids;
        }
    }
}
