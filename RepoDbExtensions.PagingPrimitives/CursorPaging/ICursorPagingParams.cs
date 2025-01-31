namespace RepoDb.PagingPrimitives.CursorPaging
{
    public interface ICursorPagingParams
    {
        int? First { get; }
        string After { get; }
        int? Last { get; }
        string Before { get; }
        bool RetrieveTotalCount { get; }
    }
}