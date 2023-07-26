namespace HotChocolate.ResolverProcessingExtensions.Arguments
{
    public interface IArgumentValue
    {
        string Name { get; }
        object Value { get; }
    }
}