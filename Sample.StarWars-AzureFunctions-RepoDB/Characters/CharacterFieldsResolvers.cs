using HotChocolate;
using HotChocolate.RepoDb;
using HotChocolate.ResolverProcessingExtensions;
using HotChocolate.Types;
using StarWars.Repositories;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace StarWars.Characters
{
    [ExtendObjectType(nameof(Human))]
    public class HumanFieldResolvers
    {
        [GraphQLName("droids")]
        [ResolverProcessingParentDependencies(nameof(ICharacter.Id))]
        public async Task<IEnumerable<Droid>> GetDroidsAsync(
            [Service] ICharacterRepository repository,
            [Parent] ICharacter character,
            [GraphQLRepoDbMapper] GraphQLRepoDbMapper<Droid> repoDbGraphQL
        )
        {
            //Log Debug info. for the Dependency Fields for this Field Resolver...
            #if DEBUG
            Debug.WriteLine($"Pre-processing Dependency Fields: [{string.Join(", ", repoDbGraphQL.GraphQLParamsContext.SelectionDependencies.Select(d => d.DependencyMemberName))}]");
            #endif

            var friends = await repository.GetCharacterFriendsAsync(character.Id);
            var droids = friends.OfType<Droid>();
            return droids;
        }
    }
}
