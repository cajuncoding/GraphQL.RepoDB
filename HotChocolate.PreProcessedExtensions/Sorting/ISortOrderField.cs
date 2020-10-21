namespace HotChocolate.PreProcessedExtensions.Sorting
{
    public interface ISortOrderField
    {
        string FieldName { get; }
        string SortDirection { get; }

        bool IsAscending();
        bool IsDescending();
        string ToString();
    }
}