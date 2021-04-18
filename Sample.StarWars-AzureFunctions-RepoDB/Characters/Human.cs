# nullable enable

using System.Collections.Generic;
using HotChocolate;
using HotChocolate.PreProcessingExtensions;
using HotChocolate.Types;
using HotChocolate.Types.Relay;
using RepoDb.Attributes;

namespace StarWars.Characters
{
    /// <summary>
    /// A human character in the Star Wars universe.
    /// </summary>
    public class Human : ICharacter
    {
        public Human(
            int id,
            string name,
            string? homePlanet = null,
            IReadOnlyList<ICharacter>? friends = null,
            IReadOnlyList<Episode>? appearsIn = null,
            double height = 1.72d)
        {
            Id = id;
            Name = name;
            Friends = friends ?? new List<ICharacter>();
            AppearsIn = appearsIn ?? new List<Episode>();
            HomePlanet = homePlanet;
            Height = height;
        }

        /// <inheritdoc />
        [GraphQLName("personalIdentifier")]
        public int Id { get; set;  }

        /// <inheritdoc />
        public string Name { get; set; } = "";

        /// <summary>
        /// The planet the character is originally from.
        /// </summary>
        public string? HomePlanet { get; set; }
        
        /// <inheritdoc />
        [UsePaging(type: typeof(InterfaceType<ICharacter>))]
        [GetFriendsResolver]
        //Establish configured link to required Selection for Character.Id for 
        //  resolvers that implement pre-processed results within the Repository Layer.
        [PreProcessingParentDependencies(nameof(ICharacter.Id))]
        public IReadOnlyList<ICharacter>? Friends { get; set; }

        /// <inheritdoc />
        public IReadOnlyList<Episode>? AppearsIn { get; set; }

        /// <inheritdoc />
        [UseConvertUnit]
        public double Height { get; set; } = 1.72d;
    }
}
