using System.Collections.Generic;
using HotChocolate;
using HotChocolate.Types;
using HotChocolate.Types.Relay;

namespace StarWars.Characters
{
    /// <summary>
    /// A character in the Star Wars universe.
    /// </summary>
    //TODO: Fix this Interface type when SubTypes are re-registered in Starup...
    [InterfaceType(Name = "Character")]
    public interface ICharacter : ISearchResult
    {
        /// <summary>
        /// The unique identifier for the character.
        /// </summary>
        [GraphQLName("personalIdentifier")]
        int Id { get; }

        /// <summary>
        /// The name of the character.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// The ids of the character's friends.
        /// </summary>
        //NOTE: Updated to use v11 method...
        [UsePaging(type: typeof(InterfaceType<ICharacter>))]
        IReadOnlyList<ICharacter> Friends { get; }

        /// <summary>
        /// The episodes the character appears in.
        /// </summary>
        IReadOnlyList<Episode> AppearsIn { get; }

        /// <summary>
        /// The height of the character.
        /// </summary>
        [UseConvertUnit]
        double Height { get; }
    }
}
