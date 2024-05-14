namespace RepoDb.CursorPaging
{
    /// <summary>
    /// Optional Marker interface that denotes a Model as specifically having a Cursor Index Field. Can be used for processing
    /// models that have a Cursor property and/or get value from being processed as an entity with CursorIndex value.
    /// When queried via Pagination Slice query this value may be populated automatically.
    /// </summary>
    public interface IHaveCursor
    {
        int CursorIndex { get; }
    }
}
