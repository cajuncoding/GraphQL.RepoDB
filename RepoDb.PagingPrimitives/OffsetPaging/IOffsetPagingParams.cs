namespace RepoDb.PagingPrimitives.OffsetPaging
{
    public interface IOffsetPagingParams
    {
        int? Skip { get; }
        int? Take { get; }
        bool IsTotalCountRequested { get; }
    }
}