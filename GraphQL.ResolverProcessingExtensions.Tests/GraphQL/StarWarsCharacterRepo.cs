using System;
using System.Collections.Generic;
using System.Text;
using HotChocolate.ResolverProcessingExtensions.Tests;

namespace HotChocolate.ResolverProcessingExtensions.Tests
{
    internal class StarWarsCharacterRepo
    {
        public static IEnumerable<IStarWarsCharacter> CreateCharacters()
        {
            yield return new StarWarsHuman
            (
                1000,
                "Luke Skywalker",
                "Tatooine",
                new int[] {2000, 2001}
            );

            yield return new StarWarsHuman
            (
                1001,
                "Darth Vader",
                "Tatooine"
            );

            yield return new StarWarsHuman
            (
                1002,
                "Han Solo"
            );

            yield return new StarWarsHuman
            (
                1003,
                "Leia Organa",
                "Alderaan",
                new int[] { 2000, 2001 }
            );

            yield return new StarWarsHuman
            (
                1004,
                "Wilhuff Tarkin"
            );

            yield return new StarWarsDroid
            (
                2000,
                "C-3PO",
                "Protocol"
            );

            yield return new StarWarsDroid
            (
                2001,
                "R2-D2",
                "Astromech"
            );
        }
    }
}
