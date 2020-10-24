using HotChocolate.Types;

namespace StarWars.Characters
{
    //TODO: Try to fix this for v11???? 
    //[UnionType(Name = "SearchResult")]
    public interface ISearchResult
    {
        /// <summary>
        /// The Id of the starship.
        /// </summary>
        int Id { get; }

        /// <summary>
        /// The name of the starship.
        /// </summary>
        string Name { get; }
    }
}
