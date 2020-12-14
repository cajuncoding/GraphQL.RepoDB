//NOTE: Updated to alleviate Visual Studio warning...
#nullable enable

using System.Collections.Generic;
using GraphQL.PreProcessingExtensions.Tests;
using HotChocolate.Types;
using HotChocolate.Types.Relay;

namespace GraphQL.PreProcessingExtensions.Tests
{
    /// <summary>
    /// A human character in the Star Wars universe.
    /// </summary>
    public class StarWarsHuman : IStarWarsCharacter
    {
        public StarWarsHuman(
            int id, 
            string name, 
            string? homePlanet = null)
        {
            Id = id;
            Name = name;
            HomePlanet = homePlanet;
        }

        /// <inheritdoc />
        public int Id { get; }

        /// <inheritdoc />
        public string Name { get; }

        /// <summary>
        /// The planet the character is originally from.
        /// </summary>
        public string? HomePlanet { get; }
    }
}
