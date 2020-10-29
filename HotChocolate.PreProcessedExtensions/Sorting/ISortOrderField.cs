using HotChocolate.Data.Sorting;

namespace HotChocolate.PreProcessedExtensions.Sorting
{
    public interface ISortOrderField
    {
        SortField Field { get; }
        string FieldName { get; }
        string MemberName { get; }
        string SortDirection { get; }

        bool IsAscending();
        bool IsDescending();
        string ToString();
    }
}