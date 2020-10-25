# nullable enable

using System.Collections.Generic;
using HotChocolate.Types;
using HotChocolate.Types.Relay;
using RepoDb.Attributes;

namespace StarWars.Characters
{
    /// <summary>
    /// A human character in the Star Wars universe.
    /// </summary>
    [Map("StarWarsCharacters")]
    public class Human : ICharacter
    {
        //RepoDb will need an Empty Constructor
        public Human() { }

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
        public IReadOnlyList<ICharacter>? Friends { get; set; }

        /// <inheritdoc />
        public IReadOnlyList<Episode>? AppearsIn { get; set; }

        /// <inheritdoc />
        [UseConvertUnit]
        public double Height { get; set; } = 1.72d;
    }
}
