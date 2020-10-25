//NOTE: Updated to alleviate Visual Studio warning...
#nullable enable

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
        public Human()
        {
        }

        //public Human(
        //    int id, 
        //    string name, 
        //    IReadOnlyList<int> friends, 
        //    IReadOnlyList<Episode> appearsIn, 
        //    string? homePlanet = null, 
        //    double height = 1.72d)
        //{
        //    Id = id;
        //    Name = name;
        //    Friends = friends;
        //    AppearsIn = appearsIn;
        //    HomePlanet = homePlanet;
        //    Height = height;
        //}

        /// <inheritdoc />
        public int Id { get; set;  }

        /// <inheritdoc />
        public string Name { get; set; }

        /// <inheritdoc />
        [UsePaging(type: typeof(InterfaceType<ICharacter>))]
        [GetFriendsResolver]
        public IReadOnlyList<ICharacter> Friends { get; set; } 

        /// <inheritdoc />
        public IReadOnlyList<Episode> AppearsIn { get; set;  }

        /// <summary>
        /// The planet the character is originally from.
        /// </summary>
        public string? HomePlanet { get; set; }

        /// <inheritdoc />
        [UseConvertUnit]
        public double Height { get; set; }
    }
}
