using System;

namespace HotChocolate.ResolverProcessingExtensions.Arguments
{
    public class ArgumentValue : IArgumentValue
    {
        public ArgumentValue(string name, object value)
        {
            Name = name;
            Value = value;
        }
        public string Name { get; }
        public object Value { get; }
    }
}
