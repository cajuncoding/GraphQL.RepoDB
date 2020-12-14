using System;
using System.Collections.Generic;
using System.Text;
using HotChocolate.Types;

namespace GraphQL.PreProcessingExtensions.Tests
{
    [InterfaceType(Name = "Character")]
    public interface IStarWarsCharacter
    {
        public int Id { get; }
        public string Name { get; }
    }
}
