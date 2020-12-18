using System;
using System.Collections.Generic;
using System.Text;

namespace HotChocolate.PreProcessingExtensions.Arguments
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
