using HotChocolate;
using HotChocolate.PreProcessedExtensions;
using HotChocolate.Types;
using StarWars.Characters;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace StarWars.Characters
{
    [ExtendObjectType(nameof(Human))]
    public class HumanFieldResolvers
    {
        [GraphQLName("droids")]
        [PreProcessingParentDependencies(nameof(ICharacter.Id))]
        public Task<Droid> GetDroidsAsync(
            [GraphQLParams] IParamsContext graphQLParams    
        )
        {
            var parent = graphQLParams.ResolverContext.Parent<ICharacter>();
            return Task.FromResult(new Droid(parent.Id, "BB-8", "Astromech"));
        }
    }
}
