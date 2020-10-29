using HotChocolate.Data.Sorting;
using HotChocolate.Language;
using HotChocolate.Types;
using System;

namespace HotChocolate.PreProcessedExtensions.Sorting
{
    public class SortOrderField : ISortOrderField
    {
        public const string AscendingDescription = "ASC";
        public const string DescendingDescription = "DESC";

        public SortField  Field { get; }
        public string FieldName { get; }
        public string MemberName { get; }
        public string SortDirection { get; }

        public bool IsAscending() => this.SortDirection.StartsWith(AscendingDescription, StringComparison.OrdinalIgnoreCase);
        public bool IsDescending() => this.SortDirection.StartsWith(DescendingDescription, StringComparison.OrdinalIgnoreCase);

        public SortOrderField(SortField field, string sortDirection)
        {
            this.Field = field 
                ?? throw new ArgumentException("Input Field cannot be null.", nameof(field));

            this.FieldName = field.Name.Value
                ?? throw new ArgumentException("Field Name cannot be blank or null", "InputField.Name");

            this.MemberName = field.Member?.Name
                ?? throw new ArgumentException("Field Name cannot be blank or null", "InputField.Name");

            this.SortDirection = sortDirection
                ?? throw new ArgumentException("Sort Direction value cannot be blank or null", nameof(sortDirection));
        }

        public override string ToString()
        {
            return $"{{{this.FieldName}:{this.SortDirection}}}";
        }
    }
}
