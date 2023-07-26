# nullable enable

using System.Collections.Generic;
using HotChocolate;
using HotChocolate.ResolverProcessingExtensions;
using HotChocolate.Types;
using HotChocolate.Types.Relay;

namespace StarWars.Characters
{
    /// <summary>
    /// A droid in the Star Wars universe.
    /// </summary>
    public class Droid : ICharacter
    {
        public Droid(
            int id,
            string name,
            string primaryFunction,
            IReadOnlyList<ICharacter> friends = null!,
            IReadOnlyList<Episode> appearsIn = null!,
            double height = 1.72d)
        {
            Id = id;
            Name = name;
            Friends = friends ?? new List<ICharacter>();
            AppearsIn = appearsIn ?? new List<Episode>();
            PrimaryFunction = primaryFunction;
            Height = height;
        }

        /// <inheritdoc />
        [GraphQLName("personalIdentifier")]
        public int Id { get; set; }

        /// <inheritdoc />
        public string Name { get; set; }

        /// <summary>
        /// The droid's primary function.
        /// </summary>
        public string PrimaryFunction { get; set; }

        /// <inheritdoc />
        //NOTE: Updated to use v11 method...
        [UsePaging(type: typeof(InterfaceType<ICharacter>))]
        [GetFriendsResolver]
        //Establish configured link to required Selection for Character.Id for 
        //  resolvers that implement pre-processed results within the Repository Layer.
        [ResolverProcessingParentDependencies(nameof(ICharacter.Id))]
        public IReadOnlyList<ICharacter>? Friends { get; set; }

        /// <inheritdoc />
        public IReadOnlyList<Episode>? AppearsIn { get; set; }

        /// <inheritdoc />
        [UseConvertUnit]
        public double Height { get; set; } = 1.72d;
    }
}
