namespace HotChocolate.PreProcessingExtensions.Arguments
{
    public interface IArgumentValue
    {
        string Name { get; }
        object Value { get; }
    }
}