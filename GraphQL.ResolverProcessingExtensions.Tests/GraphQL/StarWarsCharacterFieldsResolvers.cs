using HotChocolate;
using HotChocolate.ResolverProcessingExtensions;
using HotChocolate.Types;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using HotChocolate.ResolverProcessingExtensions.Tests;

namespace StarWars.Characters
{
    [ExtendObjectType(nameof(StarWarsHuman))]
    public class HumanFieldResolvers
    {
        [GraphQLName("droids")]
        [ResolverProcessingParentDependencies(nameof(IStarWarsCharacter.Id))]
        public Task<IEnumerable<StarWarsDroid>> GetDroidsAsync(
            [Parent] StarWarsHuman character,
            [GraphQLParams] IParamsContext graphQLParams    
        )
        {
            #if DEBUG
            Debug.WriteLine($"Pre-processing Dependency Fields: [{string.Join(", ", graphQLParams.SelectionDependencies.Select(d => d.DependencyMemberName))}]");
            #endif

            var allDroids = StarWarsCharacterRepo.CreateCharacters().OfType<StarWarsDroid>().ToLookup(d => d.Id);
            var droids = character.DroidIds.Select(droidId => allDroids[droidId].FirstOrDefault());
            return Task.FromResult(droids);
        }
    }
}
