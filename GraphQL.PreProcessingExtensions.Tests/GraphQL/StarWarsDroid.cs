using System.Collections.Generic;
using HotChocolate.Types;
using HotChocolate.Types.Relay;

namespace HotChocolate.PreProcessingExtensions.Tests
{
    /// <summary>
    /// A droid in the Star Wars universe.
    /// </summary>
    public class StarWarsDroid : IStarWarsCharacter
    {
        public StarWarsDroid(
            int id,
            string name,
            string primaryFunction
        )
        {
            Id = id;
            Name = name;
            PrimaryFunction = primaryFunction;
        }

        /// <inheritdoc />
        public int Id { get; }

        /// <inheritdoc />
        public string Name { get; }

        /// <summary>
        /// The droid's primary function.
        /// </summary>
        public string PrimaryFunction { get; }
    }
}
