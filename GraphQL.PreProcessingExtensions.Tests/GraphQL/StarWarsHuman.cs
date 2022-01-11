//NOTE: Updated to alleviate Visual Studio warning...
#nullable enable

using System;
using System.Collections.Generic;
using HotChocolate.PreProcessingExtensions.Tests;
using HotChocolate.Types;
using HotChocolate.Types.Relay;

namespace HotChocolate.PreProcessingExtensions.Tests
{
    /// <summary>
    /// A human character in the Star Wars universe.
    /// </summary>
    public class StarWarsHuman : IStarWarsCharacter
    {
        public StarWarsHuman(
            int id, 
            string name, 
            string? homePlanet = null,
            int[]? droidIds = null
        )
        {
            Id = id;
            Name = name;
            HomePlanet = homePlanet;
            DroidIds = droidIds ?? Array.Empty<int>();
        }

        /// <inheritdoc />
        public int Id { get; }

        /// <inheritdoc />
        public string Name { get; }

        /// <inheritdoc />
        [GraphQLIgnore]
        public int[] DroidIds { get; }

        /// <summary>
        /// The planet the character is originally from.
        /// </summary>
        public string? HomePlanet { get; }
    }
}
