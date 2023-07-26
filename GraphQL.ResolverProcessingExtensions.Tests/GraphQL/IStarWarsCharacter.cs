using System;
using System.Collections.Generic;
using System.Text;
using HotChocolate.Types;

namespace HotChocolate.ResolverProcessingExtensions.Tests
{
    [InterfaceType(Name = "Character")]
    public interface IStarWarsCharacter
    {
        public int Id { get; }
        public string Name { get; }
    }
}
