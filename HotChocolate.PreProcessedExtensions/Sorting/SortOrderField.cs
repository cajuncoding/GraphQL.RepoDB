using System;

namespace HotChocolate.PreProcessedExtensions.Sorting
{
    public class SortOrderField : ISortOrderField
    {
        public const string AscendingDescription = "ASC";
        public const string DescendingDescription = "DESC";

        public string FieldName { get; }
        public string SortDirection { get; }

        public bool IsAscending() => this.SortDirection.StartsWith(AscendingDescription, StringComparison.OrdinalIgnoreCase);
        public bool IsDescending() => this.SortDirection.StartsWith(DescendingDescription, StringComparison.OrdinalIgnoreCase);

        public SortOrderField(string fieldName, string sortDirectionText = AscendingDescription)
        {
            this.FieldName = string.IsNullOrWhiteSpace(fieldName)
                ? throw new ArgumentException("Field Name cannot be blank or null", nameof(fieldName))
                : fieldName;

            this.SortDirection = string.IsNullOrWhiteSpace(sortDirectionText)
                ? throw new ArgumentException("Sort Direction text cannot be blank or null", nameof(sortDirectionText))
                : sortDirectionText;
        }

        public override string ToString()
        {
            return $"{{{this.FieldName}:{this.SortDirection}}}";
        }
    }
}
