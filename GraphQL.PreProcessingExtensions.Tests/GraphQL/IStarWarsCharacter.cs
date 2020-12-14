using System;
using System.Collections.Generic;
using System.Text;

namespace GraphQL.PreProcessingExtensions.Tests
{
    public interface IStarWarsCharacter
    {
        public int Id { get; }
        public string Name { get; }
    }
}
