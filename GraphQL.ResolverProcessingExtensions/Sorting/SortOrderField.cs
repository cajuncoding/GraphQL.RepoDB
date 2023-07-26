using HotChocolate.Data.Sorting;
using System;

namespace HotChocolate.ResolverProcessingExtensions.Sorting
{
    public class SortOrderField : ISortOrderField
    {
        public const string AscendingDescription = "ASC";
        public const string DescendingDescription = "DESC";

        public ISortingFieldInfo Field { get; }
        public string FieldName { get; }
        public string MemberName { get; }
        public string SortDirection { get; }

        public bool IsAscending() => this.SortDirection.StartsWith(AscendingDescription, StringComparison.OrdinalIgnoreCase);
        public bool IsDescending() => this.SortDirection.StartsWith(DescendingDescription, StringComparison.OrdinalIgnoreCase);

        public SortOrderField(ISortingFieldInfo field, string sortDirection)
        {
            this.Field = field
                ?? throw new ArgumentException("Input Field cannot be null.", nameof(field));

            this.FieldName = field.Field.Name
                ?? throw new ArgumentException("Field Name cannot be blank or null", "InputField.Name");

            this.MemberName = field.Field.Member?.Name
                ?? throw new ArgumentException("Field Name cannot be blank or null", "InputField.Member.Name");

            this.SortDirection = sortDirection
                ?? throw new ArgumentException("Sort Direction value cannot be blank or null", nameof(sortDirection));
        }

        public override string ToString()
        {
            return $"{{{this.FieldName}:{this.SortDirection}}}";
        }
    }
}
