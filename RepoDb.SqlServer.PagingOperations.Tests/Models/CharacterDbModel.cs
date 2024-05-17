using RepoDb.Attributes;

namespace StarWars.Characters.DbModels
{
    [Map("[dbo].[StarWarsCharacters]")]
    public class CharacterDbModel
    {
        public int Id { get; set; }

        public string Name { get; set; }

        //public double Height { get; set; }

        public string HomePlanet { get; set; }

        public string PrimaryFunction { get; set; }

        public bool IsHuman => this.Id >= 1000 && this.Id <= 1999;

        public bool IsDroid => this.Id >= 2000 && this.Id <= 2999;
    }
}
