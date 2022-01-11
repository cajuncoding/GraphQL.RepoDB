using HotChocolate;
using HotChocolate.PreProcessingExtensions;
using HotChocolate.Types;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using HotChocolate.PreProcessingExtensions.Tests;

namespace StarWars.Characters
{
    [ExtendObjectType(nameof(StarWarsHuman))]
    public class HumanFieldResolvers
    {
        [GraphQLName("droids")]
        [PreProcessingParentDependencies(nameof(IStarWarsCharacter.Id))]
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
