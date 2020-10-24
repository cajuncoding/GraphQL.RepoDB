using RepoDb.Attributes;
using StarWars.Characters;
using System;
using System.Collections.Generic;
using System.Text;

namespace StarWars.Characters
{
    [Map("StarWarsCharacters")]
    public class Character : ICharacter
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public IReadOnlyList<int> Friends { get; set; }

        public IReadOnlyList<Episode> AppearsIn { get; set; }

        public double Height { get; set; }
    }
}
