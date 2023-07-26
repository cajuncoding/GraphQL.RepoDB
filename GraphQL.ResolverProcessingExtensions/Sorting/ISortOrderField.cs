using HotChocolate.Data.Sorting;

namespace HotChocolate.ResolverProcessingExtensions.Sorting
{
    public interface ISortOrderField
    {
        ISortingFieldInfo Field { get; }
        string FieldName { get; }
        string MemberName { get; }
        string SortDirection { get; }

        bool IsAscending();
        bool IsDescending();
        string ToString();
    }
}